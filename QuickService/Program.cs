using QuickService.Models;
using Education.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Threading.RateLimiting;
using QuickService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. &  AutoValidateAntiforgeryTokenAttribute

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.Name = "QuickServiceSession";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// ADD THIS LINE HERE
builder.Services.AddDbContext<DatabaseCon>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ADD EMAIL SERVICE HERE
builder.Services.AddScoped<EmailServices>();

// PROFIL IMAGE SERVICE
builder.Services.AddScoped<QuickService.Services.IProfileImageService, QuickService.Services.ProfileImageService>();

builder.Services.Configure<SiteSettings>(
    builder.Configuration.GetSection("SiteSettings"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.Name = "QuickServiceAuth";
    options.Cookie.HttpOnly = true;

    options.Cookie.Path = "/";
    //options.Cookie.MaxAge = TimeSpan.FromMinutes(720);
    // HTTPS only
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // CSRF protection
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many login attempts. Please try again after 1 minute.");
    };

    options.AddPolicy("RateLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            ip => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 25,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) 
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    //context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; " + "script-src 'self'; " +
    //    "style-src 'self' https://cdnjs.cloudflare.com https://fonts.googleapis.com; " +
    //    "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
    //    "img-src 'self' data:;";

    await next();
});

app.UseRouting();

app.UseRateLimiter();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseUserSecurityCheck();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();