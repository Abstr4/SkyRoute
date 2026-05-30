using System.Text.Json.Serialization;

namespace SkyRoute.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CabinClass
{
    Economy = 1,
    Business = 2,
    FirstClass = 3
}
