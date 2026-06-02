using SkyRoute.Application.Common;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces;

public interface IBookingService
{
    Task<Result<Booking>> ConfirmBooking(CreateBookingRequest request);
}
