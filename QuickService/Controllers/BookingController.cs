using QuickService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace QuickService.Controllers
{
    public class BookingController : Controller
    {
        public readonly DatabaseCon db;
        private readonly ILogger<BookingController> _logger;
        public BookingController(DatabaseCon context, ILogger<BookingController> logger)
        {
            db = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ServiceBooking()
        {
            return View();
        }

        // Service Booking Form Submission (for all users: Admin, Staff, and default user) : But only from our website not api --------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceBooking(BookingModel model)
        {
            model.service_status = "Pending";

            // Default booking (no login)
            model.BookingBy = "User";
            model.foreign_key_id = 0;

            // Check Admin or Manager (both treated as Admin)
            var adminId = HttpContext.Session.GetString("AdminId");

            if (!string.IsNullOrEmpty(adminId))
            {
                model.BookingBy = "Admin";
                model.foreign_key_id = int.Parse(adminId);
            }
            else
            {
                // Check Staff login
                var staffId = HttpContext.Session.GetString("StaffId");

                if (!string.IsNullOrEmpty(staffId))
                {
                    model.BookingBy = "Staff";
                    model.foreign_key_id = int.Parse(staffId);
                }
            }

            if (ModelState.IsValid)
            {
                db.booking_table.Add(model);
                var rows = db.SaveChanges();

                if (rows > 0)
                {
                    TempData["BookingStatus"] = "Your booking has been submitted successfully!";
                    TempData["BookingType"] = "success"; 
                }
                else
                {
                    TempData["BookingStatus"] = "Server-side error or failed!. Please try again.";
                    TempData["BookingType"] = "error";
                }

                return RedirectToAction("ServiceBooking", "Booking");
            }
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        //Console.WriteLine($"Field: {state.Key} | Error: {error.ErrorMessage}");

                        _logger.LogWarning("Model validation failed. Field: {Field}," +
                            " Error: {Error}",
                            state.Key,
                            error.ErrorMessage);
                    }
                }

                return Content("Model Invalid");
            }
            TempData["BookingStatus"] = "Something went wrong. Please try again.";
            TempData["BookingType"] = "error";

            return View("ServiceBooking", model);
        }

        // Admin Booking List for SuperAdmin and Manager ------------------

        [Authorize(Roles = "SuperAdmin, Manager")]
        [HttpGet]
        public IActionResult AdminBookingList(int page = 1, string status = "All", string createdBy = "All", string dateRange = "All")
        {
            var query = db.booking_table.AsQueryable();

            // Status Filter
            if (status != "All")
            {
                query = query.Where(b => b.service_status == status);
            }

            // Created By Filter
            if (createdBy != "All")
            {
                query = query.Where(b => b.BookingBy == createdBy);
            }

            // Date Range Filter
            query = ApplyDateFilter(query, dateRange);

            // Pagination
            int pageSize = 10;
            int totalRecords = query.Count();
            var bookings = query
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentCreatedBy = createdBy;
            ViewBag.CurrentDateRange = dateRange;

            // Resolve Staff and Admin Names
            ViewBag.StaffNames = db.staff_table.ToDictionary(s => s.staff_id, s => s.staff_name);
            ViewBag.AdminNames = db.admin_table.ToDictionary(a => a.admin_id, a => a.admin_name);

            return View("~/Views/Admin/BookingList.cshtml", bookings);
        }

        // Staff Booking List for logged-in staff ------------------ 

        [Authorize(Roles = "Staff")]
        [HttpGet]
        public IActionResult StaffBookingList(int page = 1, string status = "All", string dateRange = "All")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("StaffId")))
            {
                return RedirectToAction("Login", "Auth");
            }

            int staffId = int.Parse(HttpContext.Session.GetString("StaffId")!);
            var query = db.booking_table.Where(b => b.BookingBy == "Staff" && b.foreign_key_id == staffId);

            // Status Filter
            if (status != "All")
            {
                query = query.Where(b => b.service_status == status);
            }

            // Date Range Filter
            query = ApplyDateFilter(query, dateRange);

            // Pagination
            int pageSize = 10;
            int totalRecords = query.Count();
            var bookings = query
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentDateRange = dateRange;

            // Resolve Staff and Admin Names
            ViewBag.StaffNames = db.staff_table.ToDictionary(s => s.staff_id, s => s.staff_name);
            ViewBag.AdminNames = db.admin_table.ToDictionary(a => a.admin_id, a => a.admin_name);

            return View("~/Views/Staff/BookingList.cshtml", bookings);
        }

        // Update Booking Status via fetch API call from SuperAdmin/Manager and Staff Booking List --- 

        [Authorize(Roles = "SuperAdmin, Manager, Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateBookingStatus(int id)
        {
            var booking = db.booking_table.Find(id);

            if (booking == null)
                return Json(new { success = false, message = "Booking not found" });

            // Staff can update only their own bookings
            if (User.IsInRole("Staff"))
            {
                var staffIdString = HttpContext.Session.GetString("StaffId");

                if (!int.TryParse(staffIdString, out int staffId))
                    return Unauthorized();

                if (booking.BookingBy != "Staff" || booking.foreign_key_id != staffId)
                    return Forbid();
            }

            // SuperAdmin and Manager can update any booking

            if (booking.service_status != "Pending")
            {
                return Json(new
                {
                    success = false,
                    message = "Status cannot be updated"
                });
            }

            booking.service_status = "Completed";
            db.SaveChanges();

            return Json(new
            {
                success = true,
                newStatus = "Completed"
            });
        }

        private IQueryable<BookingModel> ApplyDateFilter(IQueryable<BookingModel> query, string dateRange)
        {
            var now = DateTime.Now;
            var today = DateTime.Today;

            switch (dateRange)
            {
                case "Today":
                    query = query.Where(b => b.BookingDate.Date == today);
                    break;
                case "Yesterday":
                    var yesterday = today.AddDays(-1);
                    query = query.Where(b => b.BookingDate.Date == yesterday);
                    break;
                case "Last24Hours":
                    var last24 = now.AddHours(-24);
                    query = query.Where(b => b.BookingDate >= last24);
                    break;
                case "ThisWeek":
                    int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    var startOfWeek = today.AddDays(-1 * diff).Date;
                    query = query.Where(b => b.BookingDate >= startOfWeek);
                    break;
                case "Last7Days":
                    var last7 = today.AddDays(-7);
                    query = query.Where(b => b.BookingDate >= last7);
                    break;
                case "ThisMonth":
                    var startOfMonth = new DateTime(today.Year, today.Month, 1);
                    query = query.Where(b => b.BookingDate >= startOfMonth);
                    break;
                case "Last30Days":
                    var last30 = today.AddDays(-30);
                    query = query.Where(b => b.BookingDate >= last30);
                    break;
                case "Last3Months":
                    var last3m = today.AddDays(-90);
                    query = query.Where(b => b.BookingDate >= last3m);
                    break;
            }
            return query;
        }

        // Authorized users can share this details with technician via WhatsApp or Email.
        // SuperAdmin, Manager, and Staff can share the booking details. Staff can only share their own bookings.  
        // The booking details will be shared in JSON format. So all the details will be available to the technician.

        [Authorize(Roles = "SuperAdmin, Manager, Staff")]
        [HttpGet]
        public IActionResult ShareMessage(int id) 
        {
            var booking = db.booking_table.Find(id);

            if (booking == null)
                return NotFound();

            if (User.IsInRole("Staff"))
            {
                var staffIdString = HttpContext.Session.GetString("StaffId");

                if (!int.TryParse(staffIdString, out int staffId))
                    return Unauthorized();

                if (booking.BookingBy != "Staff" || booking.foreign_key_id != staffId)
                    return Forbid();
            }
            return Json(new { success = true, booking });
        }
    }
}

