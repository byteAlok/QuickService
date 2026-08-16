using QuickService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuickService.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        public readonly DatabaseCon db;
        private readonly Services.IProfileImageService _profileImageService;
        public StaffController(DatabaseCon context, Services.IProfileImageService profileImageService)
        {
            db = context;
            _profileImageService = profileImageService;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Staff");
        }

        public async Task<IActionResult> Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("StaffId")))
            {
                return RedirectToAction("Login", "Auth");
            }

            int staffId = int.Parse(HttpContext.Session.GetString("StaffId")!);
            
            // TASK 1: STAFF DASHBOARD CARDS DYNAMIC
            var totalJobs = await db.booking_table.AsNoTracking().CountAsync(x => x.BookingBy == "Staff" && x.foreign_key_id == staffId);
            var pendingJobs = await db.booking_table.AsNoTracking().CountAsync(x => x.BookingBy == "Staff" && x.foreign_key_id == staffId && x.service_status == "Pending");
            var completedJobs = await db.booking_table.AsNoTracking().CountAsync(x => x.BookingBy == "Staff" && x.foreign_key_id == staffId && x.service_status == "Completed");
            
            // Today's jobs
            var today = DateTime.UtcNow.Date;
            var todayJobs = await db.booking_table.AsNoTracking().CountAsync(x => x.BookingBy == "Staff" && x.foreign_key_id == staffId && x.BookingDate >= today && x.BookingDate < today.AddDays(1));

            ViewBag.TotalJobs = totalJobs;
            ViewBag.PendingJobs = pendingJobs;
            ViewBag.CompletedJobs = completedJobs;
            ViewBag.TodayJobs = todayJobs;

            // TASK 2: RECENT BOOKINGS
            var recentBookings = await db.booking_table
                .AsNoTracking()
                .Where(x => x.BookingBy == "Staff" && x.foreign_key_id == staffId)
                .OrderByDescending(x => x.id)
                .Take(7)
                .ToListAsync();

            return View(recentBookings);
        }
        public IActionResult BookingList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("StaffId")))
            {
                return RedirectToAction("Login", "Auth");
            }

            int staffId = int.Parse(HttpContext.Session.GetString("StaffId")!);
            var staff = db.staff_table.FirstOrDefault(x => x.staff_id == staffId);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(StaffModel model, IFormFile? staff_image)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("StaffId")))
            {
                return RedirectToAction("Login", "Auth");
            }

            int staffId = int.Parse(HttpContext.Session.GetString("StaffId")!);
            var staff = db.staff_table.FirstOrDefault(x => x.staff_id == staffId);

            if (staff == null) return NotFound();

            // Only update non-readonly fields
            staff.staff_name = model.staff_name;
            staff.staff_email = model.staff_email;
            staff.staff_phone = model.staff_phone;
            staff.staff_address = model.staff_address;
            staff.staff_city_state = model.staff_city_state;

            if (staff_image != null)
            {
                string? oldImagePath = staff.staff_image;
                string? newImagePath = await _profileImageService.UploadImageAsync(staff_image, "staff", staff.staff_name, "staff");

                if (!string.IsNullOrEmpty(newImagePath))
                {
                    staff.staff_image = newImagePath;
                    _profileImageService.DeleteOldImage(oldImagePath);
                }
            }

            db.staff_table.Update(staff);
            var result = db.SaveChanges();

            if (result > 0)
            {
                // Update Session
                HttpContext.Session.SetString("StaffName", staff.staff_name);
                HttpContext.Session.SetString("StaffEmail", staff.staff_email);
                HttpContext.Session.SetString("StaffPhone", staff.staff_phone);
                HttpContext.Session.SetString("StaffAddress", staff.staff_address ?? "");
                HttpContext.Session.SetString("StaffCityState", staff.staff_city_state ?? "");
                HttpContext.Session.SetString("StaffImage", staff.staff_image ?? "");

                TempData["StaffUpdateStatus-Profile"] = "Profile updated successfully!";
                TempData["StaffUpdateType-Profile"] = "success";
            }
            else
            {
                TempData["StaffUpdateStatus-Profile"] = "Failed to update profile or no changes made.";
                TempData["StaffUpdateType-Profile"] = "error";
            }

            return RedirectToAction("Profile");
        }

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
            model.BookingBy = "Staff";
            
            // Store Staff ID in foreign_key_id
            if (HttpContext.Session.GetString("StaffId") != null)
            {
                model.foreign_key_id = int.Parse(HttpContext.Session.GetString("StaffId")!);
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
                    TempData["BookingStatus-staff"] = "Your booking has been submitted successfully!";
                    TempData["BookingType-staff"] = "success";
                }
                else
                {
                    TempData["BookingStatus-staff"] = "Server-side error or failed!. Please try again.";
                    TempData["BookingType-staff"] = "error";
                }

                return RedirectToAction("ManualBooking", "Staff");
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
            TempData["BookingStatus-staff"] = "Something went wrong. Please try again.";
            TempData["BookingType-staff"] = "error";

            return View("ManualBooking", model);
        }

        // ----------- Search Booking by ID (for only that was booked by specific staff) ----------- 
        public IActionResult SearchBooking()
        {
            return View();
        }
    }
}
