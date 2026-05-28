using SkyRoute.API.DTOs;

namespace SkyRoute.API.Services;

/// <summary>
/// In-memory repository for flight offers.
/// In production, this would be replaced with a database or distributed cache.
/// </summary>
public sealed class FlightOfferRepository
{
    private readonly Dictionary<int, FlightOffer> _offers = new();

    /// <summary>
    /// Store a flight offer by ID for later booking validation.
    /// </summary>
    public void StoreOffer(FlightOffer offer)
    {
        _offers[offer.Id] = offer;
    }

    /// <summary>
    /// Store multiple flight offers.
    /// </summary>
    public void StoreOffers(IEnumerable<FlightOffer> offers)
    {
        foreach (var offer in offers)
        {
            StoreOffer(offer);
        }
    }

    /// <summary>
    /// Retrieve a flight offer by ID. Returns null if not found.
    /// </summary>
    public FlightOffer? GetOfferById(int offerId)
    {
        _offers.TryGetValue(offerId, out var offer);
        return offer;
    }

    /// <summary>
    /// Clear all stored offers (useful for testing or cache expiration).
    /// </summary>
    public void ClearOffers()
    {
        _offers.Clear();
    }
}
