namespace SkyRoute.Application.Features.Booking;

public record CreateBookingRequest(
    string Provider,
    string FlightNumber,
    List<CreatePassengerRequest> Passengers
);
