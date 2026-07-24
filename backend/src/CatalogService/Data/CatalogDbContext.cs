using CatalogService.Models; using Microsoft.EntityFrameworkCore;
namespace CatalogService.Data;
public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{ 
    public DbSet<Product> Products => Set<Product>(); 
    protected override void OnModelCreating(ModelBuilder b) {
         b.Entity<Product>(e => {
             e.HasIndex(x => x.Slug).IsUnique(); 
            e.Property(x => x.Price).HasPrecision(18, 2); 
            e.Property(x => x.Name).HasMaxLength(160); }); 
            } 
}
