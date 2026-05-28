namespace SkyRoute.API.Contracts.Requests;

public record CreateBookingRequest(
    int FlightOfferId,
    List<CreatePassengerRequest> Passengers
);