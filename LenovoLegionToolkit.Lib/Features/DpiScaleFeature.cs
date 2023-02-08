using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using WindowsDisplayAPI.Native.DisplayConfig;

namespace LenovoLegionToolkit.Lib.Features;

public class DpiScaleFeature : IFeature<DpiScale>
{
    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task<DpiScale[]> GetAllStatesAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting all DPI scales...");

        var display = InternalDisplay.Get();
        var pds = display?.ToPathDisplaySource();
        if (pds is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return Task.FromResult(Array.Empty<DpiScale>());
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

        return Task.FromResult(result);
    }

    public Task<DpiScale> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting current DPI scale...");

        var display = InternalDisplay.Get();
        var pds = display?.ToPathDisplaySource();
        if (pds is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return Task.FromResult(default(DpiScale));
        }

        var result = (int)pds.CurrentDPIScale;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current DPI scale is {result}");

        return Task.FromResult(new DpiScale(result));
    }

    public Task SetStateAsync(DpiScale state)
    {
        var display = InternalDisplay.Get();
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
            return Task.CompletedTask;
        }

        if (!Enum.IsDefined(typeof(DisplayConfigSourceDPIScale), (uint)state.Scale))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"DPI scale {state.Scale} not found");
            return Task.CompletedTask;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting DPI scale to {state.Scale}");

        pds.CurrentDPIScale = (DisplayConfigSourceDPIScale)state.Scale;
        return Task.CompletedTask;
    }
}