using Microsoft.EntityFrameworkCore;

namespace QuickService.Models
{
    public class DatabaseCon : DbContext
    {
        public DatabaseCon(DbContextOptions<DatabaseCon> options) : base(options) { }

        // Tables
        public DbSet<BookingModel> booking_table { get; set; }
        public DbSet<AdminModel> admin_table { get; set; }
        public DbSet<StaffModel> staff_table { get; set; }

    }
}
