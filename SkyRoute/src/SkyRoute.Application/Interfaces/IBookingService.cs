using SkyRoute.Application.Features.Booking;
using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces;

public interface IBookingService
{
    Booking ConfirmBooking(CreateBookingRequest request);
}
