using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Models;

namespace SalesAssessment.Api.Data
{
    public class SalesAssessmentDbContext : DbContext
    {
        public SalesAssessmentDbContext(DbContextOptions<SalesAssessmentDbContext> options)
        : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.HasMany(e => e.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(100);
                entity.Property(e => e.OrderDate).IsRequired();
            });
        }
    }
}
