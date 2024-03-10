using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using CoordinateSharp;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Utils;

public class SunriseSunset(SunriseSunsetSettings settings, HttpClientFactory httpClientFactory)
{
    public async Task<(Time?, Time?)> GetSunriseSunsetAsync(CancellationToken token = default)
    {
        var (sunrise, sunset) = (settings.Store.Sunrise, settings.Store.Sunset);
        if (settings.Store.LastCheckDateTime == DateTime.Today && sunrise is not null && sunset is not null)
            return (sunrise, sunset);

        var coordinate = await GetGeoLocationAsync(token).ConfigureAwait(false);

        if (coordinate is null)
            return (null, null);

        (sunrise, sunset) = CalculateSunriseSunset(coordinate);

        settings.Store.LastCheckDateTime = DateTime.UtcNow;
        settings.Store.Sunrise = sunrise;
        settings.Store.Sunset = sunset;
        settings.SynchronizeStore();

        return (sunrise, sunset);
    }

    private async Task<Coordinate?> GetGeoLocationAsync(CancellationToken token)
    {
        try
        {
            using var httpClient = httpClientFactory.Create();
            var responseJson = await httpClient.GetStringAsync("http://ip-api.com/json?fields=lat,lon", token).ConfigureAwait(false);
            var responseJsonNode = JsonNode.Parse(responseJson);
            if (responseJsonNode is not null && double.TryParse(responseJsonNode["lat"]?.ToString(), out var lat) && double.TryParse(responseJsonNode["lon"]?.ToString(), out var lon))
                return new Coordinate(lat, lon, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get geolocation.", ex);
        }

        return null;
    }

    private static (Time?, Time?) CalculateSunriseSunset(Coordinate coordinate)
    {
        var sunrise = coordinate.CelestialInfo.SunRise;
        var sunset = coordinate.CelestialInfo.SunSet;

        if (sunrise is null || sunset is null)
            return (null, null);

        return (new Time(sunrise.Value.Hour, sunrise.Value.Minute), new Time(sunset.Value.Hour, sunset.Value.Minute));
    }
}
