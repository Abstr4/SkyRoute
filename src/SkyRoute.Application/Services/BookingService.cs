using SkyRoute.Application.Contracts.Requests;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Services;

public sealed class BookingService : IBookingService
{
    private readonly List<Booking> _bookingsDatabase = new();
    private readonly IEnumerable<IFlightProvider> _providers;
    private int _passengerId;

    public BookingService(IEnumerable<IFlightProvider> providers)
    {
        _providers = providers;
    }

    public Booking ConfirmBooking(CreateBookingRequest request)
    {
        var provider = _providers.FirstOrDefault(p => p.ProviderName == request.Provider);

        if (provider is null)
            throw new ArgumentException("Invalid Provider.");

        var selectedFlight = provider.GetByFlightNumber(request.FlightNumber)
            ?? throw new ArgumentException(
                $"Flight {request.FlightNumber} from {request.Provider} is no longer available.");

        bool isInternational = !string.Equals(selectedFlight.OriginAirport.Country, selectedFlight.DestinationAirport.Country, StringComparison.OrdinalIgnoreCase);

        var domainPassengers = new List<Passenger>();

        foreach (var passenger in request.Passengers)
        {
            if (isInternational && passenger.DocumentType != DocumentType.Passport)
            {
                throw new InvalidOperationException($"Passenger '{passenger.FullName}' must provide a Passport Number for international routes.");
            }

            if (!isInternational && passenger.DocumentType != DocumentType.NationalId)
            {
                throw new InvalidOperationException($"Passenger '{passenger.FullName}' must provide a National ID for domestic routes.");
            }

            CreatePassenger(domainPassengers, passenger);
        }

        Booking flatBooking = CreateBooking(selectedFlight, domainPassengers);

        _bookingsDatabase.Add(flatBooking);
        return flatBooking;
    }

    private void CreatePassenger(List<Passenger> domainPassengers, CreatePassengerRequest passenger)
    {
        domainPassengers.Add(new Passenger
        {
            Id = _passengerId++,
            FullName = passenger.FullName,
            Email = passenger.Email,
            DocumentType = passenger.DocumentType,
            DocumentNumber = passenger.DocumentNumber
        });
    }

    private Booking CreateBooking(FlightOffer selectedFlight, List<Passenger> domainPassengers)
    {
        var flatBooking = new Booking
        {
            Id = _bookingsDatabase.Count + 1,
            ReferenceCode = $"SKY-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CreatedAtUtc = DateTime.UtcNow,
            Passengers = domainPassengers,

            ProviderName = selectedFlight.Provider,
            FlightNumber = selectedFlight.FlightNumber,
            OriginAirportCode = selectedFlight.OriginAirport.Code,
            DestinationAirportCode = selectedFlight.DestinationAirport.Code,
            DepartureTime = selectedFlight.DepartureTime,
            ArrivalTime = selectedFlight.ArrivalTime,
            CabinClass = selectedFlight.CabinClass,

            PricePerPassenger = selectedFlight.PricePerPassenger,
            TotalPrice = selectedFlight.PricePerPassenger * domainPassengers.Count
        };
        return flatBooking;
    }
}
