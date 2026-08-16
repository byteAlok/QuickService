using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickService.Models
{
    [Table("admin_table")]
    public class AdminModel
    {
        [Key]
        public int admin_id { get; set; }

        [Required]
        [StringLength(150)]
        public string admin_name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string admin_email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter valid 10 digit phone")]
        public string admin_phone { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password must contain letters and numbers.")]
        public string admin_password { get; set; } = string.Empty;

        [StringLength(350)]
        public string? admin_address { get; set; } 

        [StringLength(100)]
        public string? admin_city_state { get; set; }

        [StringLength(300)]
        public string? admin_image { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime admin_register_date { get; set; }

        public DateTime? admin_last_login { get; set; }

        public bool admin_status { get; set; } = true;

        [Required]
        [StringLength(50)]
        public string admin_type { get; set; } = string.Empty; 
        public int? admin_failed_attempts { get; set; } = 0;
        public DateTime? admin_lock_until { get; set; }
        public int? otp_send_count { get; set; } = 0;
        public DateTime? otp_block_until { get; set; }
    }
}