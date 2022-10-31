using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractDriverFeature<T> : IFeature<T> where T : struct, Enum, IComparable
    {
        protected readonly uint ControlCode;
        protected readonly Func<SafeFileHandle> DriverHandle;

        protected T LastState;

        protected AbstractDriverFeature(Func<SafeFileHandle> driverHandleHandle, uint controlCode)
        {
            DriverHandle = driverHandleHandle;
            ControlCode = controlCode;
        }

        public virtual Task<bool> IsSupportedAsync() => Task.FromResult(true);

        public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

        public virtual async Task<T> GetStateAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

            var outBuffer = await SendCodeAsync(DriverHandle(), ControlCode, GetInBufferValue()).ConfigureAwait(false);
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
    }
}