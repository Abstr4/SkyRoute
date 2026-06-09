using Microsoft.EntityFrameworkCore;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Data;

public sealed class SkyRouteDbContext : DbContext
{
    public SkyRouteDbContext(DbContextOptions<SkyRouteDbContext> options) : base(options) { }

    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Passenger> Passengers => Set<Passenger>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasOne(f => f.OriginAirport)
                .WithMany()
                .HasForeignKey(f => f.OriginAirportId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(f => f.DestinationAirport)
                .WithMany()
                .HasForeignKey(f => f.DestinationAirportId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasMany(b => b.Passengers)
                .WithOne()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
