using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Data;

public sealed class BookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = [];
    private int _nextId = 1;

    public Task<Booking> AddAsync(Booking booking)
    {
        var stored = new Booking
        {
            Id = _nextId++,
            ReferenceCode = booking.ReferenceCode,
            CreatedAtUtc = booking.CreatedAtUtc,
            ProviderName = booking.ProviderName,
            FlightNumber = booking.FlightNumber,
            OriginAirportCode = booking.OriginAirportCode,
            DestinationAirportCode = booking.DestinationAirportCode,
            DepartureTime = booking.DepartureTime,
            ArrivalTime = booking.ArrivalTime,
            CabinClass = booking.CabinClass,
            Passengers = booking.Passengers,
            PricePerPassenger = booking.PricePerPassenger,
            TotalPrice = booking.TotalPrice,
        };
        _bookings.Add(stored);
        return Task.FromResult(stored);
    }

    public Task<IReadOnlyCollection<Booking>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyCollection<Booking>>(_bookings.ToList());
    }
}
