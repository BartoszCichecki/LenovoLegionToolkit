using System;
using System.Threading.Tasks;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Features.Hybrid.Notify;

public class DGPUNotify(DGPUGamezoneNotify gamezoneNotify, DGPUCapabilityNotify capabilityNotify, DGPUFeatureFlagsNotify featureFlagsNotify) : IDGPUNotify
{
    private readonly AsyncLock _lock = new();

    private bool _resolved;
    private IDGPUNotify? _dgpuNotify;

    private DGPUCapabilityNotify CapabilityNotify => capabilityNotify;
    private DGPUFeatureFlagsNotify FeatureFlagsNotify => featureFlagsNotify;
    private DGPUGamezoneNotify GamezoneNotify => gamezoneNotify;

    public bool ExperimentalGPUWorkingMode { get; set; }

    public event EventHandler<bool>? Notified
    {
        add
        {
            CapabilityNotify.Notified += value;
            FeatureFlagsNotify.Notified += value;
            GamezoneNotify.Notified += value;
        }
        remove
        {
            CapabilityNotify.Notified -= value;
            FeatureFlagsNotify.Notified -= value;
            GamezoneNotify.Notified -= value;
        }
    }

    public async Task<bool> IsSupportedAsync()
    {
        var dgpuNotify = await ResolveInternalAsync().ConfigureAwait(false);
        if (dgpuNotify is null)
            return false;
        return await dgpuNotify.IsSupportedAsync().ConfigureAwait(false);
    }

    public async Task<bool> IsDGPUAvailableAsync()
    {
        var dgpuNotify = await ResolveInternalAsync().ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found [type={GetType().Name}]");
        return await dgpuNotify.IsDGPUAvailableAsync().ConfigureAwait(false);
    }

    public async Task NotifyAsync(bool publish = true)
    {
        var dgpuNotify = await ResolveInternalAsync().ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found [type={GetType().Name}]");
        await dgpuNotify.NotifyAsync(publish).ConfigureAwait(false);
    }

    public async Task NotifyLaterIfNeededAsync()
    {
        var dgpuNotify = await ResolveInternalAsync().ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found [type={GetType().Name}]");
        await dgpuNotify.NotifyLaterIfNeededAsync().ConfigureAwait(false);
    }

    private async Task<IDGPUNotify?> ResolveAsync()
    {
        if (!ExperimentalGPUWorkingMode)
            return await GamezoneNotify.IsSupportedAsync().ConfigureAwait(false) ? GamezoneNotify : null;

        if (await CapabilityNotify.IsSupportedAsync().ConfigureAwait(false))
            return CapabilityNotify;

        if (await FeatureFlagsNotify.IsSupportedAsync().ConfigureAwait(false))
            return FeatureFlagsNotify;

        return null;
    }

    private async Task<IDGPUNotify?> ResolveInternalAsync()
    {
        using (await _lock.LockAsync().ConfigureAwait(false))
        {
            if (_resolved)
                return _dgpuNotify;

            _dgpuNotify = await ResolveAsync().ConfigureAwait(false);
            _resolved = true;
            return _dgpuNotify;
        }
    }
}
