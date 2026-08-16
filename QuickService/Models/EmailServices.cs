using QuickService.Controllers;
using QuickService.Helpers;
using QuickService.Models;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Education.Models
{
    public class EmailServices
    {
        private readonly SiteSettings AppConfig;
        private readonly ILogger<EmailServices> _logger;
        public EmailServices(IOptions<SiteSettings> config, ILogger<EmailServices> logger)
        {
            AppConfig = config.Value;
            _logger = logger;
        }

        public string Msg { get; set; } = string.Empty;
        public int Otp { get; set; }
        public bool IsEmailSent { get; set; }

        public async Task<(bool IsEmailSent, string Msg, int Otp)> Email(string userEmail)
        {
            int otp = RandomNumberGenerator.GetInt32(100000, 999999);

            try
            {
                //var client = new SmtpClient("smtp.gmail.com", 587)
                var client = new SmtpClient("smtp.hostinger.com", 587)
                {
                    Credentials = new NetworkCredential(
                        SiteData.CompanyEmail,
                        AppConfig.EmailAppPassword
                    ),
                    EnableSsl = true,
                    Timeout = 10000 // 10 seconds
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(SiteData.CompanyEmail, "QuickService Support"),
                    Subject = "QuickService - Verification Code",
                    IsBodyHtml = true
                };

                mail.To.Add(userEmail);

                mail.Body = $@"
                    <html>
                    <body style='font-family:Arial,sans-serif;background:#f4f6f8;padding:20px;'>

                    <div style='max-width:600px;margin:auto;background:white;padding:30px;border-radius:8px;border:1px solid #e5e7eb;'>

                    <h2 style='color:#1f2937;'>QuickService Account Verification</h2>

                    <p>Hello,</p>

                    <p>
                    We received a request to verify your identity. 
                    Please use the verification code below.
                    </p>

                    <div style='text-align:center;margin:30px 0;'>
                    <span style='font-size:30px;font-weight:bold;letter-spacing:6px;
                    background:#f3f4f6;padding:12px 24px;border-radius:6px;color:#111827;'>
                    {otp}
                    </span>
                    </div>

                    <p>This code will expire in <b>10 minutes</b>.</p>

                    <p style='font-size:13px;color:#6b7280;'>
                    If you did not request this code, please ignore this email.
                    </p>

                    <hr>

                    <p style='font-size:12px;color:#9ca3af;text-align:center;'>
                    © {DateTime.UtcNow.Year} QuickService
                    </p>

                    </div>

                    </body>
                    </html>";

                client.Send(mail);

                return (true, $"OTP sent to your email - {userEmail}", otp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email.");
                return (false, "Failed to send OTP on email", 0);
            }
        }
    }
}