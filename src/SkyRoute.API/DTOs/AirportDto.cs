namespace SkyRoute.API.DTOs;

public record AirportDto
{
    public required string Code { get; init; }

    public required string Name { get; init; }

    public required string City { get; init; }

    public required string Country { get; init; }

    public required string CountryCode { get; init; }

    // Example: "EZE - Ministro Pistarini Airport (Buenos Aires, Argentina)"
    public string DisplayName => $"{Code} - {Name} ({City}, {Country})";

    // Example: "Buenos Aires (EZE)"
    public string CitySelectorName => $"{City} ({Code})";
}