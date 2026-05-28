using System.Text.Json.Serialization;

namespace SkyRoute.API.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentType
{
    NationalId = 1,
    Passport = 2
}