using FluentValidation;
using Microsoft.Extensions.Logging;
using SkyRoute.Application.Common;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Services;

public sealed class BookingService : IBookingService
{
    private readonly List<Booking> _bookingsDatabase = new();
    private readonly IEnumerable<IFlightProvider> _providers;
    private readonly IValidator<CreateBookingRequest> _validator;
    private readonly ILogger<BookingService> _logger;
    private int _passengerId;

    public BookingService(
        IEnumerable<IFlightProvider> providers,
        IValidator<CreateBookingRequest> validator,
        ILogger<BookingService> logger)
    {
        _providers = providers;
        _validator = validator;
        _logger = logger;
    }

    public Result<Booking> ConfirmBooking(CreateBookingRequest request)
    {
        var validation = _validator.Validate(request);
        if (!validation.IsValid)
            return Result<Booking>.Failure(validation.Errors.Select(e => e.ErrorMessage));

        _logger.LogInformation(
            "Processing booking: {Provider} flight {FlightNumber}",
            request.Provider, request.FlightNumber);

        var provider = _providers.FirstOrDefault(p => p.ProviderName == request.Provider);

        if (provider is null)
        {
            _logger.LogWarning("Invalid provider: {Provider}", request.Provider);
            return Result<Booking>.Failure("Invalid Provider.");
        }

        var selectedFlight = provider.GetByFlightNumber(request.FlightNumber);
        if (selectedFlight is null)
        {
            _logger.LogWarning(
                "Flight not found: {Provider} / {FlightNumber}",
                request.Provider, request.FlightNumber);
            return Result<Booking>.Failure(
                $"Flight {request.FlightNumber} from {request.Provider} is no longer available.");
        }

        bool isInternational = !string.Equals(selectedFlight.OriginAirport.CountryCode, selectedFlight.DestinationAirport.CountryCode, StringComparison.OrdinalIgnoreCase);

        var domainPassengers = new List<Passenger>();

        foreach (var passenger in request.Passengers)
        {
            if (isInternational && passenger.DocumentType != DocumentType.Passport)
            {
                return Result<Booking>.Failure(
                    $"Passenger '{passenger.FullName}' must provide a Passport Number for international routes.");
            }

            if (!isInternational && passenger.DocumentType != DocumentType.NationalId)
            {
                return Result<Booking>.Failure(
                    $"Passenger '{passenger.FullName}' must provide a National ID for domestic routes.");
            }

            CreatePassenger(domainPassengers, passenger);
        }

        Booking flatBooking = CreateBooking(selectedFlight, domainPassengers);

        _bookingsDatabase.Add(flatBooking);

        _logger.LogInformation(
            "Booking confirmed: {ReferenceCode} for {Provider} flight {FlightNumber}",
            flatBooking.ReferenceCode, flatBooking.ProviderName, flatBooking.FlightNumber);

        return Result<Booking>.Success(flatBooking);
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
            CreatedAtUtc = DateTimeOffset.UtcNow,
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
