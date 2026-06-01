using SkyRoute.Application.Common;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces;

public interface IBookingService
{
    Result<Booking> ConfirmBooking(CreateBookingRequest request);
}
