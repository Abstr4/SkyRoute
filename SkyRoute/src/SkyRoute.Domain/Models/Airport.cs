namespace SkyRoute.Domain.Models;

public sealed class Airport
{
    public required int Id { get; set; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public required string City { get; init; }

    public required string Country { get; init; }

    public required string CountryCode { get; init; }
}
