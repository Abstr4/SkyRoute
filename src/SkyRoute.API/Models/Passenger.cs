namespace SkyRoute.API.Models;

public sealed class Passenger
{
    public required int Id { get; set; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required DocumentType DocumentType { get; init; }

    public required string DocumentNumber { get; init; }
}