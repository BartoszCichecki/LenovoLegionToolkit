using System;
using System.Runtime.InteropServices;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractDriverFeature<T> : IFeature<T> where T : struct, IComparable
    {
        private readonly uint _controlCode;
        private readonly SafeFileHandle _driverHandle;

        protected T LastState;

        protected AbstractDriverFeature(SafeFileHandle driverHandleHandle, uint controlCode)
        {
            _driverHandle = driverHandleHandle;
            _controlCode = controlCode;
        }

        public T GetState()
        {
            Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");
            SendCode(_driverHandle, _controlCode, GetInternalStatus(), out var result);
            var state = FromInternal(result);
            LastState = state;
            Log.Instance.Trace($"State is {state} [feature={GetType().Name}]");
            return state;
        }

        public void SetState(T state)
        {
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");
            var codes = ToInternal(state);
            foreach (var code in codes)
                SendCode(_driverHandle, _controlCode, code, out _);
            LastState = state;
            Log.Instance.Trace($"State set to {state} [feature={GetType().Name}]");
        }

        protected abstract T FromInternal(uint state);
        protected abstract byte GetInternalStatus();
        protected abstract byte[] ToInternal(T state);

        private int SendCode(SafeFileHandle handle, uint controlCode, byte inBuffer, out uint outBuffer)
        {
            if (!Native.DeviceIoControl(handle, controlCode, ref inBuffer, sizeof(byte), out outBuffer, sizeof(uint), out var bytesReturned, IntPtr.Zero))
            {
                var error = Marshal.GetLastWin32Error();
                Log.Instance.Trace($"DeviceIoControl returned 0, last error: {error} [feature={GetType().Name}]");
                throw new InvalidOperationException($"DeviceIoControl returned 0, last error: {error}");
            }

            return bytesReturned;
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