using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractUSBHIDFeature<T> : IFeature<T> where T : struct, Enum, IComparable
    {
        private readonly Func<SafeFileHandle> _driverHandle;

        protected T LastState;

        protected AbstractUSBHIDFeature(Func<SafeFileHandle> driverHandleHandle)
        {
            _driverHandle = driverHandleHandle;
        }
        public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

        public async Task<T> GetStateAsync()
        {
            var state = FromInternal(KeyboardData.LegionRGBKey);

            return state;
        }

        public async Task SetStateAsync(T state)
        {
            //Todo
            KeyboardData.LegionRGBKey = ToInternal(state, KeyboardData.LegionRGBKey);
            await SendHidReport(_driverHandle(), KeyboardData.LegionRGBKey);
        }

         private Task SendHidReport(SafeFileHandle handle,LegionRGBKey LegionRGB)
         {
             return Task.Run(() =>
             {
                     
                 if (!Native.HidD_SetFeature(handle,ref LegionRGB, 33))
                 {
                     var error = Marshal.GetLastWin32Error();

                     if (Log.Instance.IsTraceEnabled)
                         Log.Instance.Trace($"HidD_SetFeature returned 0, last error: {error} [feature={GetType().Name}]");

                     throw new InvalidOperationException($"DeviceIoControl returned 0, last error: {error}");
                 }
             });
         }

        protected abstract T FromInternal(LegionRGBKey LegionRGB);
        protected abstract LegionRGBKey ToInternal(T state, LegionRGBKey LegionRGB);

    }
}
