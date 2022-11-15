using System.Runtime.InteropServices;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Windows.Win32;


Console.WriteLine(@"How to use:
  1. Open Vantage
  2. Select the key that you want to find keycode for
  3. Set the key to plain white (Hex: #FFFFFF, RGB: 255,255,255)
  4. Make sure all other keys don't have any effect set (are off)
  5. Set the keyboard brightness to maximum

When ready, press any key to continue...");
Console.ReadLine();

var extHandle = Devices.GetExtendedSpectrumRGBKeyboard();
var handle = Devices.GetSpectrumRGBKeyboard();
var device = extHandle ?? handle;

Console.WriteLine("Finding Spectrum keyboard...");

if (device is null)
{
    Console.WriteLine("Spectrum not supported");
    Console.ReadLine();
    return;
}

Console.WriteLine($"Spectrum keyboard found, reading white key keycodes... [ext={extHandle is not null}]");
Console.WriteLine();

for (var i = 0; i < 10; i++)
{
    GetFeature(device, out LENOVO_SPECTRUM_STATE state);

    var keys = state.Data
        .Where(kv => HasColor(kv.Color))
        .Select(kv => kv.Key)
        .Select(k => "0x" + k.ToString("X4"))
        .Split(8);

    await Task.Delay(1000);

    if (keys.SelectMany(k => k).IsEmpty())
    {
        Console.WriteLine($"[{i + 1}/10] No keys found");
        continue;
    }

    Console.WriteLine($"[{i + 1}/10] Keys with color found:");

    foreach (var line in keys)
        Console.WriteLine("    " + string.Join(", ", line));

}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadLine();

#region Methods

bool HasColor(LENOVO_SPECTRUM_COLOR rgbColor) => rgbColor.Red == 255 && rgbColor.Green == 255 && rgbColor.Blue == 255;

unsafe void GetFeature<T>(SafeHandle handle, out T str) where T : struct
{
    var ptr = IntPtr.Zero;
    try
    {
        var size = Marshal.SizeOf<T>();
        ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(new byte[] { 7 }, 0, ptr, 1);

        var result = PInvoke.HidD_GetFeature(handle, ptr.ToPointer(), (uint)size);
        if (!result)
            PInvokeExtensions.ThrowIfWin32Error(typeof(T).Name);

        str = Marshal.PtrToStructure<T>(ptr);
    }
    finally
    {
        Marshal.FreeHGlobal(ptr);
    }
}

#endregion

#region Structs

internal enum LENOVO_SPECTRUM_OPERATION_TYPE : byte
{
    ProfileSet1 = 0xC8,
    GetProfile = 0xCA,
    EffectChange = 0xCB,
    ProfileSet2 = 0xCC,
    GetBrightness = 0xCD,
    Brightness = 0xCE,
    AuroraSendBitmap = 0xA1,
    State = 0x03,
    Unknown1 = 0xC6,
    Unknown2 = 0x04,
}

[StructLayout(LayoutKind.Sequential)]
internal struct LENOVO_SPECTRUM_COLOR
{
    public byte Red;
    public byte Green;
    public byte Blue;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct LENOVO_SPECTRUM_KEY_STATE
{
    public ushort Key;
    public LENOVO_SPECTRUM_COLOR Color;
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal struct LENOVO_SPECTRUM_STATE
{
    public byte ReportId;
    public LENOVO_SPECTRUM_OPERATION_TYPE Type;
    public byte Unknown2;
    public byte Unknown3;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 191)]
    public LENOVO_SPECTRUM_KEY_STATE[] Data;
    public byte Unknown4;
}

#endregion
