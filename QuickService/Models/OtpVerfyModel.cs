using System.ComponentModel.DataAnnotations;

namespace QuickService.Models
{
    public class OtpVerifyModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits.")]
        public string OTP { get; set; } = string.Empty;
    }
}