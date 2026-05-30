using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Contracts.Requests;

public record CreatePassengerRequest(
    string FullName,
    string Email,
    DocumentType DocumentType,
    string DocumentNumber
);
