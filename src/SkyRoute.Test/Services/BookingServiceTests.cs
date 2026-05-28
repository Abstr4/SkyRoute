using SkyRoute.API.Contracts.Requests;
using SkyRoute.API.Data;
using SkyRoute.API.DTOs;
using SkyRoute.API.Models;
using SkyRoute.API.Services;

namespace SkyRoute.Test.Services;

[Trait("Category", "Unit")]
public sealed class BookingServiceTests
{
    private readonly FlightOfferRepository _repository;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _repository = new FlightOfferRepository();
        _service = new BookingService(_repository);
    }

    [Fact]
    public void ConfirmBooking_UnknownFlightOffer_ThrowsArgumentException()
    {
        var request = new CreateBookingRequest(999, []);

        var ex = Assert.Throws<ArgumentException>(() => _service.ConfirmBooking(request));
        Assert.Contains("999", ex.Message);
    }

    [Fact]
    public void ConfirmBooking_DomesticRoute_NationalIdRequired_Succeeds()
    {
        var offer = CreateOffer(1, "EZE", "COR");
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });

        var booking = _service.ConfirmBooking(request);

        Assert.NotNull(booking);
        Assert.StartsWith("SKY-", booking.ReferenceCode);
        Assert.Single(booking.Passengers);
    }

    [Fact]
    public void ConfirmBooking_InternationalRoute_PassportRequired_Succeeds()
    {
        var offer = CreateOffer(1, "EZE", "GRU");
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("Jane Doe", "jane@test.com", DocumentType.Passport, "AB123456"),
        });

        var booking = _service.ConfirmBooking(request);

        Assert.NotNull(booking);
        Assert.StartsWith("SKY-", booking.ReferenceCode);
    }

    [Fact]
    public void ConfirmBooking_InternationalRouteWithNationalId_ThrowsInvalidOperation()
    {
        var offer = CreateOffer(1, "EZE", "GRU");
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("Jane Doe", "jane@test.com", DocumentType.NationalId, "12345678"),
        });

        var ex = Assert.Throws<InvalidOperationException>(() => _service.ConfirmBooking(request));
        Assert.Contains("Passport", ex.Message);
    }

    [Fact]
    public void ConfirmBooking_DomesticRouteWithPassport_ThrowsInvalidOperation()
    {
        var offer = CreateOffer(1, "EZE", "COR");
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.Passport, "AB123456"),
        });

        var ex = Assert.Throws<InvalidOperationException>(() => _service.ConfirmBooking(request));
        Assert.Contains("National ID", ex.Message);
    }

    [Fact]
    public void ConfirmBooking_ValidRequest_SetsCorrectPricing()
    {
        var offer = CreateOffer(1, "EZE", "COR", 150m);
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
            new("Jane Doe", "jane@test.com", DocumentType.NationalId, "87654321"),
        });

        var booking = _service.ConfirmBooking(request);

        Assert.Equal(150m, booking.PricePerPassenger);
        Assert.Equal(300m, booking.TotalPrice);
    }

    [Fact]
    public void ConfirmBooking_ValidRequest_SetsFlightSnapshotFields()
    {
        var offer = CreateOffer(1, "EZE", "COR", 100m);
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });

        var booking = _service.ConfirmBooking(request);

        Assert.Equal("BudgetWings", booking.ProviderName);
        Assert.Equal("BW101", booking.FlightNumber);
        Assert.Equal("EZE", booking.OriginAirportCode);
        Assert.Equal("COR", booking.DestinationAirportCode);
        Assert.Equal(CabinClass.Economy, booking.CabinClass);
    }

    [Theory]
    [InlineData("EZE", "COR", DocumentType.NationalId)]
    [InlineData("EZE", "GRU", DocumentType.Passport)]
    public void ConfirmBooking_ValidRouteAndDocument_Succeeds(string origin, string dest, DocumentType docType)
    {
        var offer = CreateOffer(1, origin, dest);
        _repository.StoreOffer(offer);
        var request = new CreateBookingRequest(1, new List<CreatePassengerRequest>
        {
            new("Test Passenger", "test@test.com", docType, "12345678"),
        });

        var booking = _service.ConfirmBooking(request);

        Assert.NotNull(booking);
        Assert.NotEmpty(booking.ReferenceCode);
    }

    private static FlightOffer CreateOffer(int id, string originCode, string destCode, decimal price = 100m)
    {
        var origin = MockDataStore.Airports.First(a => a.Code == originCode);
        var dest = MockDataStore.Airports.First(a => a.Code == destCode);

        return new FlightOffer
        {
            Id = id,
            FlightNumber = "BW101",
            Provider = "BudgetWings",
            OriginAirport = new AirportDto
            {
                Code = origin.Code,
                Name = origin.Name,
                City = origin.City,
                Country = origin.Country,
                CountryCode = origin.CountryCode,
            },
            DestinationAirport = new AirportDto
            {
                Code = dest.Code,
                Name = dest.Name,
                City = dest.City,
                Country = dest.Country,
                CountryCode = dest.CountryCode,
            },
            DepartureTime = DateTime.UtcNow.AddDays(1),
            ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(3),
            CabinClass = CabinClass.Economy,
            PricePerPassenger = price,
        };
    }
}
