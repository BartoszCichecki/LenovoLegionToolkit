using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using WindowsDisplayAPI.Native.DisplayConfig;

namespace LenovoLegionToolkit.Lib.Features;

public class DpiScaleFeature : IFeature<DpiScale>
{
    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public async Task<DpiScale[]> GetAllStatesAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting all DPI scales...");

        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        var pds = display?.ToPathDisplaySource();
        if (pds is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return Array.Empty<DpiScale>();
        }

        var max = (int)pds.MaximumDPIScale;

        var result = Enum.GetValues<DisplayConfigSourceDPIScale>()
            .Select(s => (int)s)
            .Where(s => s <= max)
            .OrderBy(s => s)
            .Select(s => new DpiScale(s))
            .ToArray();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current DPI scale is {result}");

        return result;
    }

    public async Task<DpiScale> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting current DPI scale...");

        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        var pds = display?.ToPathDisplaySource();
        if (pds is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return default;
        }

        var result = (int)pds.CurrentDPIScale;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current DPI scale is {result}");

        return new DpiScale(result);
    }

    public async Task SetStateAsync(DpiScale state)
    {
        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        var pds = display?.ToPathDisplaySource();
        if (pds is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");
            throw new InvalidOperationException("Built in display not found");
        }

        if ((int)pds.CurrentDPIScale == state.Scale)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"DPI scale already set to {state.Scale}");
            return;
        }

        if (!Enum.IsDefined(typeof(DisplayConfigSourceDPIScale), (uint)state.Scale))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"DPI scale {state.Scale} not found");
            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting DPI scale to {state.Scale}");

        pds.CurrentDPIScale = (DisplayConfigSourceDPIScale)state.Scale;
    }
}