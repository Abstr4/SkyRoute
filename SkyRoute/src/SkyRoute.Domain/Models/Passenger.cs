namespace SkyRoute.Domain.Models;

public sealed class Passenger
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DocumentType DocumentType { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;
}
