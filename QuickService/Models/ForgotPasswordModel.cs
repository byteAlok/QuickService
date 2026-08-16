using System.ComponentModel.DataAnnotations;

namespace QuickService.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selection is Required")]
        public string User { get; set; } = string.Empty;
    }
}