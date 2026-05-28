using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Data;
using SkyRoute.API.DTOs;
using SkyRoute.API.Models;

namespace SkyRoute.API.Services;

public sealed class BookingService
{
    private readonly List<Booking> _bookingsDatabase = new();
    private readonly FlightOfferRepository _offerRepository;
    private int _passengerId;

    public BookingService(FlightOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public Booking ConfirmBooking(CreateBookingRequest request)
    {
        // Lookup the flight offer server-side using the provided ID
        var selectedFlight = _offerRepository.GetOfferById(request.FlightOfferId);

        if (selectedFlight == null)
        {
            throw new ArgumentException($"Flight offer with ID {request.FlightOfferId} not found. Please search for flights again.");
        }

        // Validate airports from the authoritative flight offer
        var originAirport = MockDataStore.Airports.FirstOrDefault(a => a.Code == selectedFlight.OriginAirport.Code);
        var destinationAirport = MockDataStore.Airports.FirstOrDefault(a => a.Code == selectedFlight.DestinationAirport.Code);

        if (originAirport == null || destinationAirport == null)
        {
            throw new ArgumentException("Invalid origin or destination airport records.");
        }

        bool isInternational = !string.Equals(originAirport.Country, destinationAirport.Country, StringComparison.OrdinalIgnoreCase);

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

        // Create booking using the server-fetched flight offer data
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

            // Lock down the final calculated values using server-side authoritative pricing
            PricePerPassenger = selectedFlight.PricePerPassenger,
            TotalPrice = selectedFlight.PricePerPassenger * domainPassengers.Count
        };
        return flatBooking;
    }
}