using QuickService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace QuickService.Middleware
{
    public class UserSecurityMiddleware
    {
        private readonly RequestDelegate _next;

        public UserSecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DatabaseCon db)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                var roleClaim = context.User.FindFirst(ClaimTypes.Role);

                if (userIdClaim == null || roleClaim == null)
                {
                    await SignOutUser(context);
                    return;
                }

                int userId = int.Parse(userIdClaim.Value);
                string currentRole = roleClaim.Value;
                bool isValid = true;

                // TASK 3: SESSION + COOKIE SECURITY CHECK
                string? sessionAdminId = context.Session.GetString("AdminId");
                string? sessionStaffId = context.Session.GetString("StaffId");

                if (currentRole == "SuperAdmin" || currentRole == "Manager")
                {
                    if (string.IsNullOrEmpty(sessionAdminId) || sessionAdminId != userId.ToString())
                    {
                        isValid = false;
                    }
                    else
                    {
                        var admin = await db.admin_table.AsNoTracking().FirstOrDefaultAsync(a => a.admin_id == userId);
                        // Check if account is disabled or if role has changed
                        if (admin == null || !admin.admin_status || admin.admin_type != currentRole)
                        {
                            isValid = false;
                        }
                    }
                }
                else if (currentRole == "Staff")
                {
                    if (string.IsNullOrEmpty(sessionStaffId) || sessionStaffId != userId.ToString())
                    {
                        isValid = false;
                    }
                    else
                    {
                        var staff = await db.staff_table.AsNoTracking().FirstOrDefaultAsync(s => s.staff_id == userId);
                        if (staff == null || !staff.staff_status)
                        {
                            isValid = false;
                        }
                    }
                }

                if (!isValid)
                {
                    await SignOutUser(context);
                    return;
                }
            }

            await _next(context);
        }

        private async Task SignOutUser(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Session.Clear();
            
            // Avoid redirect loops if already on login page
            if (!context.Request.Path.StartsWithSegments("/Login") && !context.Request.Path.StartsWithSegments("/Auth"))
            {
                
            }
        }
    }

    public static class UserSecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserSecurityCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserSecurityMiddleware>();
        }
    }
}
