using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Order_Flow.Domain.Entities;
using Order_Flow.Infrastructure.ReadModels;

namespace Order_Flow.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order_Item> OrderItems { get; set; }
        public DbSet<OrderDashboardReadModel> OrderDashboardReadModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<Customer>(entity =>
           { 
               entity.HasKey(e => e.Id);
               entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
               entity.HasData(new { Id = 1, Name = "Ibrahim" }, new { Id = 2, Name = "Ahmed" },new {Id = 3, Name = "Atef"},new {Id = 4, Name = "Osama"});
               
           });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerId);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.HasMany(e => e.Items).WithOne().HasForeignKey(oi => oi.OrderId).OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<Order_Item>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(18,2);
            });
            modelBuilder.Entity<OrderDashboardReadModel>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Total).IsRequired().HasPrecision(18,2);
                entity.Property(e => e.LastUpdateAt).IsRequired();
            });

        }
    }
}
