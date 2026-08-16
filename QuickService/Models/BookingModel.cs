using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace QuickService.Models
{
    [Table("booking_table")]
    public class BookingModel
    {
        [Key]
        public int id { get; set; }

        // ---------- Product Details ----------

        [Required(ErrorMessage = "Product category is required")]
        [StringLength(75)]
        public string product_category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sub category is required")]
        [StringLength(75)]
        public string sub_category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Warranty status is required")]
        [StringLength(50)]
        public string warranty_status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(75)]
        public string brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Issue is required")]
        [StringLength(100)]
        public string issue_type { get; set; } = string.Empty;

        [StringLength(500)]
        public string? issue_description { get; set; }

        // ---------- User Details ----------

        [Required(ErrorMessage = "User name is required")]
        [StringLength(150)]
        public string full_name { get; set; } = string.Empty;

        [Required]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15)]
        public string phone_number { get; set; } = string.Empty;

        [Phone]
        [StringLength(15)]
        public string? alt_phone_number { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150)]
        public string? email { get; set; }

        [Required]
        [StringLength(350)]
        public string address { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string city { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid pincode")]
        [StringLength(10)]
        public string pin_code { get; set; } = string.Empty;

        // ---------- Booking Preferences ----------

        [StringLength(30)]
        public string? priority { get; set; }

        [Required]
        [StringLength(50)]
        public string preferred_day { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string time_slot { get; set; } = string.Empty;

        public string? custom_date { get; set; }
 

        // ---------- System Fields ----------

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime BookingDate { get; set; }

        public string? service_status { get; set; }
        public string? BookingBy { get; set; }
        public int? foreign_key_id { get; set; }

    }
} 