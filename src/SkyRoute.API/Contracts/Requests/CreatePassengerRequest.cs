using SkyRoute.API.Models;

namespace SkyRoute.API.Contracts.Requests;

public record CreatePassengerRequest(
    string FullName,
    string Email,
    DocumentType DocumentType,
    string DocumentNumber
);