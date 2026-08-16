using QuickService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace QuickService.Controllers
{
    [Authorize(Roles = "SuperAdmin, Manager")]
    public class AdminController : Controller
    {
        public readonly DatabaseCon db;
        private readonly Services.IProfileImageService _profileImageService;
        public AdminController(DatabaseCon context, Services.IProfileImageService profileImageService)
        {
            db = context;
            _profileImageService = profileImageService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        public async Task<IActionResult> Dashboard()
        {
            // TASK 1: DASHBOARD CARDS DYNAMIC
            var totalBookings = await db.booking_table.AsNoTracking().CountAsync();
            var pendingBookings = await db.booking_table.AsNoTracking().CountAsync(x => x.service_status == "Pending");
            var completedBookings = await db.booking_table.AsNoTracking().CountAsync(x => x.service_status == "Completed");
            var totalStaff = await db.staff_table.AsNoTracking().CountAsync(x => x.staff_status == true);

            ViewBag.TotalBookings = totalBookings;
            ViewBag.PendingBookings = pendingBookings;
            ViewBag.CompletedBookings = completedBookings;
            ViewBag.TotalStaff = totalStaff;

            // SuperAdmin only stats
            if (HttpContext.Session.GetString("AdminType") == "SuperAdmin")
            {
                ViewBag.TotalAdmins = await db.admin_table.AsNoTracking().CountAsync(x => x.admin_status == true);
            }

            // TASK 2: RECENT BOOKINGS (Last 7-10)
            var recentBookings = await db.booking_table
                .AsNoTracking()
                .OrderByDescending(x => x.id) // Assuming id or created_at
                .Take(10)
                .ToListAsync();

            // Store Names for display (Task 2 compatibility)
            var staffIds = recentBookings.Where(b => b.BookingBy == "Staff").Select(b => b.foreign_key_id.GetValueOrDefault()).Distinct().ToList();
            var adminIds = recentBookings.Where(b => b.BookingBy == "Admin").Select(b => b.foreign_key_id.GetValueOrDefault()).Distinct().ToList();

            ViewBag.StaffNames = await db.staff_table.AsNoTracking()
                .Where(s => staffIds.Contains(s.staff_id))
                .ToDictionaryAsync(s => s.staff_id, s => s.staff_name);
            
            ViewBag.AdminNames = await db.admin_table.AsNoTracking()
                .Where(a => adminIds.Contains(a.admin_id))
                .ToDictionaryAsync(a => a.admin_id, a => a.admin_name);

            return View(recentBookings);
        }
        // =================================================================================

        public IActionResult BookingList()
        {
            return View();
        }
        // ==================================================================================

        [HttpGet]
        public IActionResult ManualBooking()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ManualBooking(BookingModel model) 
        {
            model.service_status = "Pending";
            model.BookingBy = "Admin";
            
            // Store Admin ID in foreign_key_id
            if (HttpContext.Session.GetString("AdminId") != null)
            {
                model.foreign_key_id = int.Parse(HttpContext.Session.GetString("AdminId")!);
            }
            else
            {
                model.foreign_key_id = 0;
            }

            if (ModelState.IsValid)
            {
                db.booking_table.Add(model);
                var rows = db.SaveChanges();

                if (rows > 0)
                {
                    TempData["BookingStatus-admin"] = "Your booking has been submitted successfully!";
                    TempData["BookingType-admin"] = "success"; 
                }
                else
                {
                    TempData["BookingStatus-admin"] = "Server-side error or failed!. Please try again.";
                    TempData["BookingType-admin"] = "error";
                }

                return RedirectToAction("ManualBooking", "Admin");
            }
            //if (!ModelState.IsValid)
            //{
            //    foreach (var state in ModelState)
            //    {
            //        foreach (var error in state.Value.Errors)
            //        {
            //            Console.WriteLine($"Field: {state.Key} | Error: {error.ErrorMessage}");
            //        }
            //    }

            //    return Content("Model Invalid");
            //}
            TempData["BookingStatus-admin"] = "Something went wrong. Please try again.";
            TempData["BookingType-admin"] = "error";

            return View("ManualBooking", model);
        }
        // ==============================================================================================================
    
        [HttpGet]
        public async Task<IActionResult> StaffList(string? status, string? search, int page = 1)
        {
            int pageSize = 10;
            var query = db.staff_table.AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                bool isActive = status == "Active";
                query = query.Where(x => x.staff_status == isActive);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.staff_name.Contains(search) || x.staff_email.Contains(search));
            }

            int totalRecords = await query.CountAsync();
            var staff = await query
                .OrderByDescending(x => x.staff_register_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var s in staff)
            {
                s.staff_last_login = s.staff_last_login.HasValue ? QuickService.Helpers.TimeHelper.ConvertToIst(s.staff_last_login.Value) : null;
                s.staff_register_date = QuickService.Helpers.TimeHelper.ConvertToIst(s.staff_register_date);
                s.staff_lock_until = s.staff_lock_until.HasValue ? QuickService.Helpers.TimeHelper.ConvertToIst(s.staff_lock_until.Value) : null;
                s.otp_block_until = s.otp_block_until.HasValue ? QuickService.Helpers.TimeHelper.ConvertToIst(s.otp_block_until.Value) : null;
            }

            ViewBag.TotalRecords = totalRecords;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentStatus = status ?? "All";
            ViewBag.SearchTerm = search;

            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(StaffModel model)
        {
            var staff = await db.staff_table.FindAsync(model.staff_id);
            if (staff == null) return NotFound();

            // Check if email changed and if it's unique
            if (staff.staff_email != model.staff_email)
            {
                var existing = await db.staff_table.AnyAsync(x => x.staff_email == model.staff_email);
                if (existing)
                {
                    TempData["StaffUpdateStatus"] = "Email already in use by another staff.";
                    TempData["StaffUpdateType"] = "error";
                    return RedirectToAction("StaffList");
                }
                staff.staff_email = model.staff_email;
            }

            staff.staff_name = model.staff_name;
            staff.staff_phone = model.staff_phone;
            staff.staff_skill = model.staff_skill;
            staff.staff_address = model.staff_address;
            staff.staff_city_state = model.staff_city_state;
            staff.staff_status = model.staff_status;

            db.staff_table.Update(staff);
            await db.SaveChangesAsync();

            TempData["StaffUpdateStatus"] = "Staff updated successfully!";
            TempData["StaffUpdateType"] = "success";

            return RedirectToAction("StaffList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableStaff(int id)
        {
            var staff = await db.staff_table.FindAsync(id);
            if (staff == null) return NotFound();

            staff.staff_status = false;
            db.staff_table.Update(staff);
            await db.SaveChangesAsync();

            TempData["StaffUpdateStatus"] = "Staff disabled successfully!";
            TempData["StaffUpdateType"] = "success";

            return RedirectToAction("StaffList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableStaff(int id)
        {
            var staff = await db.staff_table.FindAsync(id);
            if (staff == null) return NotFound();

            staff.staff_status = true;
            db.staff_table.Update(staff);
            await db.SaveChangesAsync();

            TempData["StaffUpdateStatus"] = "Staff enabled successfully!";
            TempData["StaffUpdateType"] = "success";

            return RedirectToAction("StaffList");
        }
        // ==============================================================================================================
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> AdminList(string? status, string? type, string? search, int page = 1)
        {
            int pageSize = 10;
            var query = db.admin_table.AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                bool isActive = status == "Active";
                query = query.Where(x => x.admin_status == isActive);
            }

            if (!string.IsNullOrEmpty(type) && type != "All")
            {
                query = query.Where(x => x.admin_type == type);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.admin_name.Contains(search) || x.admin_email.Contains(search));
            }

            int totalRecords = await query.CountAsync();
            var admins = await query
                .OrderByDescending(x => x.admin_register_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var a in admins)
            {
                a.admin_last_login = a.admin_last_login.HasValue ? QuickService.Helpers.TimeHelper.ConvertToIst(a.admin_last_login.Value) : null;
                a.admin_register_date = QuickService.Helpers.TimeHelper.ConvertToIst(a.admin_register_date);
                a.admin_lock_until = a.admin_lock_until.HasValue ? QuickService.Helpers.TimeHelper.ConvertToIst(a.admin_lock_until.Value) : null;
                a.otp_block_until = a.otp_block_until.HasValue ? QuickService.Helpers.TimeHelper.ConvertToIst(a.otp_block_until.Value) : null;
            }

            ViewBag.TotalRecords = totalRecords;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentStatus = status ?? "All";
            ViewBag.CurrentType = type ?? "All";
            ViewBag.SearchTerm = search;

            return View(admins);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(AdminModel model)
        {
            var admin = await db.admin_table.FindAsync(model.admin_id);
            if (admin == null) return NotFound();

            int currentAdminId = int.Parse(HttpContext.Session.GetString("AdminId") ?? "0");

            // Self-disable and SuperAdmin protection
            if (!model.admin_status)
            {
                if (model.admin_id == currentAdminId)
                {
                    TempData["AdminUpdateStatus"] = "You cannot disable your own account.";
                    TempData["AdminUpdateType"] = "error";
                    return RedirectToAction("AdminList");
                }

                if (admin.admin_type == "SuperAdmin")
                {
                    TempData["AdminUpdateStatus"] = "Super Admin accounts cannot be disabled.";
                    TempData["AdminUpdateType"] = "error";
                    return RedirectToAction("AdminList");
                }
            }

            // Check if email changed and if it's unique
            if (admin.admin_email != model.admin_email)
            {
                var existing = await db.admin_table.AnyAsync(x => x.admin_email == model.admin_email);
                if (existing)
                {
                    TempData["AdminUpdateStatus"] = "Email already in use by another admin.";
                    TempData["AdminUpdateType"] = "error";
                    return RedirectToAction("AdminList");
                }
                admin.admin_email = model.admin_email;
            }

            // Update allowed fields
            admin.admin_name = model.admin_name;
            admin.admin_phone = model.admin_phone;
            admin.admin_address = model.admin_address;
            admin.admin_city_state = model.admin_city_state;
            admin.admin_type = model.admin_type;
            admin.admin_status = model.admin_status;

            db.admin_table.Update(admin);
            await db.SaveChangesAsync();

            // TASK 4: Refresh authentication cookie if role changed for CURRENT USER
            if (admin.admin_id == currentAdminId)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.admin_id.ToString()),
                    new Claim(ClaimTypes.Name, admin.admin_name),
                    new Claim(ClaimTypes.Email, admin.admin_email),
                    new Claim(ClaimTypes.Role, admin.admin_type)
                };

                var identity = new ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true
                });

                // Update Session Role
                HttpContext.Session.SetString("AdminType", admin.admin_type);
            }

            TempData["AdminUpdateStatus"] = "Admin updated successfully!";
            TempData["AdminUpdateType"] = "success";

            return RedirectToAction("AdminList");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableAdmin(int id)
        {
            var admin = await db.admin_table.FindAsync(id);
            if (admin == null) return NotFound();

            // Self-disable and SuperAdmin protection
            int currentAdminId = int.Parse(HttpContext.Session.GetString("AdminId") ?? "0");
            if (id == currentAdminId)
            {
                TempData["AdminUpdateStatus"] = "You cannot disable your own account.";
                TempData["AdminUpdateType"] = "error";
                return RedirectToAction("AdminList");
            }

            if (admin.admin_type == "SuperAdmin")
            {
                TempData["AdminUpdateStatus"] = "Super Admin accounts cannot be disabled.";
                TempData["AdminUpdateType"] = "error";
                return RedirectToAction("AdminList");
            }

            admin.admin_status = false;
            db.admin_table.Update(admin);
            await db.SaveChangesAsync();

            TempData["AdminUpdateStatus"] = "Admin disabled successfully!";
            TempData["AdminUpdateType"] = "success";

            return RedirectToAction("AdminList");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAdmin(int id)
        {
            var admin = await db.admin_table.FindAsync(id);
            if (admin == null) return NotFound();

            admin.admin_status = true;
            db.admin_table.Update(admin);
            await db.SaveChangesAsync();

            TempData["AdminUpdateStatus"] = "Admin enabled successfully!";
            TempData["AdminUpdateType"] = "success";

            return RedirectToAction("AdminList");
        }

        // ==============================================================================================================
    
        [HttpGet]
        public IActionResult RegisterStaff() 
        {
            return View();
        }
 
        [HttpPost]
        public IActionResult RegisterStaff(StaffModel staff)
        {
            if (!ModelState.IsValid)
            {
                return View(staff);
            }

            // Email unique check
            if (db.staff_table.Any(x => x.staff_email == staff.staff_email))
            {
                ModelState.AddModelError("staff_email", "Email already registered");

                TempData["StaffType"] = "Registration failed!. Please try again.";
                TempData["StaffStatus"] = "error";

                return View(staff);
            }

            // Password hashing
            var hasher = new PasswordHasher<StaffModel>();
            staff.staff_password = hasher.HashPassword(staff, staff.staff_password);

            // Default values
            staff.staff_status = true;
            staff.staff_last_login = null;

            // Optional fields agar aaye hain to automatically insert ho jayenge
            // EF Core null ko ignore kar deta hai

            db.staff_table.Add(staff);
            var row = db.SaveChanges();
            
            if (row > 0)
            {
                TempData["StaffType"] = "New Staff Registered Successfully!";
                TempData["StaffStatus"] = "success";

                return RedirectToAction("StaffList");   
            }
            else
            {
                TempData["StaffType"] = "Server-side error or failed!. Please try again.";
                TempData["StaffStatus"] = "error";
            }
            return View(staff);
        }
        // ==============================================================================================================
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult RegisterAdmin()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public IActionResult RegisterAdmin(AdminModel admin)
        {
            if (!ModelState.IsValid)
            {
                return View(admin);
            }

            // Email unique check
            if (db.admin_table.Any(x => x.admin_email == admin.admin_email))
            {
                ModelState.AddModelError("admin_email", "Email already registered");

                TempData["AdminType"] = "Registration failed!. Please try again.";
                TempData["AdminStatus"] = "error";

                return View(admin);
            }

            // Password hashing
            var hasher = new PasswordHasher<AdminModel>();
            admin.admin_password = hasher.HashPassword(admin, admin.admin_password);

            // Default values
            admin.admin_status = true;
            admin.admin_last_login = null;

            // Optional fields agar aaye hain to automatically insert ho jayenge
            // EF Core null ko ignore kar deta hai

            db.admin_table.Add(admin);
            var row = db.SaveChanges();

            if (row > 0)
            {
                TempData["AdminType"] = "New Admin Registered Successfully!";
                TempData["AdminStatus"] = "success";

                return RedirectToAction("AdminList");
            }
            else
            {
                TempData["AdminType"] = "Server-side error or failed!. Please try again.";
                TempData["AdminStatus"] = "error";
            }
            return View(admin);
        }
        // ==============================================================================================================
       
        [HttpGet]
        public IActionResult Settings()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Login", "Auth");
            }

            int adminId = int.Parse(HttpContext.Session.GetString("AdminId")!);
            var admin = db.admin_table.FirstOrDefault(x => x.admin_id == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            // Do not convert here to allow GetTimeAgo to work with UTC in the view
            // The view will handle IST conversion for direct display
            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSettings(AdminModel model, IFormFile? admin_image)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Login", "Auth");
            }

            int adminId = int.Parse(HttpContext.Session.GetString("AdminId")!);
            var admin = db.admin_table.FirstOrDefault(x => x.admin_id == adminId);

            if (admin == null) return NotFound();

            // Only update non-readonly fields
            admin.admin_name = model.admin_name;
            admin.admin_email = model.admin_email;
            admin.admin_phone = model.admin_phone;
            admin.admin_address = model.admin_address;
            admin.admin_city_state = model.admin_city_state;

            if (admin_image != null)
            {
                string? oldImagePath = admin.admin_image;
                string? newImagePath = await _profileImageService.UploadImageAsync(admin_image, "admin", admin.admin_name, "admin");

                if (!string.IsNullOrEmpty(newImagePath))
                {
                    admin.admin_image = newImagePath;
                    // Delete old image only after successful upload
                    _profileImageService.DeleteOldImage(oldImagePath);
                }
            }

            db.admin_table.Update(admin);
            var result = db.SaveChanges();

            if (result > 0)
            {
                // Update Session
                HttpContext.Session.SetString("AdminName", admin.admin_name);
                HttpContext.Session.SetString("AdminEmail", admin.admin_email);
                HttpContext.Session.SetString("AdminPhone", admin.admin_phone);
                HttpContext.Session.SetString("AdminAddress", admin.admin_address ?? "");
                HttpContext.Session.SetString("AdminCityState", admin.admin_city_state ?? "");
                HttpContext.Session.SetString("AdminImage", admin.admin_image ?? "");

                TempData["AdminUpdateStatus-Profile"] = "Profile updated successfully!";
                TempData["AdminUpdateType-Profile"] = "success";
            }
            else
            {
                TempData["AdminUpdateStatus-Profile"] = "Failed to update profile or no changes made.";
                TempData["AdminUpdateType-Profile"] = "error";
            }

            return RedirectToAction("Settings");
        }

        // ----------- Search Booking by ID (for SuperAdmin/Manager) ----------- 

        public IActionResult SearchBooking()
        {
            return View();
        }

    }
}
