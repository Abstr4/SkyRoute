using Microsoft.EntityFrameworkCore;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Data;

public sealed class SkyRouteDbContext : DbContext
{
    public DbSet<Flight> Flights => Set<Flight>();

    public SkyRouteDbContext(DbContextOptions<SkyRouteDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(f => new { f.Provider, f.Id });
            entity.OwnsOne(f => f.OriginAirport);
            entity.OwnsOne(f => f.DestinationAirport);
            entity.Property(f => f.CabinClass).HasConversion<int>();
        });
    }
}
