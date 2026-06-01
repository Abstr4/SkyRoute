using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Features.Booking;

public record CreatePassengerRequest(
    string FullName,
    string Email,
    DocumentType DocumentType,
    string DocumentNumber
);
