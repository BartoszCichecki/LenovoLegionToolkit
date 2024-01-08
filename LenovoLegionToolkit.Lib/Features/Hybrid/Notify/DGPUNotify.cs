using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features.Hybrid.Notify;

public class DGPUNotify : IDGPUNotify
{
    private readonly DGPUCapabilityNotify _capabilityNotify;
    private readonly DGPUFeatureFlagsNotify _featureFlagsNotify;
    private readonly DGPUGamezoneNotify _gamezoneNotify;

    private readonly Lazy<Task<IDGPUNotify?>> _lazyAsyncNotify;

    public event EventHandler<bool>? Notified
    {
        add
        {
            _capabilityNotify.Notified += value;
            _featureFlagsNotify.Notified += value;
            _gamezoneNotify.Notified += value;
        }
        remove
        {
            _capabilityNotify.Notified -= value;
            _featureFlagsNotify.Notified -= value;
            _gamezoneNotify.Notified -= value;
        }
    }

    public bool ExperimentalGPUWorkingMode { get; set; }

    public DGPUNotify(DGPUGamezoneNotify gamezoneNotify, DGPUCapabilityNotify capabilityNotify, DGPUFeatureFlagsNotify featureFlagsNotify)
    {
        _gamezoneNotify = gamezoneNotify ?? throw new ArgumentNullException(nameof(gamezoneNotify));
        _capabilityNotify = capabilityNotify ?? throw new ArgumentNullException(nameof(capabilityNotify));
        _featureFlagsNotify = featureFlagsNotify ?? throw new ArgumentNullException(nameof(featureFlagsNotify));

        _lazyAsyncNotify = new(GetNotifyLazyAsync);
    }

    public async Task<bool> IsSupportedAsync() => await _lazyAsyncNotify.Value.ConfigureAwait(false) != null;

    public async Task<bool> IsDGPUAvailableAsync()
    {
        var feature = await _lazyAsyncNotify.Value.ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found. [type={GetType().Name}]");
        return await feature.IsDGPUAvailableAsync().ConfigureAwait(false);
    }

    public async Task NotifyAsync(bool publish = true)
    {
        var feature = await _lazyAsyncNotify.Value.ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found. [type={GetType().Name}]");
        await feature.NotifyAsync(publish).ConfigureAwait(false);
    }

    public async Task NotifyLaterIfNeededAsync()
    {
        var feature = await _lazyAsyncNotify.Value.ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found. [type={GetType().Name}]");
        await feature.NotifyLaterIfNeededAsync().ConfigureAwait(false);
    }

    private async Task<IDGPUNotify?> GetNotifyLazyAsync()
    {
        if (ExperimentalGPUWorkingMode)
        {
            if (await _capabilityNotify.IsSupportedAsync().ConfigureAwait(false))
                return _capabilityNotify;

            if (await _featureFlagsNotify.IsSupportedAsync().ConfigureAwait(false))
                return _featureFlagsNotify;

            return null;
        }

        return await _gamezoneNotify.IsSupportedAsync().ConfigureAwait(false) ? _gamezoneNotify : null;
    }
}
