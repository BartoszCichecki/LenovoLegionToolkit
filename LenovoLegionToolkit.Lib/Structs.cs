using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;
using Octokit;

namespace LenovoLegionToolkit.Lib
{
    public struct BatteryInformation
    {
        public bool IsCharging { get; }
        public int BatteryPercentage { get; }
        public int BatteryLifeRemaining { get; }
        public int FullBatteryLifeRemaining { get; }
        public int DischargeRate { get; }
        public int EstimateChargeRemaining { get; }
        public int DesignCapacity { get; }
        public int FullChargeCapacity { get; }
        public int CycleCount { get; }
        public double? BatteryTemperatureC { get; }
        public DateTime? ManufactureDate { get; }
        public DateTime? FirstUseDate { get; }

        public BatteryInformation(
            bool isCharging,
            int batteryPercentage,
            int batteryLifeRemaining,
            int fullBatteryLifeRemaining,
            int dischargeRate,
            int estimateChargeRemaining,
            int designCapacity,
            int fullChargeCapacity,
            int cycleCount,
            double? batteryTemperatureC,
            DateTime? manufactureDate,
            DateTime? firstUseDate)
        {
            IsCharging = isCharging;
            BatteryPercentage = batteryPercentage;
            BatteryLifeRemaining = batteryLifeRemaining;
            FullBatteryLifeRemaining = fullBatteryLifeRemaining;
            DischargeRate = dischargeRate;
            EstimateChargeRemaining = estimateChargeRemaining;
            DesignCapacity = designCapacity;
            FullChargeCapacity = fullChargeCapacity;
            CycleCount = cycleCount;
            BatteryTemperatureC = batteryTemperatureC;
            ManufactureDate = manufactureDate;
            FirstUseDate = firstUseDate;
        }
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

    public struct MachineInformation
    {
        public struct CompatibiltyProperties
        {
            public bool ShouldFlipFnLock { get; init; }
            public bool SupportsGodMode { get; init; }
            public bool SupportsACDetection { get; init; }
            public bool SupportsExtendedHybridMode { get; init; }
        }

        public string Vendor { get; init; }
        public string MachineType { get; init; }
        public string Model { get; init; }
        public string SerialNumber { get; init; }
        public string BIOSVersion { get; init; }
        public ModelYear ModelYear { get; init; }
        public CompatibiltyProperties Properties { get; init; }
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
        public NotificationIcon Icon { get; }

        public string Text { get; }

        public NotificationDuration Duration { get; }

        public Notification(NotificationIcon icon, string text, NotificationDuration duration)
        {
            Icon = icon;
            Text = text;
            Duration = duration;
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

        public string ExecutablePath { get; }

        [JsonConstructor]
        public ProcessInfo(string name, string executablePath)
        {
            Name = name;
            ExecutablePath = executablePath;
        }

        #region Equality

        public int CompareTo(object? obj)
        {
            var other = obj is null ? default : (ProcessInfo)obj;

            var result = Name.CompareTo(other.Name);
            if (result != 0)
                return result;

            return ExecutablePath.CompareTo(other.ExecutablePath);
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
        public bool LinkOnly { get; init; }
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
