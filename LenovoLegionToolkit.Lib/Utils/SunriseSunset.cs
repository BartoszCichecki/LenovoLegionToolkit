using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using CoordinateSharp;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Utils;

public class SunriseSunset
{
    private readonly SunriseSunsetSettings _settings;

    public SunriseSunset(SunriseSunsetSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<(Time?, Time?)> GetSunriseSunsetAsync(CancellationToken token = default)
    {
        var (sunrise, sunset) = (_settings.Store.Sunrise, _settings.Store.Sunset);
        if (_settings.Store.LastCheckDateTime == DateTime.Today && sunrise is not null && sunset is not null)
            return (sunrise, sunset);

        var coordinate = await GetGeoLocationAsync(token).ConfigureAwait(false);

        if (coordinate is null)
            return (null, null);

        (sunrise, sunset) = CalculateSunriseSunset(coordinate);

        _settings.Store.LastCheckDateTime = DateTime.UtcNow;
        _settings.Store.Sunrise = sunrise;
        _settings.Store.Sunset = sunset;
        _settings.SynchronizeStore();

        return (sunrise, sunset);
    }

    private async Task<Coordinate?> GetGeoLocationAsync(CancellationToken token)
    {
        try
        {
            var httpClient = new HttpClient();
            var responseJson = await httpClient.GetStringAsync("http://ip-api.com/json?fields=lat,lon", token).ConfigureAwait(false);
            var responseJsonNode = JsonNode.Parse(responseJson);
            if (double.TryParse(responseJsonNode?["lat"]?.ToString(), out double lat) && double.TryParse(responseJsonNode?["lon"]?.ToString(), out double lon))
                return new Coordinate(lat, lon, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get geolocation.", ex);
        }

        return null;
    }

    private (Time?, Time?) CalculateSunriseSunset(Coordinate coordinate)
    {
        var sunrise = coordinate.CelestialInfo.SunRise;
        var sunset = coordinate.CelestialInfo.SunSet;

        if (sunrise == null || sunset == null)
            return (null, null);

        return (new Time { Hour = sunrise.Value.Hour, Minute = sunrise.Value.Minute }, new Time { Hour = sunset.Value.Hour, Minute = sunset.Value.Minute });
    }
}