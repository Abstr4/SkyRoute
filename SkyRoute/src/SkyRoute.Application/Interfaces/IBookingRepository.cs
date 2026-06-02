using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking> AddAsync(Booking booking);
    Task<IReadOnlyCollection<Booking>> GetAllAsync();
}
