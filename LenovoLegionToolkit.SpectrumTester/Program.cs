using System.Runtime.InteropServices;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Windows.Win32;


Console.WriteLine(@"How to use:
  1. Make sure Vantage and LLT is closed.
  2. Set the keyboard brightness to maximum.

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


Console.WriteLine("Spectrum keyboard found");

Console.WriteLine();
Console.WriteLine($"Reading response for 0xC6...");

SetFeature(device, new LENOVO_SPECTRUM_UNKNOWN1_REQUEST());
GetFeature(device, out LENOVO_SPECTRUM_GENERIC_RESPONSE res1);

var res1Output = res1.Bytes.Take(128).Split(16);
foreach (var i in res1Output)
    Console.WriteLine(string.Join(" ", i.Select(i => $"{i:X2}")));


Console.WriteLine();
Console.WriteLine("Reading response for 0x04...");

SetFeature(device, new LENOVO_SPECTRUM_UNKNOWN2_REQUEST());
GetFeature(device, out LENOVO_SPECTRUM_GENERIC_RESPONSE res2);

var res2Output = res2.Bytes.Take(128).Split(16);
foreach (var i in res2Output)
    Console.WriteLine(string.Join(" ", i.Select(i => $"{i:X2}")));

Console.WriteLine();

Console.WriteLine(@"Reading config complete.

How to find a keycode for a specific key:
  1. Open Vantage
  2. Select the key that you want to find keycode for
  3. Set the key to plain white (Hex: #FFFFFF, RGB: 255,255,255)
  4. Make sure all other keys don't have any effect set (are off)
  5. Set the keyboard brightness to maximum

When ready, press any key to continue...");
Console.ReadLine();

Console.WriteLine($"Reading white key keycodes... [ext={extHandle is not null}]");
Console.WriteLine();

const int Iterations = 5;

for (var i = 0; i < Iterations; i++)
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
        Console.WriteLine($"[{i + 1}/{Iterations}] No keys found");
        continue;
    }

    Console.WriteLine($"[{i + 1}/{Iterations}] Keys with color found:");

    foreach (var line in keys)
        Console.WriteLine("    " + string.Join(", ", line));

}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadLine();

#region Methods

bool HasColor(LENOVO_SPECTRUM_COLOR rgbColor) => rgbColor.Red == 255 && rgbColor.Green == 255 && rgbColor.Blue == 255;

unsafe void SetFeature<T>(SafeHandle handle, T str) where T : notnull
{
    var ptr = IntPtr.Zero;
    try
    {
        int size;
        if (str is byte[] bytes)
        {
            size = bytes.Length;
            ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
        }
        else
        {
            size = Marshal.SizeOf<T>();
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, false);
        }

        var result = PInvoke.HidD_SetFeature(handle, ptr.ToPointer(), (uint)size);
        if (!result)
            PInvokeExtensions.ThrowIfWin32Error(typeof(T).Name);
    }
    finally
    {
        Marshal.FreeHGlobal(ptr);
    }
}

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

[StructLayout(LayoutKind.Sequential)]
internal struct LENOVO_SPECTRUM_HEADER
{
    public byte Head = 7;
    public LENOVO_SPECTRUM_OPERATION_TYPE Type;
    public byte Size;
    public byte Tail = 3;

    public LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE type, int size)
    {
        Type = type;
        Size = (byte)(size % 256);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal struct LENOVO_SPECTRUM_UNKNOWN1_REQUEST
{
    public LENOVO_SPECTRUM_HEADER Header;

    public LENOVO_SPECTRUM_UNKNOWN1_REQUEST()
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.Unknown1, 0xC0);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal struct LENOVO_SPECTRUM_UNKNOWN2_REQUEST
{
    public LENOVO_SPECTRUM_HEADER Header;

    public LENOVO_SPECTRUM_UNKNOWN2_REQUEST()
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.Unknown2, 0xC0);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal struct LENOVO_SPECTRUM_GENERIC_RESPONSE
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 960)]
    public byte[] Bytes;
}

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
