using QuickService.Models;
using Education.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Security.Cryptography;

namespace QuickService.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseCon db;
        private readonly EmailServices emailService;
        public AuthController(DatabaseCon context, EmailServices _emailService)
        {
            db = context;
            emailService = _emailService;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [EnableRateLimiting("RateLimit")]
        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthModel user)
        {
            if (!ModelState.IsValid) return View(user);

            string email = user.Email.Trim().ToLower();

            var hasher = new PasswordHasher<object>();

            // ADMIN LOGIN (SuperAdmin / Manager)

            var admin = db.admin_table.FirstOrDefault(x => x.admin_email.ToLower() == email);

            if (admin != null)
            {
                // check if locked
                if (admin.admin_lock_until != null && admin.admin_lock_until > DateTime.UtcNow)
                {
                    ModelState.AddModelError("", "Account locked. Try again after 60 Minutes.");
                    return View(user);
                }

                // check status
                if (!admin.admin_status)
                {
                    ModelState.AddModelError("", "Admin account disabled/deleted.");
                    return View(user);
                }

                var result = hasher.VerifyHashedPassword(null, admin.admin_password, user.Password );

                if (result == PasswordVerificationResult.Success)
                {
                    // reset failed attempts
                    admin.admin_failed_attempts = 0;
                    admin.admin_lock_until = null;

                    // update last login
                    admin.admin_last_login = DateTime.UtcNow;

                    await db.SaveChangesAsync();


                    // claims
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.admin_id.ToString()),
                    new Claim(ClaimTypes.Name, admin.admin_name),
                    new Claim(ClaimTypes.Email, admin.admin_email),
                    new Claim(ClaimTypes.Role, admin.admin_type)
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme );

                    var principal = new ClaimsPrincipal(identity);

                    // clear previous session
                    HttpContext.Session.Clear();

                    // login cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = true,
                        AllowRefresh = true
                    });

                    HttpContext.Session.SetString("AdminId", admin.admin_id.ToString());
                    HttpContext.Session.SetString("AdminName", admin.admin_name);
                    HttpContext.Session.SetString("AdminEmail", admin.admin_email);
                    HttpContext.Session.SetString("AdminPhone", admin.admin_phone);
                    HttpContext.Session.SetString("AdminAddress", admin.admin_address ?? "");
                    HttpContext.Session.SetString("AdminCityState", admin.admin_city_state ?? "");
                    HttpContext.Session.SetString("AdminImage", admin.admin_image ?? "");
                    HttpContext.Session.SetString("AdminRegisterDate", admin.admin_register_date.ToString());
                    HttpContext.Session.SetString("AdminLastLogin", admin.admin_last_login.ToString() ?? "");
                    HttpContext.Session.SetString("AdminType", admin.admin_type);

                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    // password wrong
                    //admin.admin_failed_attempts++;
                    admin.admin_failed_attempts = (admin.admin_failed_attempts ?? 0) + 1;

                    if (admin.admin_failed_attempts >= 5)
                    {
                        admin.admin_lock_until = DateTime.UtcNow.AddMinutes(60);
                        admin.admin_failed_attempts = 0;
                    }

                    await db.SaveChangesAsync();
                }
            }

            // ==========================================
            // STAFF LOGIN
            // ==========================================

            var staff = db.staff_table.FirstOrDefault(x => x.staff_email.ToLower() == email);

            if (staff != null)
            {
                if (staff.staff_lock_until != null && staff.staff_lock_until > DateTime.UtcNow)
                {
                    ModelState.AddModelError("", "Account locked. Try again after 60 Minutes.");
                    return View(user);
                }

                if (!staff.staff_status)
                {
                    ModelState.AddModelError("", "Staff account disabled/deleted.");
                    return View(user);
                }

                var result = hasher.VerifyHashedPassword( null, staff.staff_password, user.Password );

                if (result == PasswordVerificationResult.Success)
                {
                    staff.staff_failed_attempts = 0;
                    staff.staff_lock_until = null;

                    staff.staff_last_login = DateTime.UtcNow;

                    await db.SaveChangesAsync();


                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, staff.staff_id.ToString()),
                    new Claim(ClaimTypes.Name, staff.staff_name),
                    new Claim(ClaimTypes.Email, staff.staff_email),
                    new Claim(ClaimTypes.Role, "Staff")
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme );

                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.Session.Clear();

                    await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = true,
                        AllowRefresh = true
                    });

                    HttpContext.Session.SetString("StaffId", staff.staff_id.ToString());
                    HttpContext.Session.SetString("StaffName", staff.staff_name);
                    HttpContext.Session.SetString("StaffEmail", staff.staff_email);
                    HttpContext.Session.SetString("StaffPhone", staff.staff_phone);
                    HttpContext.Session.SetString("StaffAddress", staff.staff_address ?? "");
                    HttpContext.Session.SetString("StaffCityState", staff.staff_city_state ?? "");
                    HttpContext.Session.SetString("StaffImage", staff.staff_image ?? "");
                    HttpContext.Session.SetString("StaffRegisterDate", staff.staff_register_date.ToString());
                    HttpContext.Session.SetString("StaffLastLogin", staff.staff_last_login.ToString() ?? "");
                    HttpContext.Session.SetString("StaffSkill", staff.staff_skill);

                    return RedirectToAction("Dashboard", "Staff");
                }
                else
                {
                    //staff.staff_failed_attempts++;
                    staff.staff_failed_attempts = (staff.staff_failed_attempts ?? 0) + 1;

                    if (staff.staff_failed_attempts >= 5)
                    {
                        staff.staff_lock_until = DateTime.UtcNow.AddHours(1);
                        staff.staff_failed_attempts = 0;
                    }

                    await db.SaveChangesAsync();
                }
            }
            // INVALID LOGIN

            ModelState.AddModelError("", "Invalid email or password");

            return View(user);
        }
    // ===============================================================================================
        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            // remove authentication cookie
            await HttpContext.SignOutAsync();

            // clear session data
            HttpContext.Session.Clear();

            // optional: delete custom cookies if any
            Response.Cookies.Delete("QuickServiceAuth");

            return RedirectToAction("Login");
        }
        // ===============================================================================================
        [HttpGet]
        [Route("Forgot-Password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("Forgot-Password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            HttpContext.Session.SetString("UserType", model.User);

            if (!ModelState.IsValid) return View(model);

            string email = model.Email.Trim().ToLower();
            string otp = null;

            if (model.User == "Staff")
            {
                var staff = db.staff_table.FirstOrDefault(x => x.staff_email.ToLower() == email);

                if (staff == null)
                {
                    ModelState.AddModelError("", "Email not found.");
                    return View(model);
                }

                if (staff.otp_block_until != null && staff.otp_block_until > DateTime.UtcNow)
                {
                    ModelState.AddModelError("", "Too many OTP requests. Try again after 60 Minutes.");
                    return View(model);
                }

                if (staff.otp_send_count >= 3)
                {
                    staff.otp_block_until = DateTime.UtcNow.AddMinutes(60);
                    staff.otp_send_count = 0;

                    db.SaveChanges();

                    ModelState.AddModelError("", "Too many OTP requests. Try again after 60 Minutes.");
                    return View(model);
                }

                // OTP generate here
                //otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
                var result = await emailService.Email(email);

                if (!result.IsEmailSent)
                {
                    ModelState.AddModelError("", result.Msg);
                    return View(model);
                }

                // success message
                TempData["SuccessOTP"] = result.Msg;

                otp = result.Otp.ToString();

                // OTP send hone ke baad
                staff.otp_send_count = (staff.otp_send_count ?? 0) + 1;

                //Console.WriteLine($"Staff = {staff.otp_send_count}");
                //db.SaveChanges();
                //Console.WriteLine($"Staff = {staff.otp_send_count}");

            }
            else if (model.User == "Admin")
            {
                var admin = db.admin_table.FirstOrDefault(x => x.admin_email.ToLower() == email);

                if (admin == null)
                {
                    ModelState.AddModelError("", "Email not found.");
                    return View(model);
                }

                if (admin.otp_block_until != null && admin.otp_block_until > DateTime.UtcNow)
                {
                    ModelState.AddModelError("", "Too many OTP requests. Try again after 60 Minutes.");
                    return View(model);
                }

                if (admin.otp_send_count >= 3)
                {
                    admin.otp_block_until = DateTime.UtcNow.AddMinutes(60);
                    admin.otp_send_count = 0;

                    db.SaveChanges();

                    ModelState.AddModelError("", "Too many OTP requests. Try again after 60 Minutes.");
                    return View(model);
                }

                // OTP generate
                //otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
                var result = await emailService.Email(email);

                if (!result.IsEmailSent)
                {
                    ModelState.AddModelError("", result.Msg);
                    return View(model);
                }

                // success message
                TempData["SuccessOTP"] = result.Msg;

                otp = result.Otp.ToString();

                // OTP send hone ke baad
                admin.otp_send_count = (admin.otp_send_count ?? 0) + 1;

                //Console.WriteLine($"Admin = {admin.otp_send_count}");
                //db.SaveChanges();
                //Console.WriteLine($"Admin = {admin.otp_send_count}");
            }

            db.SaveChanges();

            HttpContext.Session.SetString("ResetEmail", email);
            HttpContext.Session.SetString("ResetOTP", otp);
            HttpContext.Session.SetString("OTPFlow", "true");
            HttpContext.Session.SetString("ResetPasswordFlow", "");
            HttpContext.Session.SetInt32("OTPRetryCount", 0);

            HttpContext.Session.SetString("SessionTimeout", DateTime.UtcNow.AddMinutes(10).ToString());

            // TODO: send email
            //Console.WriteLine($"OTP = {otp}");
            //Console.WriteLine($"Email = {email}");

            return RedirectToAction("VerifyOtp");
        }
        // ====================================================
        [HttpGet]
        [Route("OTP-Verification")]
        public IActionResult VerifyOtp()
        {
            var timeout = HttpContext.Session.GetString("SessionTimeout");

            DateTime timeoutStr;
            bool success = DateTime.TryParse(timeout, out timeoutStr);

            var retryCount = HttpContext.Session.GetInt32("OTPRetryCount") ?? 0;

            if (HttpContext.Session.GetString("OTPFlow") != "true")
            {
                TempData["ErrorType"] = "Unauthorized access. Please start again.";
                return RedirectToAction("ForgotPassword");
            }
            if (!success || DateTime.UtcNow > timeoutStr)
            {
                HttpContext.Session.Remove("ResetOTP");
                HttpContext.Session.Remove("ResetEmail");
                HttpContext.Session.Remove("OTPFlow");
                HttpContext.Session.Remove("ResetPasswordFlow");
                HttpContext.Session.Remove("OTPRetryCount");
                HttpContext.Session.Remove("UserType");
              
                TempData["ErrorType"] = "Session expired. Please start again.";
                return RedirectToAction("ForgotPassword");
            }
            if (retryCount >= 3)
            {
                HttpContext.Session.Remove("ResetOTP");
                HttpContext.Session.Remove("ResetEmail");
                HttpContext.Session.Remove("OTPFlow");
                HttpContext.Session.Remove("ResetPasswordFlow");
                HttpContext.Session.Remove("OTPRetryCount");
                HttpContext.Session.Remove("UserType");
                
                TempData["ErrorType"] = "Too many attempts. Please start again.";
                return RedirectToAction("ForgotPassword");
            }

            var em = HttpContext.Session.GetString("ResetEmail");

            var model = new OtpVerifyModel
            {
                Email = em
            };
            return View(model);
        }
        [HttpPost]
        [Route("OTP-Verification")]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOtp(OtpVerifyModel verifyData)
        {
            if (HttpContext.Session.GetString("OTPFlow") != "true")
            {
                TempData["ErrorType"] = "Unauthorized access. Please start again.";
                return RedirectToAction("ForgotPassword");
            }

            if (!ModelState.IsValid) return View(verifyData);

            var sessionOtp = HttpContext.Session.GetString("ResetOTP");

            if (sessionOtp == null)
            {
                TempData["ErrorType"] = "Session expired. Please start again.";
                return RedirectToAction("ForgotPassword");
            }

            if (verifyData.OTP != sessionOtp)
            {
                int retry = HttpContext.Session.GetInt32("OTPRetryCount") ?? 0;
                retry++;

                HttpContext.Session.SetInt32("OTPRetryCount", retry);

                ModelState.AddModelError("", "Invalid OTP.");
                return View(verifyData);
            }

            HttpContext.Session.SetString("ResetPasswordFlow", "true");

            HttpContext.Session.Remove("ResetOTP");

            TempData["OTPVerfied"] = "OTP verified. Please create a new password.";

            return RedirectToAction("ResetPassword");
        }
        // ====================================================
        [HttpGet]
        [Route("Create-Password")]
        public IActionResult ResetPassword()
        {
            var timeout = HttpContext.Session.GetString("SessionTimeout");

            DateTime timeoutStr;
            bool success = DateTime.TryParse(timeout, out timeoutStr);

            if (HttpContext.Session.GetString("ResetPasswordFlow") != "true")
            {
                TempData["ErrorType"] = "Unauthorized access. Please start again.";
                return RedirectToAction("ForgotPassword");
            }
            if ( !success || DateTime.UtcNow > timeoutStr)
            {
                HttpContext.Session.Remove("ResetOTP");
                HttpContext.Session.Remove("ResetEmail");
                HttpContext.Session.Remove("OTPFlow");
                HttpContext.Session.Remove("ResetPasswordFlow");
                HttpContext.Session.Remove("OTPRetryCount");
                HttpContext.Session.Remove("UserType");
               
                TempData["ErrorType"] = "Session expired. Please start again.";
                return RedirectToAction("ForgotPassword");
            }
            var em = HttpContext.Session.GetString("ResetEmail");

            var model = new ResetPasswordModel
            {
                Email = em
            };
            return View(model);
        }
        [HttpPost]
        [Route("Create-Password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetData)
        {
            if (HttpContext.Session.GetString("ResetPasswordFlow") != "true")
            {
                TempData["ErrorType"] = "Unauthorized access. Please start again.";
                return RedirectToAction("ForgotPassword");
            }

            if (!ModelState.IsValid) return View(resetData);

            var email = HttpContext.Session.GetString("ResetEmail");
            var dbTable = HttpContext.Session.GetString("UserType");

            //if (email == null || string.IsNullOrEmpty(dbTable))
            //{
            //    TempData["ErrorType"] = "Session expired. Please start again.";
            //    return RedirectToAction("ForgotPassword");
            //}
            if (email == null)
            {
                TempData["ErrorType"] = "Email lost!. Session expired. Please start again.";
                return RedirectToAction("ForgotPassword");
            }
            if (string.IsNullOrEmpty(dbTable))
            {
                TempData["ErrorType"] = "UserType Missing. Session expired. Please start again.";
                return RedirectToAction("ForgotPassword");
            }

            if (dbTable == "Staff")
            {
                var staff = db.staff_table.FirstOrDefault(x => x.staff_email == email);

                var hasher = new PasswordHasher<string>();
                string hashedPassword = hasher.HashPassword(null, resetData.NewPassword);

                if (staff != null)
                {
                    staff.staff_password = hashedPassword;
                    staff.staff_failed_attempts = 0;
                    staff.staff_lock_until = null;

                    staff.otp_send_count = 0;
                    staff.otp_block_until = null;
                }
            }
            else if (dbTable == "Admin")
            {
                var admin = db.admin_table.FirstOrDefault(x => x.admin_email == email);

                var hasher = new PasswordHasher<string>();
                string hashedPassword = hasher.HashPassword(null, resetData.NewPassword);

                if (admin != null)
                {
                    admin.admin_password = hashedPassword;
                    admin.admin_failed_attempts = 0;
                    admin.admin_lock_until = null;

                    admin.otp_send_count = 0;
                    admin.otp_block_until = null;
                }
            }

            await db.SaveChangesAsync();

            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("OTPFlow");
            HttpContext.Session.Remove("ResetPasswordFlow");
            HttpContext.Session.Remove("OTPRetryCount");
            HttpContext.Session.Remove("UserType");

            TempData["SuccessMessage"] = "Password reset successful. Please log in with your new password.";

            // remove authentication cookie
            await HttpContext.SignOutAsync();

            // clear session data
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

    }
}  