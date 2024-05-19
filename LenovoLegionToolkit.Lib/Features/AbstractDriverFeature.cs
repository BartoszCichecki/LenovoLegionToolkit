using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractDriverFeature<T>(Func<SafeFileHandle> driverHandleHandle, uint controlCode) : IFeature<T> where T : struct, Enum, IComparable
{
    protected readonly uint ControlCode = controlCode;
    protected readonly Func<SafeFileHandle> DriverHandle = driverHandleHandle;

    protected T LastState;

    public virtual async Task<bool> IsSupportedAsync()
    {
        try
        {
            _ = await GetStateAsync().ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

    public virtual async Task<T> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

        var outBuffer = await SendCodeAsync(DriverHandle(), ControlCode, GetInBufferValue()).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Buffer value: {outBuffer} [feature={GetType().Name}]");

        var state = await FromInternalAsync(outBuffer).ConfigureAwait(false);
        LastState = state;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {state} [feature={GetType().Name}]");

        return state;
    }

    public virtual async Task SetStateAsync(T state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

        var codes = await ToInternalAsync(state).ConfigureAwait(false);
        foreach (var code in codes)
            await SendCodeAsync(DriverHandle(), ControlCode, code).ConfigureAwait(false);
        LastState = state;

        await VerifyStateSetAsync(state).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State set to {state} [feature={GetType().Name}]");
    }

    protected abstract Task<T> FromInternalAsync(uint state);

    protected abstract uint GetInBufferValue();

    protected abstract Task<uint[]> ToInternalAsync(T state);

    protected Task<uint> SendCodeAsync(SafeFileHandle handle, uint controlCode, uint inBuffer) => Task.Run(() =>
    {
        if (PInvokeExtensions.DeviceIoControl(handle, controlCode, inBuffer, out uint outBuffer))
            return outBuffer;

        var error = Marshal.GetLastWin32Error();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"DeviceIoControl returned 0, last error: {error} [feature={GetType().Name}]");

        throw new InvalidOperationException($"DeviceIoControl returned 0, last error: {error}");
    });

    private async Task VerifyStateSetAsync(T state)
    {
        var retries = 0;
        while (retries < 10)
        {
            if (state.Equals(await GetStateAsync().ConfigureAwait(false)))
                break;

            retries++;

            await Task.Delay(50).ConfigureAwait(false);
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Verify state {state} set failed. [feature={GetType().Name}]");
    }
}
