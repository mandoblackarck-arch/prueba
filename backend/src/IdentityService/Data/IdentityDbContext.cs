using IdentityService.Models;
using Microsoft.EntityFrameworkCore;
namespace IdentityService.Data;
public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AppUser>(e => { e.HasIndex(x => x.Email).IsUnique(); e.Property(x => x.Email).HasMaxLength(256); e.Property(x => x.Name).HasMaxLength(120); });
    }
}
