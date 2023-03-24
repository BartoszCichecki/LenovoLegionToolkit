using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public class HDRFeature : IFeature<HDRState>
{
    public Task<bool> IsSupportedAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Checking HDR support...");

        var display = InternalDisplay.Get();
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return Task.FromResult(false);
        }

        var isSupported = display.GetAdvancedColorInfo().AdvancedColorSupported;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"HDR support: {isSupported}");

        return Task.FromResult(isSupported);
    }

    public Task<bool> IsHdrBlockedAsync()
    {
        var display = InternalDisplay.Get() ?? throw new InvalidOperationException("Built in display not found");

        var result = display.GetAdvancedColorInfo().AdvancedColorForceDisabled;
        return Task.FromResult(result);
    }

    public Task<HDRState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<HDRState>());

    public Task<HDRState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting current HDR state...");

        var display = InternalDisplay.Get() ?? throw new InvalidOperationException("Built in display not found");

        var result = display.GetAdvancedColorInfo().AdvancedColorEnabled ? HDRState.On : HDRState.Off;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"HDR is {result}");

        return Task.FromResult(result);
    }

    public async Task SetStateAsync(HDRState state)
    {
        var currentState = await GetStateAsync().ConfigureAwait(false);

        if (currentState == state)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"HDR already set to {state}");
            return;
        }

        var display = InternalDisplay.Get() ?? throw new InvalidOperationException("Built in display not found");

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting display HDR to {state}");

        display.SetAdvancedColorState(state == HDRState.On);
    }
}