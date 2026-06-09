using Microsoft.EntityFrameworkCore;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Models;
using SkyRoute.Infrastructure.Data;

namespace SkyRoute.Infrastructure.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private readonly SkyRouteDbContext _dbContext;

    public BookingRepository(SkyRouteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
