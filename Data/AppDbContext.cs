using Microsoft.EntityFrameworkCore;
using VirtualStore.Models;

namespace VirtualStore.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Store> Stores { get; set; }
        public DbSet<Space> Spaces { get; set; }
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>()
                .HasMany(s => s.Spaces)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Space>()
                .HasMany(s => s.Products)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
