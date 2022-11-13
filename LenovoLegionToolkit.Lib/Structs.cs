using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LenovoLegionToolkit.Lib.Extensions;
using Newtonsoft.Json;
using Octokit;

namespace LenovoLegionToolkit.Lib
{
    public struct BatteryInformation
    {
        public bool IsCharging { get; init; }
        public int BatteryPercentage { get; init; }
        public DateTime? OnBatterySince { get; init; }
        public int BatteryLifeRemaining { get; init; }
        public int FullBatteryLifeRemaining { get; init; }
        public int DischargeRate { get; init; }
        public int EstimateChargeRemaining { get; init; }
        public int DesignCapacity { get; init; }
        public int FullChargeCapacity { get; init; }
        public int CycleCount { get; init; }
        public double? BatteryTemperatureC { get; init; }
        public DateTime? ManufactureDate { get; init; }
        public DateTime? FirstUseDate { get; init; }
    }

    public struct CPUBoostMode
    {
        public int Value { get; }
        public string Name { get; }

        public CPUBoostMode(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }

    public struct CPUBoostModeSettings
    {
        public PowerPlan PowerPlan { get; }
        public List<CPUBoostMode> CPUBoostModes { get; }
        public int ACSettingValue { get; }
        public int DCSettingValue { get; }

        public CPUBoostModeSettings(PowerPlan powerPlan, List<CPUBoostMode> cpuBoostModes, int acSettingValue, int dcSettingValue)
        {
            PowerPlan = powerPlan;
            CPUBoostModes = cpuBoostModes;
            ACSettingValue = acSettingValue;
            DCSettingValue = dcSettingValue;
        }
    }

    public struct DisplayAdvancedColorInfo
    {
        public bool AdvancedColorSupported { get; }
        public bool AdvancedColorEnabled { get; }
        public bool WideColorEnforced { get; }
        public bool AdvancedColorForceDisabled { get; }

        public DisplayAdvancedColorInfo(bool advancedColorSupported, bool advancedColorEnabled, bool wideColorEnforced, bool advancedColorForceDisabled)
        {
            AdvancedColorSupported = advancedColorSupported;
            AdvancedColorEnabled = advancedColorEnabled;
            WideColorEnforced = wideColorEnforced;
            AdvancedColorForceDisabled = advancedColorForceDisabled;
        }
    }

    public struct FanTableData
    {
        public byte FanId { get; init; }
        public byte SensorId { get; init; }
        public ushort[] FanSpeeds { get; init; }
        public ushort[] Temps { get; init; }

        public FanTableType Type => (FanId, SensorId) switch
        {
            (0, 3) => FanTableType.CPU,
            (1, 4) => FanTableType.GPU,
            (0, 0) => FanTableType.CPUSensor,
            _ => FanTableType.Unknown
        };
    }

    public struct FanTable
    {
        public byte FSTM { get; set; }
        public byte FSID { get; set; }
        public uint FSTL { get; set; }
        public ushort FSS0 { get; set; }
        public ushort FSS1 { get; set; }
        public ushort FSS2 { get; set; }
        public ushort FSS3 { get; set; }
        public ushort FSS4 { get; set; }
        public ushort FSS5 { get; set; }
        public ushort FSS6 { get; set; }
        public ushort FSS7 { get; set; }
        public ushort FSS8 { get; set; }
        public ushort FSS9 { get; set; }

        public FanTable(ushort[] fanTable)
        {
            if (fanTable.Length != 10)
                throw new ArgumentException("Length must be 10.", nameof(fanTable));

            for (var i = 0; i < fanTable.Length; i++)
                fanTable[i] = Math.Clamp(fanTable[i], (ushort)1, (ushort)10u);

            FSTM = 1;
            FSID = 0;
            FSTL = 0;
            FSS0 = fanTable[0];
            FSS1 = fanTable[1];
            FSS2 = fanTable[2];
            FSS3 = fanTable[3];
            FSS4 = fanTable[4];
            FSS5 = fanTable[5];
            FSS6 = fanTable[6];
            FSS7 = fanTable[7];
            FSS8 = fanTable[8];
            FSS9 = fanTable[9];
        }

        public ushort[] GetTable() => new[] { FSS0, FSS1, FSS2, FSS3, FSS4, FSS5, FSS6, FSS7, FSS8, FSS9 };

        public byte[] GetBytes()
        {
            using var ms = new MemoryStream(new byte[64]);
            ms.WriteByte(FSTM);
            ms.WriteByte(FSID);
            ms.Write(BitConverter.GetBytes(FSTL));
            ms.Write(BitConverter.GetBytes(FSS0));
            ms.Write(BitConverter.GetBytes(FSS1));
            ms.Write(BitConverter.GetBytes(FSS2));
            ms.Write(BitConverter.GetBytes(FSS3));
            ms.Write(BitConverter.GetBytes(FSS4));
            ms.Write(BitConverter.GetBytes(FSS5));
            ms.Write(BitConverter.GetBytes(FSS6));
            ms.Write(BitConverter.GetBytes(FSS7));
            ms.Write(BitConverter.GetBytes(FSS8));
            ms.Write(BitConverter.GetBytes(FSS9));
            return ms.ToArray();
        }
    }

    public struct FanTableInfo
    {
        public FanTableData[] Data { get; }
        public FanTable Table { get; }

        public FanTableInfo(FanTableData[] data, FanTable table)
        {
            Data = data;
            Table = table;
        }
    }

    public struct HardwareId
    {
        public string Vendor { get; private init; }
        public string Device { get; private init; }
        public string SubSystem { get; private init; }

        public static HardwareId FromDGPUHWId(string? gpuHwId)
        {
            try
            {
                if (gpuHwId is null)
                    return default;

                string? vendor = null;
                string? device = null;
                string? subsystem = null;

                foreach (var subPath in gpuHwId.Split("&"))
                {
                    var subSubPaths = subPath.Split("_");
                    var type = subSubPaths[0];
                    var value = subSubPaths[1].ToUpperInvariant();

                    if (type.Equals("pciven", StringComparison.InvariantCultureIgnoreCase))
                        vendor = value;
                    if (type.Equals("dev", StringComparison.InvariantCultureIgnoreCase))
                        device = value;
                    if (type.Equals("subsys", StringComparison.InvariantCultureIgnoreCase))
                        subsystem = value;
                }

                if (vendor is null || device is null || subsystem is null)
                    return default;

                return new HardwareId { Vendor = vendor, Device = device, SubSystem = subsystem };
            }
            catch
            {
                return default;
            }
        }

        public static HardwareId FromDevicePath(string devicePath)
        {
            try
            {
                var path = devicePath;
                path = path[(path.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase) + 1)..];
                path = path[..path.LastIndexOf("#", StringComparison.InvariantCultureIgnoreCase)];
                path = path[..path.LastIndexOf("#", StringComparison.InvariantCultureIgnoreCase)];
                path = path.Replace("#", "");

                string? vendor = null;
                string? device = null;
                string? subsystem = null;

                foreach (var subPath in path.Split("&"))
                {
                    var subSubPaths = subPath.Split("_");

                    if (subSubPaths.Length != 2)
                        continue;

                    var type = subSubPaths[0];
                    var value = subSubPaths[1].ToUpperInvariant();

                    if (type.Equals("pciven", StringComparison.InvariantCultureIgnoreCase))
                        vendor = value;
                    if (type.Equals("dev", StringComparison.InvariantCultureIgnoreCase))
                        device = value;
                    if (type.Equals("subsys", StringComparison.InvariantCultureIgnoreCase))
                        subsystem = value;
                }

                if (vendor is null || device is null || subsystem is null)
                    return default;

                return new HardwareId { Vendor = vendor, Device = device, SubSystem = subsystem };
            }
            catch
            {
                return default;
            }
        }

        public static bool operator ==(HardwareId left, HardwareId right) => left.Equals(right);

        public static bool operator !=(HardwareId left, HardwareId right) => !left.Equals(right);

        public override bool Equals(object? obj) => obj is HardwareId other && Vendor == other.Vendor && Device == other.Device && SubSystem == other.SubSystem;

        public override int GetHashCode() => HashCode.Combine(Vendor, Device, SubSystem);
    }

    public struct MachineInformation
    {
        public struct CompatibilityProperties
        {
            public bool SupportsGodMode { get; init; }
            public bool SupportsACDetection { get; init; }
            public bool SupportsExtendedHybridMode { get; init; }
            public bool SupportsIntelligentSubMode { get; init; }
            public bool HasPerformanceModeSwitchingBug { get; init; }
        }

        public string Vendor { get; init; }
        public string MachineType { get; init; }
        public string Model { get; init; }
        public string SerialNumber { get; init; }
        public string BIOSVersion { get; init; }
        public CompatibilityProperties Properties { get; init; }
    }

    public struct Package
    {
        public string Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string Version { get; init; }
        public string Category { get; init; }
        public string FileName { get; init; }
        public string FileSize { get; init; }
        public DateTime ReleaseDate { get; init; }
        public string? Readme { get; init; }
        public string FileLocation { get; init; }

        private string? _index;

        public string Index
        {
            get
            {
                _index ??= new StringBuilder()
                        .Append(Title)
                        .Append(Description)
                        .Append(Version)
                        .Append(Category)
                        .Append(FileName)
                        .ToString();
                return _index;
            }
        }
    }

    public struct Notification
    {
        public NotificationType Type { get; }

        public NotificationDuration Duration { get; }

        public object[] Args { get; }

        public Notification(NotificationType type, NotificationDuration duration, params object[] args)
        {
            Type = type;
            Duration = duration;
            Args = args;
        }
    }

    public struct PowerPlan
    {
        public string InstanceID { get; }
        public string Name { get; }
        public bool IsActive { get; }
        public string Guid => InstanceID.Split("\\").Last().Replace("{", "").Replace("}", "");

        public PowerPlan(string instanceID, string name, bool isActive)
        {
            InstanceID = instanceID;
            Name = name;
            IsActive = isActive;
        }

        public override string ToString() => Name;
    }

    public struct ProcessEventInfo
    {
        public ProcessEventInfoType Type { get; }

        public ProcessInfo Process { get; }

        public ProcessEventInfo(ProcessEventInfoType type, ProcessInfo process)
        {
            Type = type;
            Process = process;
        }
    }

    public struct ProcessInfo : IComparable
    {
        public static ProcessInfo FromPath(string path) => new(Path.GetFileNameWithoutExtension(path), path);

        public string Name { get; }

        public string? ExecutablePath { get; }

        [JsonConstructor]
        public ProcessInfo(string name, string? executablePath)
        {
            Name = name;
            ExecutablePath = executablePath;
        }

        #region Equality

        public int CompareTo(object? obj)
        {
            var other = obj is null ? default : (ProcessInfo)obj;
            var result = string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
            return result != 0 ? result : string.Compare(ExecutablePath, other.ExecutablePath, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object? obj) => obj is ProcessInfo info && Name == info.Name && ExecutablePath == info.ExecutablePath;

        public override int GetHashCode() => HashCode.Combine(Name, ExecutablePath);

        public static bool operator ==(ProcessInfo left, ProcessInfo right) => left.Equals(right);

        public static bool operator !=(ProcessInfo left, ProcessInfo right) => !(left == right);

        public static bool operator <(ProcessInfo left, ProcessInfo right) => left.CompareTo(right) < 0;

        public static bool operator <=(ProcessInfo left, ProcessInfo right) => left.CompareTo(right) <= 0;

        public static bool operator >(ProcessInfo left, ProcessInfo right) => left.CompareTo(right) > 0;

        public static bool operator >=(ProcessInfo left, ProcessInfo right) => left.CompareTo(right) >= 0;

        #endregion
    }

    public struct RGBColor
    {
        public byte R { get; } = 255;
        public byte G { get; } = 255;
        public byte B { get; } = 255;

        [JsonConstructor]
        public RGBColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public struct RGBKeyboardBacklightSettings
    {
        public RGBKeyboardEffect Effect { get; } = RGBKeyboardEffect.Static;
        public RBGKeyboardSpeed Speed { get; } = RBGKeyboardSpeed.Slowest;
        public RGBKeyboardBrightness Brightness { get; } = RGBKeyboardBrightness.Low;
        public RGBColor Zone1 { get; } = new();
        public RGBColor Zone2 { get; } = new();
        public RGBColor Zone3 { get; } = new();
        public RGBColor Zone4 { get; } = new();

        [JsonConstructor]
        public RGBKeyboardBacklightSettings(
            RGBKeyboardEffect effect,
            RBGKeyboardSpeed speed,
            RGBKeyboardBrightness brightness,
            RGBColor zone1,
            RGBColor zone2,
            RGBColor zone3,
            RGBColor zone4)
        {
            Effect = effect;
            Speed = speed;
            Brightness = brightness;
            Zone1 = zone1;
            Zone2 = zone2;
            Zone3 = zone3;
            Zone4 = zone4;
        }

        #region Equality

        public override bool Equals(object? obj)
        {
            return obj is RGBKeyboardBacklightSettings settings &&
                   Effect == settings.Effect &&
                   Speed == settings.Speed &&
                   Brightness == settings.Brightness &&
                   Zone1.Equals(settings.Zone1) &&
                   Zone2.Equals(settings.Zone2) &&
                   Zone3.Equals(settings.Zone3) &&
                   Zone4.Equals(settings.Zone4);
        }

        public override int GetHashCode() => HashCode.Combine(Effect, Speed, Brightness, Zone1, Zone2, Zone3, Zone4);

        public static bool operator ==(RGBKeyboardBacklightSettings left, RGBKeyboardBacklightSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RGBKeyboardBacklightSettings left, RGBKeyboardBacklightSettings right)
        {
            return !(left == right);
        }

        #endregion

    }

    public struct RGBKeyboardBacklightState
    {
        public RGBKeyboardBacklightPreset SelectedPreset { get; }
        public Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightSettings> Presets { get; }

        [JsonConstructor]
        public RGBKeyboardBacklightState(RGBKeyboardBacklightPreset selectedPreset, Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightSettings> presets)
        {
            SelectedPreset = selectedPreset;
            Presets = presets;
        }
    }

    public struct RefreshRate : IDisplayName, IEquatable<RefreshRate>
    {
        public int Frequency { get; }

        [JsonIgnore]
        public string DisplayName => $"{Frequency} Hz";

        [JsonConstructor]
        public RefreshRate(int frequency)
        {
            Frequency = frequency;
        }

        #region Equality

        public override bool Equals(object? obj) => obj is RefreshRate rate && Equals(rate);

        public bool Equals(RefreshRate other) => Frequency == other.Frequency;

        public override int GetHashCode() => HashCode.Combine(Frequency);

        public static bool operator ==(RefreshRate left, RefreshRate right) => left.Equals(right);

        public static bool operator !=(RefreshRate left, RefreshRate right) => !(left == right);

        #endregion
    }

    public struct StepperValue
    {
        public int Value { get; }
        public int Min { get; }
        public int Max { get; }
        public int Step { get; }

        public StepperValue(int value, int min, int max, int step)
        {
            Value = MathExtensions.RoundNearest(value, step);
            Min = min;
            Max = max;
            Step = step;
        }

        public StepperValue WithValue(int value) => new(value, Min, Max, Step);
    }

    public struct Time
    {
        public int Hour { get; init; }
        public int Minute { get; init; }

        #region Equality

        public override bool Equals(object? obj) => obj is Time time && Hour == time.Hour && Minute == time.Minute;

        public override int GetHashCode() => HashCode.Combine(Hour, Minute);

        public static bool operator ==(Time left, Time right) => left.Equals(right);

        public static bool operator !=(Time left, Time right) => !(left == right);

        #endregion
    }

    public struct Update
    {
        public Version Version { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTimeOffset Date { get; }
        public string? Url { get; }

        public Update(Release release)
        {
            Version = Version.Parse(release.TagName);
            Title = release.Name;
            Description = release.Body;
            Date = release.PublishedAt ?? release.CreatedAt;
            Url = release.Assets.Where(ra => ra.Name.EndsWith("setup.exe", StringComparison.InvariantCultureIgnoreCase)).Select(ra => ra.BrowserDownloadUrl).FirstOrDefault();
        }
    }

    public struct WarrantyInfo
    {
        public string? Status { get; init; }
        public DateTime? Start { get; init; }
        public DateTime? End { get; init; }
        public Uri? Link { get; init; }
    }

    public struct WindowSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public WindowSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
