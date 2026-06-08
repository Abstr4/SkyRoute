using FluentValidation;

namespace SkyRoute.Application.Features.Flights;

public sealed class FlightSearchRequestValidator : AbstractValidator<FlightSearchRequest>
{
    public FlightSearchRequestValidator()
    {
        RuleFor(x => x.OriginAirportCode)
            .NotEmpty()
            .Length(3)
            .Must(BeValidAirportCode)
                .WithMessage("Origin airport code must be a 3-letter uppercase IATA code.");

        RuleFor(x => x.DestinationAirportCode)
            .NotEmpty()
            .Length(3)
            .Must(BeValidAirportCode)
                .WithMessage("Destination airport code must be a 3-letter uppercase IATA code.");

        RuleFor(x => x)
            .Must(BeDifferentAirports)
                .WithMessage("Origin and destination airports cannot be the same.");

        RuleFor(x => x.DepartureDate)
            .NotEmpty()
            .Must(BeInFuture)
                .WithMessage("Departure date cannot be in the past.");

        RuleFor(x => x.Passengers)
            .GreaterThan(0)
                .WithMessage("At least one passenger is required.");

        RuleFor(x => x.CabinClass)
            .IsInEnum()
                .WithMessage("Invalid cabin class.");

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .Must(BeValidTimeZone)
                .WithMessage("Invalid timezone.");
    }

    private static bool BeValidAirportCode(string code) =>
        code.All(char.IsAsciiLetterUpper);

    private static bool BeDifferentAirports(FlightSearchRequest request) =>
        !string.Equals(request.OriginAirportCode, request.DestinationAirportCode, StringComparison.OrdinalIgnoreCase);

    private static bool BeInFuture(FlightSearchRequest request, DateOnly date)
    {
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            var localEnd = date.ToDateTime(TimeOnly.MaxValue);
            var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, tz);
            return utcEnd > DateTimeOffset.UtcNow;
        }
        catch (TimeZoneNotFoundException)
        {
            return true;
        }
    }

    private static bool BeValidTimeZone(string tz)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(tz);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
