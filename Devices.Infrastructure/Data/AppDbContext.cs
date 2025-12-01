using Microsoft.EntityFrameworkCore;
using Devices.Domain.Entities;

namespace Devices.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<Device> Devices { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Brand).IsRequired();
            b.Property(x => x.State).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
        });
    }
}
