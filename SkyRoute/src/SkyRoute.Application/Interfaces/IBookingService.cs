using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces;

public interface IBookingService
{
    Booking ConfirmBooking(CreateBookingRequest request);
}
