namespace SkyRoute.Application.DTOs;

public record PassengerDetailsDto
{
    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required string DocumentNumber { get; init; }
}
