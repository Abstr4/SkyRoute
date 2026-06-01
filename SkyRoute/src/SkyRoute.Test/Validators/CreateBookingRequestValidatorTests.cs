using FluentValidation.TestHelper;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Domain.Models;

namespace SkyRoute.Test.Validators;

[Trait("Category", "Unit")]
public sealed class CreateBookingRequestValidatorTests
{
    private readonly CreateBookingRequestValidator _validator = new();

    [Fact]
    public void Provider_Empty_ReturnsError()
    {
        var request = new CreateBookingRequest("", "BW101", [ValidPassenger()]);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Provider);
    }

    [Fact]
    public void FlightNumber_Empty_ReturnsError()
    {
        var request = new CreateBookingRequest("BudgetWings", "", [ValidPassenger()]);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FlightNumber);
    }

    [Fact]
    public void Passengers_Empty_ReturnsError()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101", []);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Passengers);
    }

    [Fact]
    public void Passenger_Invalid_ReturnsChildError()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101",
        [
            new CreatePassengerRequest("", "invalid", DocumentType.Passport, ""),
        ]);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Passengers[0].FullName");
        result.ShouldHaveValidationErrorFor("Passengers[0].Email");
        result.ShouldHaveValidationErrorFor("Passengers[0].DocumentNumber");
    }

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new CreateBookingRequest("BudgetWings", "BW101",
        [
            ValidPassenger(),
        ]);
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreatePassengerRequest ValidPassenger()
    {
        return new CreatePassengerRequest("John Doe", "john@test.com", DocumentType.Passport, "AB123456");
    }
}
