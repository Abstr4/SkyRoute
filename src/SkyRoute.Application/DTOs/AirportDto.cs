namespace SkyRoute.Application.DTOs;

public record AirportDto
{
    public required string Code { get; init; }

    public required string Name { get; init; }

    public required string City { get; init; }

    public required string Country { get; init; }

    public required string CountryCode { get; init; }

    public string DisplayName => $"{Code} - {Name} ({City}, {Country})";

    public string CitySelectorName => $"{City} ({Code})";
}
