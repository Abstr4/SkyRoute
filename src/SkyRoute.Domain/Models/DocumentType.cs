using System.Text.Json.Serialization;

namespace SkyRoute.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentType
{
    NationalId = 1,
    Passport = 2
}
