using System.Data.Entity;
namespace WebApplication1.Models // Ensure correct namespace
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection") { }

        public DbSet<UserModel> UserModels { get; set; }
    }
}