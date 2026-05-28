namespace SkyRoute.API.DTOs;

public record PassengerDetailsDto
{
    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required string DocumentNumber { get; init; } // Will hold either Passport or National ID
}