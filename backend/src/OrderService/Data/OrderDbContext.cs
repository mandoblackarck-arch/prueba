using Microsoft.EntityFrameworkCore; using OrderService.Models;
namespace OrderService.Data;
public sealed class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options) { public DbSet<Order> Orders => Set<Order>(); protected override void OnModelCreating(ModelBuilder b) { b.Entity<Order>(x => { x.HasIndex(y => y.Number).IsUnique(); x.Property(y => y.Total).HasPrecision(18,2); x.HasMany(y => y.Lines).WithOne(y => y.Order!).HasForeignKey(y => y.OrderId); }); b.Entity<OrderLine>().Property(x => x.UnitPrice).HasPrecision(18,2); } }
