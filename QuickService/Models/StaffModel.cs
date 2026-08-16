using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickService.Models
{
    [Table("staff_table")]
    public class StaffModel
    {
        [Key]
        public int staff_id { get; set; }

        [Required]
        [StringLength(150)]
        public string staff_name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string staff_email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter valid 10 digit phone")]
        public string staff_phone { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password must contain letters and numbers.")]
        public string staff_password { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string staff_skill { get; set; } = string.Empty;

        [StringLength(350)]
        public string? staff_address { get; set; }

        [StringLength(100)]
        public string? staff_city_state { get; set; }

        [StringLength(300)]
        public string? staff_image { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime staff_register_date { get; set; }

        public DateTime? staff_last_login { get; set; }

        public bool staff_status { get; set; } = true;

        public int? staff_failed_attempts { get; set; } = 0;
        public DateTime? staff_lock_until { get; set; }
        public int? otp_send_count { get; set; } = 0;
        public DateTime? otp_block_until { get; set; }
    }
}