using System.Data.Entity; // Import Entity Framework

namespace WebApplication1.Models // Ensure correct namespace
{
    public class ApplicationDbContext : DbContext
    {
        // Define a table for addpatient
        public DbSet<AddPatient> AddPatients { get; set; }

        // Constructor that uses a connection string from Web.config
        public ApplicationDbContext() : base("DefaultConnection") { }
    }
}
