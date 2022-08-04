using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
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

        public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

        public virtual async Task<T> GetStateAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

            var (_, outBuffer) = await SendCodeAsync(DriverHandle(), ControlCode, GetInBufferValue()).ConfigureAwait(false);
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

        protected Task<(int bytesReturned, uint outBuffer)> SendCodeAsync(SafeFileHandle handle, uint controlCode, uint inBuffer)
        {
            return Task.Run(() =>
            {
                if (!Native.DeviceIoControl(handle, controlCode, ref inBuffer, sizeof(uint), out uint outBuffer, sizeof(uint), out var bytesReturned, IntPtr.Zero))
                {
                    var error = Marshal.GetLastWin32Error();

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"DeviceIoControl returned 0, last error: {error} [feature={GetType().Name}]");

                    throw new InvalidOperationException($"DeviceIoControl returned 0, last error: {error}");
                }

                return (bytesReturned, outBuffer);
            });
        }

        protected static uint ReverseEndianness(uint state)
        {
            var bytes = BitConverter.GetBytes(state);
            Array.Reverse(bytes, 0, bytes.Length);
            return BitConverter.ToUInt32(bytes, 0);
        }

        protected static bool GetNthBit(uint num, int n) => (num & (1 << n)) != 0;
    }
}