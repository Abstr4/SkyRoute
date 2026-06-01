using FluentValidation.TestHelper;
using SkyRoute.Application.Features.Flights;
using SkyRoute.Domain.Models;

namespace SkyRoute.Test.Validators;

[Trait("Category", "Unit")]
public sealed class FlightSearchRequestValidatorTests
{
    private readonly FlightSearchRequestValidator _validator = new();

    [Fact]
    public void OriginAirportCode_Empty_ReturnsError()
    {
        var request = ValidRequest() with { OriginAirportCode = "" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.OriginAirportCode);
    }

    [Fact]
    public void OriginAirportCode_WrongLength_ReturnsError()
    {
        var request = ValidRequest() with { OriginAirportCode = "EZZZ" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.OriginAirportCode);
    }

    [Fact]
    public void OriginAirportCode_Lowercase_ReturnsError()
    {
        var request = ValidRequest() with { OriginAirportCode = "eze" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.OriginAirportCode);
    }

    [Fact]
    public void DestinationAirportCode_Empty_ReturnsError()
    {
        var request = ValidRequest() with { DestinationAirportCode = "" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DestinationAirportCode);
    }

    [Fact]
    public void SameOriginAndDestination_ReturnsError()
    {
        var request = ValidRequest() with { DestinationAirportCode = "EZE" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Passengers_Zero_ReturnsError()
    {
        var request = ValidRequest() with { Passengers = 0 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Passengers);
    }

    [Fact]
    public void CabinClass_Invalid_ReturnsError()
    {
        var request = ValidRequest() with { CabinClass = (CabinClass)99 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CabinClass);
    }

    [Fact]
    public void TimeZone_Invalid_ReturnsError()
    {
        var request = ValidRequest() with { TimeZone = "NotARealTimeZone" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.TimeZone);
    }

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = ValidRequest();
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static FlightSearchRequest ValidRequest()
    {
        return new FlightSearchRequest(
            "EZE", "GRU",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1, CabinClass.Economy, "UTC");
    }
}
