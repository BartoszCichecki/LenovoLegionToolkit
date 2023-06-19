using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib;

// ReSharper disable InconsistentNaming
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

#region Battery

[StructLayout(LayoutKind.Sequential)]
internal readonly struct LENOVO_BATTERY_INFORMATION
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
    private readonly byte[] Bytes1;
    public readonly ushort Temperature;
    public readonly ushort ManufactureDate;
    public readonly ushort FirstUseDate;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    private readonly byte[] Bytes2;
}

#endregion

#region RGB Keyboard

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct LENOVO_RGB_KEYBOARD_STATE
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Header;
    public byte Effect;
    public byte Speed;
    public byte Brightness;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Zone1Rgb;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Zone2Rgb;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Zone3Rgb;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Zone4Rgb;
    public byte Padding;
    public byte WaveLTR;
    public byte WaveRTL;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
    public byte[] Unused;
}

#endregion

#region Spectrum Keyboard

internal enum LENOVO_SPECTRUM_OPERATION_TYPE : byte
{
    Compatibility = 0xD1,
    KeyCount = 0xC4,
    KeyPage = 0xC5,
    ProfileChange = 0xC8,
    ProfileDefault = 0xC9,
    Profile = 0xCA,
    EffectChange = 0xCB,
    Effect = 0xCC,
    GetBrightness = 0xCD,
    Brightness = 0xCE,
    AuroraStartStop = 0xD0,
    AuroraSendBitmap = 0xA1,
    GetLogoStatus = 0xA5,
    LogoStatus = 0xA6
}

internal enum LENOVO_SPECTRUM_EFFECT_TYPE : byte
{
    ScrewRainbow = 1,
    RainbowWave = 2,
    ColorChange = 3,
    ColorPulse = 4,
    ColorWave = 5,
    Smooth = 6,
    Rain = 7,
    Ripple = 8,
    AudioBounceLighting = 9,
    AudioRippleLighting = 10,
    Always = 11,
    TypeLighting = 12,
    LegionAuraSync = 13,
}

internal enum LENOVO_SPECTRUM_COLOR_MODE : byte
{
    None = 0,
    RandomColor = 1,
    ColorList = 2
}

internal enum LENOVO_SPECTRUM_SPEED : byte
{
    None = 0,
    Speed1 = 1,
    Speed2 = 2,
    Speed3 = 3
}

internal enum LENOVO_SPECTRUM_CLOCKWISE_DIRECTION : byte
{
    None = 0,
    Clockwise = 1,
    CounterClockwise = 2
}

internal enum LENOVO_SPECTRUM_DIRECTION : byte
{
    None = 0,
    BottomToTop = 1,
    TopToBottom = 2,
    RightToLeft = 3,
    LeftToRight = 4
}

[StructLayout(LayoutKind.Sequential)]
internal readonly struct LENOVO_SPECTRUM_COLOR
{
    public readonly byte R;
    public readonly byte G;
    public readonly byte B;

    public LENOVO_SPECTRUM_COLOR(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct LENOVO_SPECTRUM_KEY_STATE
{
    public readonly ushort KeyCode;
    public LENOVO_SPECTRUM_COLOR Color;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct LENOVO_SPECTRUM_KEY_PAGE_ITEM
{
    private readonly byte Index;
    public readonly ushort KeyCode;
}

[StructLayout(LayoutKind.Sequential)]
internal readonly struct LENOVO_SPECTRUM_HEADER
{
    public readonly byte Head = 7;
    public readonly LENOVO_SPECTRUM_OPERATION_TYPE Type;
    public readonly byte Size;
    public readonly byte Tail = 3;

    public LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE type, int size)
    {
        Type = type;
        Size = (byte)(size % 256);
    }
}

[StructLayout(LayoutKind.Sequential)]
internal readonly struct LENOVO_SPECTRUM_EFFECT_HEADER
{
    public readonly byte Head = 0x6;
    public readonly byte Unknown1 = 0x1;
    public readonly LENOVO_SPECTRUM_EFFECT_TYPE EffectType;
    public readonly byte Unknown2 = 0x2;
    public readonly LENOVO_SPECTRUM_SPEED Speed;
    public readonly byte Unknown3 = 0x3;
    public readonly LENOVO_SPECTRUM_CLOCKWISE_DIRECTION ClockwiseDirection;
    public readonly byte Unknown5 = 0x4;
    public readonly LENOVO_SPECTRUM_DIRECTION Direction;
    public readonly byte Unknown6 = 0x5;
    public readonly LENOVO_SPECTRUM_COLOR_MODE ColorMode;
    public readonly byte Unknown7 = 0x6;
    public readonly byte Tail = 0x0;

    public LENOVO_SPECTRUM_EFFECT_HEADER(
        LENOVO_SPECTRUM_EFFECT_TYPE effectType,
        LENOVO_SPECTRUM_SPEED speed,
        LENOVO_SPECTRUM_DIRECTION direction,
        LENOVO_SPECTRUM_CLOCKWISE_DIRECTION clockwiseDirection,
        LENOVO_SPECTRUM_COLOR_MODE colorMode)
    {
        EffectType = effectType;
        Speed = speed;
        Direction = direction;
        ClockwiseDirection = clockwiseDirection;
        ColorMode = colorMode;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct LENOVO_SPECTRUM_EFFECT
{
    public readonly byte EffectNo;
    public readonly LENOVO_SPECTRUM_EFFECT_HEADER EffectHeader;
    public readonly byte NumberOfColors;
    public readonly LENOVO_SPECTRUM_COLOR[] Colors;
    public readonly byte NumberOfKeys;
    public readonly ushort[] KeyCodes;

    public LENOVO_SPECTRUM_EFFECT(LENOVO_SPECTRUM_EFFECT_HEADER effectHeader, int effectNo, LENOVO_SPECTRUM_COLOR[] colors, ushort[] keyCodes)
    {
        EffectHeader = effectHeader;
        EffectNo = (byte)effectNo;
        NumberOfColors = (byte)colors.Length;
        Colors = colors;
        NumberOfKeys = (byte)keyCodes.Length;
        KeyCodes = keyCodes;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct LENOVO_SPECTRUM_AURORA_ITEM
{
    public readonly ushort KeyCode;
    public readonly LENOVO_SPECTRUM_COLOR Color;

    public LENOVO_SPECTRUM_AURORA_ITEM(ushort keyCode, LENOVO_SPECTRUM_COLOR color)
    {
        KeyCode = keyCode;
        Color = color;
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_COMPATIBILITY_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;

    public LENOVO_SPECTRUM_GET_COMPATIBILITY_REQUEST()
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.Compatibility, 0xC0);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_COMPATIBILITY_RESPONSE
{
    private readonly byte ReportId;
    private readonly LENOVO_SPECTRUM_OPERATION_TYPE Type;
    private readonly byte Length;
    private readonly byte Unknown1;
    private readonly byte Compatibility;

    public bool IsCompatible => Compatibility == 0;
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_KEY_COUNT_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly byte Parameter = 7;

    public LENOVO_SPECTRUM_GET_KEY_COUNT_REQUEST()
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.KeyCount, 0xC0);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_KEY_COUNT_RESPONSE
{
    private readonly byte ReportId;
    private readonly LENOVO_SPECTRUM_OPERATION_TYPE Type;
    private readonly byte Length;
    private readonly byte Unknown1;
    private readonly byte Parameter;
    public readonly byte Indexes;
    public readonly byte KeysPerIndex;
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_KEY_PAGE_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly byte Parameter;
    private readonly byte Index;

    public LENOVO_SPECTRUM_GET_KEY_PAGE_REQUEST(byte index, bool secondary = false)
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.KeyPage, 0xC0);
        Parameter = secondary ? (byte)8 : (byte)7;
        Index = index;
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_KEY_PAGE_RESPONSE
{
    private readonly byte ReportId;
    private readonly LENOVO_SPECTRUM_OPERATION_TYPE Type;
    private readonly byte Length;
    private readonly byte Unknown1;
    private readonly byte Parameter;
    private readonly byte Index;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public readonly LENOVO_SPECTRUM_KEY_PAGE_ITEM[] Items;
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_BRIGHTNESS_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;

    public LENOVO_SPECTRUM_GET_BRIGHTNESS_REQUEST()
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.GetBrightness, 0xC0);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_BRIGHTNESS_RESPONSE
{
    private readonly byte ReportId;
    private readonly LENOVO_SPECTRUM_OPERATION_TYPE Type;
    private readonly byte Length;
    private readonly byte Unknown1;
    public readonly byte Brightness;
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_SET_BRIGHTNESS_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly byte Brightness;

    public LENOVO_SPECTRUM_SET_BRIGHTNESS_REQUEST(byte brightness)
    {
        Header = new(LENOVO_SPECTRUM_OPERATION_TYPE.Brightness, 0xC0);
        Brightness = brightness;
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_LOGO_STATUS
{
    private readonly LENOVO_SPECTRUM_HEADER Header;

    public LENOVO_SPECTRUM_GET_LOGO_STATUS()
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.GetLogoStatus, 0xC0);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_LOGO_STATUS_RESPONSE
{
    private readonly byte ReportId;
    private readonly LENOVO_SPECTRUM_OPERATION_TYPE Type;
    private readonly byte Length;
    private readonly byte Unknown1;
    private readonly byte Status;

    public bool IsOn => Status == 1;
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_SET_LOGO_STATUS_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly byte Status;

    public LENOVO_SPECTRUM_SET_LOGO_STATUS_REQUEST(bool isOn)
    {
        Header = new(LENOVO_SPECTRUM_OPERATION_TYPE.LogoStatus, 0xC0);
        Status = (byte)(isOn ? 1 : 0);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_PROFILE_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;

    public LENOVO_SPECTRUM_GET_PROFILE_REQUEST()
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.Profile, 0xC0);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_PROFILE_RESPONSE
{
    private readonly byte ReportId;
    private readonly LENOVO_SPECTRUM_OPERATION_TYPE Type;
    private readonly byte Length;
    private readonly byte Unknown1;
    public readonly byte Profile;
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_SET_PROFILE_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly byte Profile;

    public LENOVO_SPECTRUM_SET_PROFILE_REQUEST(byte profile)
    {
        Header = new(LENOVO_SPECTRUM_OPERATION_TYPE.ProfileChange, 0xC0);
        Profile = profile;
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_SET_PROFILE_DEFAULT_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly byte Profile;

    public LENOVO_SPECTRUM_SET_PROFILE_DEFAULT_REQUEST(byte profile)
    {
        Header = new(LENOVO_SPECTRUM_OPERATION_TYPE.ProfileDefault, 0xC0);
        Profile = profile;
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_GET_EFFECT_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly byte Profile;

    public LENOVO_SPECTRUM_GET_EFFECT_REQUEST(byte profile)
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.Effect, 0xC0);
        Profile = profile;
    }
}

internal readonly struct LENOVO_SPECTRUM_EFFECT_DESCRIPTION
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    public readonly byte Profile;
    private readonly byte Unknown1 = 1;
    private readonly byte Unknown2 = 1;
    public readonly LENOVO_SPECTRUM_EFFECT[] Effects;

    public LENOVO_SPECTRUM_EFFECT_DESCRIPTION(LENOVO_SPECTRUM_HEADER header, byte profile, LENOVO_SPECTRUM_EFFECT[] effects)
    {
        Header = header;
        Profile = profile;
        Effects = effects;
    }

    public static LENOVO_SPECTRUM_EFFECT_DESCRIPTION FromBytes(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        using var br = new BinaryReader(ms);

        _ = br.ReadByte();
        var type = (LENOVO_SPECTRUM_OPERATION_TYPE)br.ReadByte();
        var size = br.ReadByte();
        _ = br.ReadByte();

        var header = new LENOVO_SPECTRUM_HEADER(type, size);

        var profile = br.ReadByte();
        _ = br.ReadByte();
        _ = br.ReadByte();

        var effects = new List<LENOVO_SPECTRUM_EFFECT>();

        var lastEffectNo = 1;
        while (true)
        {
            var effectNo = br.ReadByte();

            if (effectNo < lastEffectNo)
                break;

            lastEffectNo = effectNo;

            _ = br.ReadByte();
            _ = br.ReadByte();
            var effectType = (LENOVO_SPECTRUM_EFFECT_TYPE)br.ReadByte();
            _ = br.ReadByte();
            var speed = (LENOVO_SPECTRUM_SPEED)br.ReadByte();
            _ = br.ReadByte();
            var clockwiseDirection = (LENOVO_SPECTRUM_CLOCKWISE_DIRECTION)br.ReadByte();
            _ = br.ReadByte();
            var direction = (LENOVO_SPECTRUM_DIRECTION)br.ReadByte();
            _ = br.ReadByte();
            var colorMode = (LENOVO_SPECTRUM_COLOR_MODE)br.ReadByte();
            _ = br.ReadByte();
            _ = br.ReadByte();

            var effectHeader = new LENOVO_SPECTRUM_EFFECT_HEADER(effectType, speed, direction, clockwiseDirection, colorMode);
            var colors = new List<LENOVO_SPECTRUM_COLOR>();
            var keyCodes = new List<ushort>();

            var noOfColors = br.ReadByte();
            for (var i = 0; i < noOfColors; i++)
            {
                var r = br.ReadByte();
                var g = br.ReadByte();
                var b = br.ReadByte();
                colors.Add(new(r, g, b));
            }

            var noOfKeys = br.ReadByte();
            for (var i = 0; i < noOfKeys; i++)
            {
                var keyCode = br.ReadUInt16();
                keyCodes.Add(keyCode);
            }

            effects.Add(new(effectHeader, effectNo, colors.ToArray(), keyCodes.ToArray()));
        }

        return new(header, profile, effects.ToArray());
    }

    public byte[] ToBytes()
    {
        using var ms = new MemoryStream(new byte[960]);
        using var bf = new BinaryWriter(ms);

        bf.Write(Header.Head);
        bf.Write((byte)Header.Type);
        bf.Write(Header.Size);
        bf.Write(Header.Tail);

        bf.Write(Profile);
        bf.Write(Unknown1);
        bf.Write(Unknown2);

        foreach (var effect in Effects)
        {
            bf.Write(effect.EffectNo);

            bf.Write(effect.EffectHeader.Head);
            bf.Write(effect.EffectHeader.Unknown1);
            bf.Write((byte)effect.EffectHeader.EffectType);
            bf.Write(effect.EffectHeader.Unknown2);
            bf.Write((byte)effect.EffectHeader.Speed);
            bf.Write(effect.EffectHeader.Unknown3);
            bf.Write((byte)effect.EffectHeader.ClockwiseDirection);
            bf.Write(effect.EffectHeader.Unknown5);
            bf.Write((byte)effect.EffectHeader.Direction);
            bf.Write(effect.EffectHeader.Unknown6);
            bf.Write((byte)effect.EffectHeader.ColorMode);
            bf.Write(effect.EffectHeader.Unknown7);
            bf.Write(effect.EffectHeader.Tail);

            bf.Write(effect.NumberOfColors);
            foreach (var color in effect.Colors)
            {
                bf.Write(color.R);
                bf.Write(color.G);
                bf.Write(color.B);
            }

            bf.Write(effect.NumberOfKeys);
            foreach (var keyCode in effect.KeyCodes)
            {
                bf.Write(keyCode);
            }
        }

        var position = ms.Position;
        bf.Seek(2, SeekOrigin.Begin);
        bf.Write((byte)(position % 255));

        return ms.ToArray();
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_AURORA_START_STOP_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly byte StartStop;
    private readonly byte Profile;

    public LENOVO_SPECTRUM_AURORA_START_STOP_REQUEST(bool start, byte profile)
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.AuroraStartStop, 0xC0);
        StartStop = start ? (byte)1 : (byte)2;
        Profile = profile;
    }
}

internal readonly struct LENOVO_SPECTRUM_AURORA_SEND_BITMAP_REQUEST
{
    private readonly LENOVO_SPECTRUM_HEADER Header;
    private readonly LENOVO_SPECTRUM_AURORA_ITEM[] Items;

    public LENOVO_SPECTRUM_AURORA_SEND_BITMAP_REQUEST(LENOVO_SPECTRUM_AURORA_ITEM[] items)
    {
        Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.AuroraSendBitmap, 0xC0);
        Items = items;
    }

    public byte[] ToBytes()
    {
        using var ms = new MemoryStream(new byte[960]);
        using var bf = new BinaryWriter(ms);

        bf.Write(Header.Head);
        bf.Write((byte)Header.Type);
        bf.Write(Header.Size);
        bf.Write(Header.Tail);

        foreach (var item in Items)
        {
            bf.Write(item.KeyCode);
            bf.Write(item.Color.R);
            bf.Write(item.Color.G);
            bf.Write(item.Color.B);
        }

        return ms.ToArray();
    }
}

[StructLayout(LayoutKind.Sequential, Size = 960)]
internal readonly struct LENOVO_SPECTRUM_STATE_RESPONSE
{
    private readonly byte ReportId;
    private readonly LENOVO_SPECTRUM_OPERATION_TYPE Type;
    private readonly byte Length;
    private readonly byte Unknown1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 190)]
    public readonly LENOVO_SPECTRUM_KEY_STATE[] Data;
}

#endregion
