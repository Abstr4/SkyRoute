namespace SkyRoute.Application.Contracts.Requests;

public record CreateBookingRequest(
    string Provider,
    string FlightNumber,
    List<CreatePassengerRequest> Passengers
);
