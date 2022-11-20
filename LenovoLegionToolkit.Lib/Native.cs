using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib
{

    #region Battery

    [StructLayout(LayoutKind.Sequential)]
    internal struct LENOVO_BATTERY_INFORMATION
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] Bytes1;
        public ushort Temperature;
        public ushort ManufactureDate;
        public ushort FirstUseDate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Bytes2;
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
    internal struct LENOVO_SPECTRUM_COLOR
    {
        public byte R;
        public byte G;
        public byte B;

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
        public ushort Key;
        public LENOVO_SPECTRUM_COLOR Color;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LENOVO_SPECTRUM_KEYPAGE_ITEM
    {
        public byte Index;
        public ushort Key;
    }

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

    [StructLayout(LayoutKind.Sequential)]
    internal struct LENOVO_SPECTRUM_EFFECT_HEADER
    {
        public byte Head = 0x6;
        public byte Unknown1 = 0x1;
        public LENOVO_SPECTRUM_EFFECT_TYPE EffectType;
        public byte Unknown2 = 0x2;
        public LENOVO_SPECTRUM_SPEED Speed;
        public byte Unknown3 = 0x3;
        public LENOVO_SPECTRUM_CLOCKWISE_DIRECTION ClockwiseDirection;
        public byte Unknown5 = 0x4;
        public LENOVO_SPECTRUM_DIRECTION Direction;
        public byte Unknown6 = 0x5;
        public LENOVO_SPECTRUM_COLOR_MODE ColorMode;
        public byte Unknown7 = 0x6;
        public byte Tail = 0x0;

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
    internal struct LENOVO_SPECTRUM_EFFECT
    {
        public byte EffectNo;
        public LENOVO_SPECTRUM_EFFECT_HEADER EffectHeader;
        public byte NumberOfColors;
        public LENOVO_SPECTRUM_COLOR[] Colors;
        public byte NumberOfKeys;
        public ushort[] Keys;

        public LENOVO_SPECTRUM_EFFECT(LENOVO_SPECTRUM_EFFECT_HEADER effectHeader, int effectNo, LENOVO_SPECTRUM_COLOR[] colors, ushort[] keys)
        {
            EffectHeader = effectHeader;
            EffectNo = (byte)effectNo;
            NumberOfColors = (byte)colors.Length;
            Colors = colors;
            NumberOfKeys = (byte)keys.Length;
            Keys = keys;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LENOVO_SEPCTRUM_AURORA_ITEM
    {
        public ushort KeyCode;
        public LENOVO_SPECTRUM_COLOR Color;

        public LENOVO_SEPCTRUM_AURORA_ITEM(ushort keyCode, LENOVO_SPECTRUM_COLOR color)
        {
            KeyCode = keyCode;
            Color = color;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_COMPATIBILITY_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;

        public LENOVO_SPECTRUM_GET_COMPATIBILITY_REQUEST()
        {
            Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.Compatibility, 0xC0);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_COMPATIBILITY_RESPONSE
    {
        public byte ReportId;
        public LENOVO_SPECTRUM_OPERATION_TYPE Type;
        public byte Length;
        public byte Unknown1;
        public byte Compatibility;

        public bool IsCompatible => Compatibility == 0;
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_KEYCOUNT_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Parameter = 7;

        public LENOVO_SPECTRUM_GET_KEYCOUNT_REQUEST()
        {
            Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.KeyCount, 0xC0);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_KEYCOUNT_RESPONSE
    {
        public byte ReportId;
        public LENOVO_SPECTRUM_OPERATION_TYPE Type;
        public byte Length;
        public byte Unknown1;
        public byte Parameter;
        public byte Indexes;
        public byte KeysPerIndex;

        public bool IsExtended => Indexes == 0x09 && KeysPerIndex == 0x16;
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_KEYPAGE_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Parameter = 7;
        public byte Index;

        public LENOVO_SPECTRUM_GET_KEYPAGE_REQUEST(byte index)
        {
            Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.KeyPage, 0xC0);
            Index = index;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_KEYPAGE_RESPONSE
    {
        public byte ReportId;
        public LENOVO_SPECTRUM_OPERATION_TYPE Type;
        public byte Length;
        public byte Unknown1;
        public byte Parameter;
        public byte Index;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public LENOVO_SPECTRUM_KEYPAGE_ITEM[] Items;
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_BRIGHTNESS_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;

        public LENOVO_SPECTRUM_GET_BRIGHTNESS_REQUEST()
        {
            Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.GetBrightness, 0xC0);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_BRIGTHNESS_RESPONSE
    {
        public byte ReportId;
        public LENOVO_SPECTRUM_OPERATION_TYPE Type;
        public byte Length;
        public byte Unknown1;
        public byte Brightness;
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_SET_BRIGHTHNESS_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Brightness;

        public LENOVO_SPECTRUM_SET_BRIGHTHNESS_REQUEST(byte brightness)
        {
            Header = new(LENOVO_SPECTRUM_OPERATION_TYPE.Brightness, 0xC0);
            Brightness = brightness;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_PROFILE_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;

        public LENOVO_SPECTRUM_GET_PROFILE_REQUEST()
        {
            Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.Profile, 0xC0);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_PROFILE_RESPONSE
    {
        public byte ReportId;
        public LENOVO_SPECTRUM_OPERATION_TYPE Type;
        public byte Length;
        public byte Unknown1;
        public byte Profile;
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_SET_PROFILE_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Profile;

        public LENOVO_SPECTRUM_SET_PROFILE_REQUEST(byte profile)
        {
            Header = new(LENOVO_SPECTRUM_OPERATION_TYPE.ProfileChange, 0xC0);
            Profile = profile;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_SET_PROFILE_DEFAULT_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Profile;

        public LENOVO_SPECTRUM_SET_PROFILE_DEFAULT_REQUEST(byte profile)
        {
            Header = new(LENOVO_SPECTRUM_OPERATION_TYPE.ProfileDefault, 0xC0);
            Profile = profile;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_GET_EFFECT_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Profile;

        public LENOVO_SPECTRUM_GET_EFFECT_REQUEST(byte profile)
        {
            Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.Effect, 0xC0);
            Profile = profile;
        }
    }

    internal struct LENOVO_SPECTRUM_EFFECT_DESCRIPTION
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte Profile;
        public byte Unknown1 = 1;
        public byte Unknown2 = 1;
        public LENOVO_SPECTRUM_EFFECT[] Effects;

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
                    var key = br.ReadUInt16();
                    keyCodes.Add(key);
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
                foreach (var key in effect.Keys)
                {
                    bf.Write(key);
                }
            }

            var position = ms.Position;
            bf.Seek(2, SeekOrigin.Begin);
            bf.Write((byte)(position % 255));

            return ms.ToArray();
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 960)]
    internal struct LENOVO_SPECTRUM_AURORA_STARTSTOP_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public byte StartStop;
        public byte Profile;

        public LENOVO_SPECTRUM_AURORA_STARTSTOP_REQUEST(bool start, byte profile)
        {
            Header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.AuroraStartStop, 0xC0);
            StartStop = start ? (byte)1 : (byte)2;
            Profile = profile;
        }
    }

    internal struct LENOVO_SPECTRUM_AURORA_SEND_BITMAP_REQUEST
    {
        public LENOVO_SPECTRUM_HEADER Header;
        public LENOVO_SEPCTRUM_AURORA_ITEM[] Items;

        public LENOVO_SPECTRUM_AURORA_SEND_BITMAP_REQUEST(LENOVO_SEPCTRUM_AURORA_ITEM[] items)
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
    internal struct LENOVO_SPECTRUM_STATE_RESPONSE
    {
        public byte ReportId;
        public LENOVO_SPECTRUM_OPERATION_TYPE Type;
        public byte Length;
        public byte Unknown1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 190)]
        public LENOVO_SPECTRUM_KEY_STATE[] Data;
    }

    #endregion

}
