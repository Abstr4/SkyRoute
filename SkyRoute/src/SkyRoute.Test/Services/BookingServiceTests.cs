using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using SkyRoute.Application.DTOs;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Services;
using SkyRoute.Domain.Models;

namespace SkyRoute.Test.Services;

[Trait("Category", "Unit")]
public sealed class BookingServiceTests
{
    private readonly Mock<IFlightProvider> _providerMock;
    private readonly Mock<IValidator<CreateBookingRequest>> _validatorMock;
    private readonly Mock<IBookingRepository> _repoMock;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _providerMock = new Mock<IFlightProvider>();
        _providerMock.Setup(p => p.ProviderName).Returns("BudgetWings");

        _validatorMock = new Mock<IValidator<CreateBookingRequest>>();
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateBookingRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        _repoMock = new Mock<IBookingRepository>();

        _service = new BookingService(
            _repoMock.Object,
            [_providerMock.Object],
            _validatorMock.Object,
            Mock.Of<ILogger<BookingService>>());
    }

    [Fact]
    public async Task ConfirmBooking_UnknownFlight_ReturnsFailure()
    {
        var request = new CreateBookingRequest("BudgetWings", "UNKNOWN", new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });

        var result = await _service.ConfirmBookingAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("UNKNOWN", result.Errors[0]);
    }

    [Fact]
    public async Task ConfirmBooking_DomesticRoute_NationalIdRequired_Succeeds()
    {
        var offer = CreateOffer("EZE", "COR");
        _providerMock.Setup(p => p.GetByFlightNumberAsync("BW101", It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });

        var result = await _service.ConfirmBookingAsync(request);

        Assert.True(result.IsSuccess);
        Assert.StartsWith("SKY-", result.Value!.ReferenceCode);
        Assert.Single(result.Value.Passengers);
    }

    [Fact]
    public async Task ConfirmBooking_InternationalRoute_PassportRequired_Succeeds()
    {
        var offer = CreateOffer("EZE", "GRU");
        _providerMock.Setup(p => p.GetByFlightNumberAsync("BW101", It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("Jane Doe", "jane@test.com", DocumentType.Passport, "AB123456"),
        });

        var result = await _service.ConfirmBookingAsync(request);

        Assert.True(result.IsSuccess);
        Assert.StartsWith("SKY-", result.Value!.ReferenceCode);
    }

    [Fact]
    public async Task ConfirmBooking_InternationalRouteWithNationalId_ReturnsFailure()
    {
        var offer = CreateOffer("EZE", "GRU");
        _providerMock.Setup(p => p.GetByFlightNumberAsync("BW101", It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("Jane Doe", "jane@test.com", DocumentType.NationalId, "12345678"),
        });

        var result = await _service.ConfirmBookingAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Passport", result.Errors[0]);
    }

    [Fact]
    public async Task ConfirmBooking_DomesticRouteWithPassport_ReturnsFailure()
    {
        var offer = CreateOffer("EZE", "COR");
        _providerMock.Setup(p => p.GetByFlightNumberAsync("BW101", It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.Passport, "AB123456"),
        });

        var result = await _service.ConfirmBookingAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("National ID", result.Errors[0]);
    }

    [Fact]
    public async Task ConfirmBooking_ValidRequest_SetsCorrectPricing()
    {
        var offer = CreateOffer("EZE", "COR", 150m);
        _providerMock.Setup(p => p.GetByFlightNumberAsync("BW101", It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
            new("Jane Doe", "jane@test.com", DocumentType.NationalId, "87654321"),
        });

        var result = await _service.ConfirmBookingAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(150m, result.Value!.PricePerPassenger);
        Assert.Equal(300m, result.Value.TotalPrice);
    }

    [Fact]
    public async Task ConfirmBooking_ValidRequest_SetsFlightSnapshotFields()
    {
        var offer = CreateOffer("EZE", "COR", 100m);
        _providerMock.Setup(p => p.GetByFlightNumberAsync("BW101", It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("John Doe", "john@test.com", DocumentType.NationalId, "12345678"),
        });

        var result = await _service.ConfirmBookingAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("BudgetWings", result.Value!.ProviderName);
        Assert.Equal("BW101", result.Value.FlightNumber);
        Assert.Equal("EZE", result.Value.OriginAirportCode);
        Assert.Equal("COR", result.Value.DestinationAirportCode);
        Assert.Equal(CabinClass.Economy, result.Value.CabinClass);
    }

    [Theory]
    [InlineData("EZE", "COR", DocumentType.NationalId)]
    [InlineData("EZE", "GRU", DocumentType.Passport)]
    public async Task ConfirmBooking_ValidRouteAndDocument_Succeeds(string origin, string dest, DocumentType docType)
    {
        var offer = CreateOffer(origin, dest);
        _providerMock.Setup(p => p.GetByFlightNumberAsync("BW101", It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        var request = new CreateBookingRequest("BudgetWings", "BW101", new List<CreatePassengerRequest>
        {
            new("Test Passenger", "test@test.com", docType, "12345678"),
        });

        var result = await _service.ConfirmBookingAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value!.ReferenceCode);
    }

    private static FlightOffer CreateOffer(string originCode, string destCode, decimal price = 100m)
    {
        var airports = new Dictionary<string, (string Name, string City, string Country, string Code)>
        {
            ["EZE"] = ("Ministro Pistarini International Airport", "Buenos Aires", "Argentina", "AR"),
            ["COR"] = ("Ingeniero Aeronáutico Ambrosio L.V. Taravella International Airport", "Córdoba", "Argentina", "AR"),
            ["MDZ"] = ("Governor Francisco Gabrielli International Airport", "Mendoza", "Argentina", "AR"),
            ["GRU"] = ("São Paulo/Guarulhos International Airport", "São Paulo", "Brazil", "BR"),
            ["GIG"] = ("Rio de Janeiro/Galeão International Airport", "Rio de Janeiro", "Brazil", "BR"),
            ["SCL"] = ("Arturo Merino Benítez International Airport", "Santiago", "Chile", "CL"),
        };

        var origin = airports[originCode];
        var dest = airports[destCode];

        return new FlightOffer
        {
            FlightNumber = "BW101",
            Provider = "BudgetWings",
            OriginAirport = new AirportDto
            {
                Code = originCode,
                Name = origin.Name,
                City = origin.City,
                Country = origin.Country,
                CountryCode = origin.Code,
            },
            DestinationAirport = new AirportDto
            {
                Code = destCode,
                Name = dest.Name,
                City = dest.City,
                Country = dest.Country,
                CountryCode = dest.Code,
            },
            DepartureTime = DateTimeOffset.UtcNow.AddDays(1),
            ArrivalTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(3),
            CabinClass = CabinClass.Economy,
            PricePerPassenger = price,
        };
    }
}
