using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractDriverFeature<T> : IFeature<T> where T : struct, IComparable
    {
        private readonly uint _controlCode;
        private readonly Func<SafeFileHandle> _driverHandle;

        protected T LastState;

        protected AbstractDriverFeature(Func<SafeFileHandle> driverHandleHandle, uint controlCode)
        {
            _driverHandle = driverHandleHandle;
            _controlCode = controlCode;
        }

        public async Task<T> GetStateAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

            var (_, outBuffer) = await SendCodeAsync(_driverHandle(), _controlCode, GetInternalStatus()).ConfigureAwait(false);
            var state = FromInternal(outBuffer);
            LastState = state;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"State is {state} [feature={GetType().Name}]");

            return state;
        }

        public async Task SetStateAsync(T state)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

            var codes = ToInternal(state);
            foreach (var code in codes)
                await SendCodeAsync(_driverHandle(), _controlCode, code).ConfigureAwait(false);
            LastState = state;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"State set to {state} [feature={GetType().Name}]");
        }

        protected abstract T FromInternal(uint state);
        protected abstract byte GetInternalStatus();
        protected abstract byte[] ToInternal(T state);

        private Task<(int bytesReturned, uint outBuffer)> SendCodeAsync(SafeFileHandle handle, uint controlCode, byte inBuffer)
        {
            return Task.Run(() =>
            {
                if (!Native.DeviceIoControl(handle, controlCode, ref inBuffer, sizeof(byte), out uint outBuffer, sizeof(uint), out var bytesReturned, IntPtr.Zero))
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