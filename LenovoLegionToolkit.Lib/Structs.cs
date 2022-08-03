using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public int FullChargeCapactiy { get; }
        public int CycleCount { get; }
        public double? BatteryTemperatureC { get; }
        public DateTime? ManufactureDate { get; }

        public BatteryInformation(
            bool isCharging,
            int batteryPercentage,
            int batteryLifeRemaining,
            int fullBatteryLifeRemaining,
            int dischargeRate,
            int estimateChargeRemaining,
            int designCapacity,
            int fullChargeCapactiy,
            int cycleCount,
            double? batteryTemperatureC,
            DateTime? manufactureDate)
        {
            IsCharging = isCharging;
            BatteryPercentage = batteryPercentage;
            BatteryLifeRemaining = batteryLifeRemaining;
            FullBatteryLifeRemaining = fullBatteryLifeRemaining;
            DischargeRate = dischargeRate;
            EstimateChargeRemaining = estimateChargeRemaining;
            DesignCapacity = designCapacity;
            FullChargeCapactiy = fullChargeCapactiy;
            CycleCount = cycleCount;
            BatteryTemperatureC = batteryTemperatureC;
            ManufactureDate = manufactureDate;
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

    public struct MachineInformation
    {
        public string Vendor { get; }
        public string MachineType { get; }
        public string Model { get; }
        public string SerialNumber { get; }
        public ModelYear ModelYear { get; }

        public MachineInformation(string vendor, string machineType, string model, string serialNumber, ModelYear modelYear)
        {
            Vendor = vendor;
            MachineType = machineType;
            Model = model;
            SerialNumber = serialNumber;
            ModelYear = modelYear;
        }
    }

    public struct Package
    {
        public string Description { get; }
        public string Version { get; }
        public string Category { get; }
        public string FileName { get; }
        public int FileSize { get; }
        public DateTime ReleaseDate { get; }
        public string? Readme { get; }
        public string FileLocation { get; }

        private string? _index = null;

        public string Index
        {
            get
            {
                if (_index is null)
                {
                    _index = new StringBuilder()
                        .Append(Description)
                        .Append(Version)
                        .Append(Category)
                        .Append(FileName)
                        .ToString();
                }
                return _index;
            }
        }

        public Package(string description, string version, string category, string fileName, int fileSize, DateTime releaseDate, string? readme, string fileLocation)
        {
            Description = description;
            Version = version;
            Category = category;
            FileName = fileName;
            FileSize = fileSize;
            ReleaseDate = releaseDate;
            Readme = readme;
            FileLocation = fileLocation;
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
