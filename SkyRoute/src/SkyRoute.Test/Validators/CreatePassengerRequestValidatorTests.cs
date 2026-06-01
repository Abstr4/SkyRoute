using FluentValidation.TestHelper;
using SkyRoute.Application.Features.Booking;
using SkyRoute.Domain.Models;

namespace SkyRoute.Test.Validators;

[Trait("Category", "Unit")]
public sealed class CreatePassengerRequestValidatorTests
{
    private readonly CreatePassengerRequestValidator _validator = new();

    [Fact]
    public void FullName_Empty_ReturnsError()
    {
        var request = new CreatePassengerRequest("", "john@test.com", DocumentType.Passport, "AB123456");
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Email_Empty_ReturnsError()
    {
        var request = new CreatePassengerRequest("John Doe", "", DocumentType.Passport, "AB123456");
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Email_InvalidFormat_ReturnsError()
    {
        var request = new CreatePassengerRequest("John Doe", "not-an-email", DocumentType.Passport, "AB123456");
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void DocumentNumber_Empty_ReturnsError()
    {
        var request = new CreatePassengerRequest("John Doe", "john@test.com", DocumentType.Passport, "");
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
    }

    [Fact]
    public void ValidPassenger_PassesValidation()
    {
        var request = new CreatePassengerRequest("John Doe", "john@test.com", DocumentType.Passport, "AB123456");
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
