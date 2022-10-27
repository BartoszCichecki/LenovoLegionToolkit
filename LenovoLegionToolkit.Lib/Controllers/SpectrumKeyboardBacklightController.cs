using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class SpectrumKeyboardBacklightController
    {
        private static readonly object IoLock = new();

        private readonly Vantage _vantage;

        private SafeFileHandle? DriverHandle => Devices.GetSpectrumRGBKeyboard();

        public SpectrumKeyboardBacklightController(Vantage vantage)
        {
            _vantage = vantage ?? throw new ArgumentNullException(nameof(vantage));
        }

        public bool IsSupported() => DriverHandle is not null;

        public async Task<int> GetBrightnessAsync()
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var input = new LENOVO_SPECTRUM_GET_BRIGHTNESS();
            SetAndGetFeature(input, out LENOVO_SPECTRUM_GET_BRIGTHNESS_RESPONSE output);
            return output.Brightness;
        }

        public async Task SetBrightnessAsync(int brightness)
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var input = new LENOVO_SPECTRUM_SET_BRIGHTHNESS((byte)brightness);
            SetFeature(input);
        }

        public async Task<int> GetProfileAsync()
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var input = new LENOVO_SPECTRUM_GET_PROFILE();
            SetAndGetFeature(input, out LENOVO_SPECTRUM_GET_PROFILE_RESPONSE output);
            return output.Profile;
        }

        public async Task SetProfileAsync(int profile)
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var input = new LENOVO_SPECTRUM_SET_PROFILE((byte)profile, true);
            SetFeature(input);
        }

        private void ThrowIfHandleNull()
        {
            var handle = DriverHandle;
            if (handle is null)
                throw new InvalidOperationException("Spectrum Keyboard unsupported.");
        }

        private async Task ThrowIfVantageEnabled()
        {
            var vantageStatus = await _vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == SoftwareStatus.Enabled)
                throw new InvalidOperationException("Can't manage Spectrum keyboard with Vantage enabled.");
        }

        private void SetAndGetFeature<TIn, TOut>(TIn input, out TOut output) where TIn : struct where TOut : struct
        {
            lock (IoLock)
            {
                SetFeature(input);
                GetFeature(out output);
            }
        }

        private unsafe void SetFeature<T>(T str) where T : struct
        {
            lock (IoLock)
            {
                var ptr = IntPtr.Zero;
                try
                {
                    var size = Marshal.SizeOf<T>();
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.StructureToPtr(str, ptr, false);

                    var result = PInvoke.HidD_SetFeature(DriverHandle, ptr.ToPointer(), (uint)size);
                    if (!result)
                        PInvokeExtensions.ThrowIfWin32Error(typeof(T).Name);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        private unsafe void GetFeature<T>(out T str) where T : struct
        {
            lock (IoLock)
            {
                var ptr = IntPtr.Zero;
                try
                {
                    var size = Marshal.SizeOf<T>();
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.Copy(new byte[] { 7 }, 0, ptr, 1);

                    var result = PInvoke.HidD_GetFeature(DriverHandle, ptr.ToPointer(), (uint)size);
                    if (!result)
                        PInvokeExtensions.ThrowIfWin32Error(typeof(T).Name);

                    str = Marshal.PtrToStructure<T>(ptr);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }
}
