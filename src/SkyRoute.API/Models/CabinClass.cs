using System.Text.Json.Serialization;

namespace SkyRoute.API.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CabinClass
{
    Economy = 1,
    Business = 2,
    FirstClass = 3
}