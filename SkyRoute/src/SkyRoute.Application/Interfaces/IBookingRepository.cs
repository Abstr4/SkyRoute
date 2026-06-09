using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
}
