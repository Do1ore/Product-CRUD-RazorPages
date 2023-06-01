using Microsoft.EntityFrameworkCore;

namespace ProjectStore.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
