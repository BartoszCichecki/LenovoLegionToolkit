using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using LenovoLegionToolkit.Lib.Extensions;
using Newtonsoft.Json;
using Octokit;

namespace LenovoLegionToolkit.Lib;

public readonly struct BatteryInformation
{
    public bool IsCharging { get; init; }
    public int BatteryPercentage { get; init; }
    public int BatteryLifeRemaining { get; init; }
    public int FullBatteryLifeRemaining { get; init; }
    public int DischargeRate { get; init; }
    public int EstimateChargeRemaining { get; init; }
    public int DesignCapacity { get; init; }
    public int FullChargeCapacity { get; init; }
    public int CycleCount { get; init; }
    public bool IsLowBattery { get; init; }
    public double? BatteryTemperatureC { get; init; }
    public DateTime? ManufactureDate { get; init; }
    public DateTime? FirstUseDate { get; init; }
}

public readonly struct BiosVersion
{
    public string Prefix { get; }
    public int? Version { get; }

    public BiosVersion(string prefix, int? version)
    {
        Prefix = prefix;
        Version = version;
    }

    public bool IsHigherOrEqualThan(BiosVersion other)
    {
        if (!Prefix.Equals(other.Prefix, StringComparison.InvariantCultureIgnoreCase))
            return false;

        if (Version is null || other.Version is null)
            return true;

        return Version >= other.Version;
    }

    public bool IsLowerThan(BiosVersion other)
    {
        if (!Prefix.Equals(other.Prefix, StringComparison.InvariantCultureIgnoreCase))
            return false;

        if (Version is null || other.Version is null)
            return true;

        return Version < other.Version;
    }

    public override string ToString() => $"{nameof(Prefix)}: {Prefix}, {nameof(Version)}: {Version}";
}

public readonly struct Brightness
{
    public byte Value { get; private init; }

    public static implicit operator Brightness(byte value) => new() { Value = value };
}

public readonly struct DiscreteCapability
{
    public CapabilityID Id { get; }
    public int Value { get; }

    public DiscreteCapability(CapabilityID id, int value)
    {
        Id = id;
        Value = value;
    }
}

public readonly struct DisplayAdvancedColorInfo
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

public readonly struct DriverInfo
{
    public string DeviceId { get; init; }
    public string HardwareId { get; init; }
    public Version? Version { get; init; }
    public DateTime? Date { get; init; }
}

public readonly struct FanTableData
{
    public FanTableType Type { get; init; }
    public byte FanId { get; init; }
    public byte SensorId { get; init; }
    public ushort[] FanSpeeds { get; init; }
    public ushort[] Temps { get; init; }

    public override string ToString() =>
        $"{nameof(Type)}: {Type}," +
        $" {nameof(FanId)}: {FanId}," +
        $" {nameof(SensorId)}: {SensorId}," +
        $" {nameof(FanSpeeds)}: [{string.Join(", ", FanSpeeds)}]," +
        $" {nameof(Temps)}: [{string.Join(", ", Temps)}]," +
        $" {nameof(Type)}: {Type}";
}

public readonly struct FanTable
{
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public byte FSTM { get; init; }
    public byte FSID { get; init; }
    public uint FSTL { get; init; }
    public ushort FSS0 { get; init; }
    public ushort FSS1 { get; init; }
    public ushort FSS2 { get; init; }
    public ushort FSS3 { get; init; }
    public ushort FSS4 { get; init; }
    public ushort FSS5 { get; init; }
    public ushort FSS6 { get; init; }
    public ushort FSS7 { get; init; }
    public ushort FSS8 { get; init; }
    public ushort FSS9 { get; init; }

    // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore IdentifierTypo
    // ReSharper restore InconsistentNaming

    public FanTable(ushort[] fanTable)
    {
        if (fanTable.Length != 10)
            // ReSharper disable once LocalizableElement
            throw new ArgumentException("Fan table length must be 10.", nameof(fanTable));

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

    public override string ToString() =>
        $"{nameof(FSTM)}: {FSTM}," +
        $" {nameof(FSID)}: {FSID}," +
        $" {nameof(FSTL)}: {FSTL}," +
        $" {nameof(FSS0)}: {FSS0}," +
        $" {nameof(FSS1)}: {FSS1}," +
        $" {nameof(FSS2)}: {FSS2}," +
        $" {nameof(FSS3)}: {FSS3}," +
        $" {nameof(FSS4)}: {FSS4}," +
        $" {nameof(FSS5)}: {FSS5}," +
        $" {nameof(FSS6)}: {FSS6}," +
        $" {nameof(FSS7)}: {FSS7}," +
        $" {nameof(FSS8)}: {FSS8}," +
        $" {nameof(FSS9)}: {FSS9}";
}

public readonly struct FanTableInfo
{
    public FanTableData[] Data { get; }
    public FanTable Table { get; }

    public FanTableInfo(FanTableData[] data, FanTable table)
    {
        Data = data;
        Table = table;
    }

    public override string ToString() =>
        $"{nameof(Data)}: [{string.Join(", ", Data)}]," +
        $" {nameof(Table)}: {Table}";
}

public readonly struct GPUOverclockInfo
{
    public static readonly GPUOverclockInfo Zero = new();

    public int CoreDeltaMhz { get; init; }
    public int MemoryDeltaMhz { get; init; }


    #region Equality

    public override bool Equals(object? obj)
    {
        return obj is GPUOverclockInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CoreDeltaMhz, MemoryDeltaMhz);
    }

    public static bool operator ==(GPUOverclockInfo left, GPUOverclockInfo right) => left.Equals(right);

    public static bool operator !=(GPUOverclockInfo left, GPUOverclockInfo right) => !left.Equals(right);

    #endregion

    public override string ToString() => $"{nameof(CoreDeltaMhz)}: {CoreDeltaMhz}, {nameof(MemoryDeltaMhz)}: {MemoryDeltaMhz}";

}

public readonly struct GodModeDefaults
{
    public int? CPULongTermPowerLimit { get; init; }
    public int? CPUShortTermPowerLimit { get; init; }
    public int? CPUPeakPowerLimit { get; init; }
    public int? CPUCrossLoadingPowerLimit { get; init; }
    public int? CPUPL1Tau { get; init; }
    public int? APUsPPTPowerLimit { get; init; }
    public int? CPUTemperatureLimit { get; init; }
    public int? GPUPowerBoost { get; init; }
    public int? GPUConfigurableTGP { get; init; }
    public int? GPUTemperatureLimit { get; init; }
    public int? GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline { get; init; }
    public int? GPUToCPUDynamicBoost { get; init; }
    public FanTable? FanTable { get; init; }
    public bool? FanFullSpeed { get; init; }

    public override string ToString() =>
        $"{nameof(CPULongTermPowerLimit)}: {CPULongTermPowerLimit}," +
        $" {nameof(CPUShortTermPowerLimit)}: {CPUShortTermPowerLimit}," +
        $" {nameof(CPUPeakPowerLimit)}: {CPUPeakPowerLimit}," +
        $" {nameof(CPUCrossLoadingPowerLimit)}: {CPUCrossLoadingPowerLimit}," +
        $" {nameof(CPUPL1Tau)}: {CPUPL1Tau}," +
        $" {nameof(APUsPPTPowerLimit)}: {APUsPPTPowerLimit}," +
        $" {nameof(CPUTemperatureLimit)}: {CPUTemperatureLimit}," +
        $" {nameof(GPUPowerBoost)}: {GPUPowerBoost}," +
        $" {nameof(GPUConfigurableTGP)}: {GPUConfigurableTGP}," +
        $" {nameof(GPUTemperatureLimit)}: {GPUTemperatureLimit}," +
        $" {nameof(GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline)}: {GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline}," +
        $" {nameof(GPUToCPUDynamicBoost)}: {GPUToCPUDynamicBoost}," +
        $" {nameof(FanTable)}: {FanTable}," +
        $" {nameof(FanFullSpeed)}: {FanFullSpeed}";
}

public readonly struct GodModeState
{
    public Guid ActivePresetId { get; init; }
    public ReadOnlyDictionary<Guid, GodModePreset> Presets { get; init; }
}

public readonly struct GodModePreset
{
    public string Name { get; init; }
    public StepperValue? CPULongTermPowerLimit { get; init; }
    public StepperValue? CPUShortTermPowerLimit { get; init; }
    public StepperValue? CPUPeakPowerLimit { get; init; }
    public StepperValue? CPUCrossLoadingPowerLimit { get; init; }
    public StepperValue? CPUPL1Tau { get; init; }
    public StepperValue? APUsPPTPowerLimit { get; init; }
    public StepperValue? CPUTemperatureLimit { get; init; }
    public StepperValue? GPUPowerBoost { get; init; }
    public StepperValue? GPUConfigurableTGP { get; init; }
    public StepperValue? GPUTemperatureLimit { get; init; }
    public StepperValue? GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline { get; init; }
    public StepperValue? GPUToCPUDynamicBoost { get; init; }
    public FanTableInfo? FanTableInfo { get; init; }
    public bool? FanFullSpeed { get; init; }
    public int? MinValueOffset { get; init; }
    public int? MaxValueOffset { get; init; }

    public override string ToString() =>
        $"{nameof(Name)}: {Name}," +
        $" {nameof(CPULongTermPowerLimit)}: {CPULongTermPowerLimit}," +
        $" {nameof(CPUShortTermPowerLimit)}: {CPUShortTermPowerLimit}," +
        $" {nameof(CPUPeakPowerLimit)}: {CPUPeakPowerLimit}," +
        $" {nameof(CPUCrossLoadingPowerLimit)}: {CPUCrossLoadingPowerLimit}," +
        $" {nameof(CPUPL1Tau)}: {CPUPL1Tau}," +
        $" {nameof(APUsPPTPowerLimit)}: {APUsPPTPowerLimit}," +
        $" {nameof(CPUTemperatureLimit)}: {CPUTemperatureLimit}," +
        $" {nameof(GPUPowerBoost)}: {GPUPowerBoost}," +
        $" {nameof(GPUConfigurableTGP)}: {GPUConfigurableTGP}," +
        $" {nameof(GPUTemperatureLimit)}: {GPUTemperatureLimit}," +
        $" {nameof(GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline)}: {GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline}," +
        $" {nameof(GPUToCPUDynamicBoost)}: {GPUToCPUDynamicBoost}," +
        $" {nameof(FanTableInfo)}: {FanTableInfo}," +
        $" {nameof(FanFullSpeed)}: {FanFullSpeed}," +
        $" {nameof(MinValueOffset)}: {MinValueOffset}," +
        $" {nameof(MaxValueOffset)}: {MaxValueOffset}";
}

public readonly struct GPUStatus
{
    public GPUState State { get; }
    public string? PerformanceState { get; }
    public List<Process> Processes { get; }
    public int ProcessCount => Processes.Count;

    public GPUStatus(GPUState state, string? performanceState, List<Process> processes)
    {
        State = state;
        PerformanceState = performanceState;
        Processes = processes;
    }
}

public readonly struct HardwareId
{
    public static readonly HardwareId Empty = new();

    public string Vendor { get; init; }
    public string Device { get; init; }

    #region Equality

    public override bool Equals(object? obj)
    {
        if (obj is not HardwareId other)
            return false;

        if (!Vendor.Equals(other.Vendor, StringComparison.InvariantCultureIgnoreCase))
            return false;

        if (!Device.Equals(other.Device, StringComparison.InvariantCultureIgnoreCase))
            return false;

        return true;
    }

    public override int GetHashCode() => HashCode.Combine(Vendor, Device);

    public static bool operator ==(HardwareId left, HardwareId right) => left.Equals(right);

    public static bool operator !=(HardwareId left, HardwareId right) => !left.Equals(right);

    #endregion
}

public readonly struct MachineInformation
{
    public readonly struct FeatureData
    {
        public static readonly FeatureData Unknown = new() { Source = SourceType.Unknown };

        public enum SourceType
        {
            Unknown,
            Flags,
            CapabilityData
        }

        public SourceType Source { get; init; }
        public bool IGPUMode { get; init; }
        public bool AIChip { get; init; }
        public bool FlipToStart { get; init; }
        public bool NvidiaGPUDynamicDisplaySwitching { get; init; }
        public bool InstantBootAc { get; init; }
        public bool InstantBootUsbPowerDelivery { get; init; }
        public bool AMDSmartShiftMode { get; init; }
        public bool AMDSkinTemperatureTracking { get; init; }
    }

    public readonly struct PropertyData
    {
        public bool SupportsGodMode => SupportsGodModeV1 || SupportsGodModeV2;

        public (bool status, bool connectivity) SupportsAlwaysOnAc { get; init; }
        public bool SupportsGodModeV1 { get; init; }
        public bool SupportsGodModeV2 { get; init; }
        public bool SupportsGSync { get; init; }
        public bool SupportsIGPUMode { get; init; }
        public bool SupportsAIMode { get; init; }
        public bool SupportBootLogoChange { get; init; }
        public bool HasQuietToPerformanceModeSwitchingBug { get; init; }
        public bool HasGodModeToOtherModeSwitchingBug { get; init; }
        public bool IsExcludedFromLenovoLighting { get; init; }
        public bool IsExcludedFromPanelLogoLenovoLighting { get; init; }
        public bool HasAlternativeFullSpectrumLayout { get; init; }
    }

    public string Vendor { get; init; }
    public string MachineType { get; init; }
    public string Model { get; init; }
    public string SerialNumber { get; init; }
    public BiosVersion? BiosVersion { get; init; }
    public string? BiosVersionRaw { get; init; }
    public PowerModeState[] SupportedPowerModes { get; init; }
    public int SmartFanVersion { get; init; }
    public int LegionZoneVersion { get; init; }
    public FeatureData Features { get; init; }
    public PropertyData Properties { get; init; }
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
    public string? FileCrc { get; init; }
    public DateTime ReleaseDate { get; init; }
    public string? Readme { get; init; }
    public string FileLocation { get; init; }
    public bool IsUpdate { get; init; }
    public RebootType Reboot { get; init; }

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

public readonly struct Notification
{
    public NotificationType Type { get; }

    public object[] Args { get; }

    public Notification(NotificationType type, params object[] args)
    {
        Type = type;
        Args = args;
    }

    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}," +
               $" {nameof(Args)}: [{string.Join(", ", Args)}]";
    }
}

public readonly struct PowerPlan
{
    public Guid Guid { get; }
    public string Name { get; }
    public bool IsActive { get; }

    public PowerPlan(Guid guid, string name, bool isActive)
    {
        Guid = guid;
        Name = name;
        IsActive = isActive;
    }

    public override string ToString() => Name;
}

public readonly struct ProcessInfo : IComparable
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

    public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(ExecutablePath)}: {ExecutablePath}";

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

public readonly struct RangeCapability
{
    public CapabilityID Id { get; }
    public int DefaultValue { get; }
    public int Min { get; }
    public int Max { get; }
    public int Step { get; }

    public RangeCapability(CapabilityID id, int defaultValue, int min, int max, int step)
    {
        Id = id;
        DefaultValue = defaultValue;
        Min = min;
        Max = max;
        Step = step;
    }
}

public readonly struct RGBColor
{
    public static readonly RGBColor Black = new(0, 0, 0);
    public static readonly RGBColor White = new(255, 255, 255);

    public byte R { get; }
    public byte G { get; }
    public byte B { get; }

    [JsonConstructor]
    public RGBColor(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    #region Equality

    public override bool Equals(object? obj)
    {
        return obj is RGBColor color && R == color.R && G == color.G && B == color.B;
    }

    public override int GetHashCode() => (R, G, B).GetHashCode();

    public static bool operator ==(RGBColor left, RGBColor right) => left.Equals(right);

    public static bool operator !=(RGBColor left, RGBColor right) => !left.Equals(right);

    #endregion

    public override string ToString() => $"{nameof(R)}: {R}, {nameof(G)}: {G}, {nameof(B)}: {B}";
}

public readonly struct RGBKeyboardBacklightBacklightPresetDescription
{
    public RGBKeyboardBacklightEffect Effect { get; } = RGBKeyboardBacklightEffect.Static;
    public RGBKeyboardBacklightSpeed Speed { get; } = RGBKeyboardBacklightSpeed.Slowest;
    public RGBKeyboardBacklightBrightness Brightness { get; } = RGBKeyboardBacklightBrightness.Low;
    public RGBColor Zone1 { get; } = RGBColor.White;
    public RGBColor Zone2 { get; } = RGBColor.White;
    public RGBColor Zone3 { get; } = RGBColor.White;
    public RGBColor Zone4 { get; } = RGBColor.White;

    [JsonConstructor]
    public RGBKeyboardBacklightBacklightPresetDescription(
        RGBKeyboardBacklightEffect effect,
        RGBKeyboardBacklightSpeed speed,
        RGBKeyboardBacklightBrightness brightness,
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
        return obj is RGBKeyboardBacklightBacklightPresetDescription settings &&
               Effect == settings.Effect &&
               Speed == settings.Speed &&
               Brightness == settings.Brightness &&
               Zone1.Equals(settings.Zone1) &&
               Zone2.Equals(settings.Zone2) &&
               Zone3.Equals(settings.Zone3) &&
               Zone4.Equals(settings.Zone4);
    }

    public override int GetHashCode() => HashCode.Combine(Effect, Speed, Brightness, Zone1, Zone2, Zone3, Zone4);

    public static bool operator ==(RGBKeyboardBacklightBacklightPresetDescription left, RGBKeyboardBacklightBacklightPresetDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RGBKeyboardBacklightBacklightPresetDescription left, RGBKeyboardBacklightBacklightPresetDescription right)
    {
        return !(left == right);
    }

    #endregion

    public override string ToString() =>
        $"{nameof(Effect)}: {Effect}," +
        $" {nameof(Speed)}: {Speed}," +
        $" {nameof(Brightness)}: {Brightness}," +
        $" {nameof(Zone1)}: {Zone1}," +
        $" {nameof(Zone2)}: {Zone2}," +
        $" {nameof(Zone3)}: {Zone3}," +
        $" {nameof(Zone4)}: {Zone4}";
}

public readonly struct RGBKeyboardBacklightState
{
    public RGBKeyboardBacklightPreset SelectedPreset { get; }
    public Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightBacklightPresetDescription> Presets { get; }

    [JsonConstructor]
    public RGBKeyboardBacklightState(RGBKeyboardBacklightPreset selectedPreset, Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightBacklightPresetDescription> presets)
    {
        SelectedPreset = selectedPreset;
        Presets = presets;
    }
}

public readonly struct SensorData
{
    public static readonly SensorData Empty = new()
    {
        Utilization = -1,
        CoreClock = -1,
        MaxCoreClock = -1,
        MemoryClock = -1,
        MaxMemoryClock = -1,
        Temperature = -1,
        MaxTemperature = -1,
        FanSpeed = -1,
        MaxFanSpeed = -1
    };

    public int Utilization { get; init; }
    public int MaxUtilization { get; init; }
    public int CoreClock { get; init; }
    public int MaxCoreClock { get; init; }
    public int MemoryClock { get; init; }
    public int MaxMemoryClock { get; init; }
    public int Temperature { get; init; }
    public int MaxTemperature { get; init; }
    public int FanSpeed { get; init; }
    public int MaxFanSpeed { get; init; }

    public override string ToString() =>
        $"{nameof(Utilization)}: {Utilization}," +
        $" {nameof(MaxUtilization)}: {MaxUtilization}," +
        $" {nameof(CoreClock)}: {CoreClock}," +
        $" {nameof(MaxCoreClock)}: {MaxCoreClock}," +
        $" {nameof(MemoryClock)}: {MemoryClock}," +
        $" {nameof(MaxMemoryClock)}: {MaxMemoryClock}," +
        $" {nameof(Temperature)}: {Temperature}," +
        $" {nameof(MaxTemperature)}: {MaxTemperature}," +
        $" {nameof(FanSpeed)}: {FanSpeed}," +
        $" {nameof(MaxFanSpeed)}: {MaxFanSpeed}";
}

public readonly struct SensorsData
{
    public static readonly SensorsData Empty = new() { CPU = SensorData.Empty, GPU = SensorData.Empty };

    public SensorData CPU { get; init; }
    public SensorData GPU { get; init; }

    public override string ToString() => $"{nameof(CPU)}: {CPU}, {nameof(GPU)}: {GPU}";
}

public readonly struct SensorSettings
{
    public int CPUSensorID { get; init; }
    public int GPUSensorID { get; init; }
    public int CPUFanID { get; init; }
    public int GPUFanID { get; init; }
}

public readonly struct DpiScale : IDisplayName, IEquatable<DpiScale>
{
    public int Scale { get; }

    [JsonIgnore]
    public string DisplayName => $"{Scale}%";

    [JsonConstructor]
    public DpiScale(int scale)
    {
        Scale = scale;
    }

    #region Equality

    public override bool Equals(object? obj) => obj is DpiScale rate && Equals(rate);

    public bool Equals(DpiScale other) => Scale == other.Scale;

    public override int GetHashCode() => HashCode.Combine(Scale);

    public static bool operator ==(DpiScale left, DpiScale right) => left.Equals(right);

    public static bool operator !=(DpiScale left, DpiScale right) => !(left == right);

    #endregion
}

public readonly struct RefreshRate : IDisplayName, IEquatable<RefreshRate>
{
    public int Frequency { get; }

    [JsonIgnore]
    public string DisplayName => $"{Frequency} Hz";

    [JsonConstructor]
    public RefreshRate(int frequency)
    {
        Frequency = frequency;
    }

    public override string ToString() => $"{Frequency}Hz";

    #region Equality

    public override bool Equals(object? obj) => obj is RefreshRate rate && Equals(rate);

    public bool Equals(RefreshRate other) => Frequency == other.Frequency;

    public override int GetHashCode() => HashCode.Combine(Frequency);

    public static bool operator ==(RefreshRate left, RefreshRate right) => left.Equals(right);

    public static bool operator !=(RefreshRate left, RefreshRate right) => !(left == right);

    #endregion
}

public readonly struct Resolution : IDisplayName, IEquatable<Resolution>, IComparable<Resolution>
{
    [JsonProperty]
    private int Width { get; }

    [JsonProperty]
    private int Height { get; }

    [JsonIgnore]
    public string DisplayName => $"{Width} × {Height}";

    [JsonConstructor]
    public Resolution(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public Resolution(Size size) : this(size.Width, size.Height) { }

    public override string ToString() => $"{Width}x{Height}";

    public int CompareTo(Resolution other)
    {
        var widthComparison = Width.CompareTo(other.Width);
        return widthComparison != 0
            ? widthComparison
            : Height.CompareTo(other.Height);
    }

    #region Conversion

    public static explicit operator Resolution(Size value) => new(value);

    public static implicit operator Size(Resolution data) => new(data.Width, data.Height);

    #endregion

    #region Equality

    public override bool Equals(object? obj) => obj is Resolution other && Equals(other);

    public bool Equals(Resolution other) => Width == other.Width && Height == other.Height;

    public override int GetHashCode() => HashCode.Combine(Width, Height);

    public static bool operator ==(Resolution left, Resolution right) => left.Equals(right);

    public static bool operator !=(Resolution left, Resolution right) => !(left == right);

    #endregion

}

public readonly struct SpectrumKeyboardBacklightEffect
{
    public SpectrumKeyboardBacklightEffectType Type { get; }
    public SpectrumKeyboardBacklightSpeed Speed { get; }
    public SpectrumKeyboardBacklightDirection Direction { get; }
    public SpectrumKeyboardBacklightClockwiseDirection ClockwiseDirection { get; }
    public RGBColor[] Colors { get; }
    public ushort[] Keys { get; }

    public SpectrumKeyboardBacklightEffect(SpectrumKeyboardBacklightEffectType type,
        SpectrumKeyboardBacklightSpeed speed,
        SpectrumKeyboardBacklightDirection direction,
        SpectrumKeyboardBacklightClockwiseDirection clockwiseDirection,
        RGBColor[] colors,
        ushort[] keys)
    {
        Type = type;
        Speed = speed;
        Direction = direction;
        ClockwiseDirection = clockwiseDirection;
        Colors = colors;
        Keys = type.IsAllLightsEffect() ? Array.Empty<ushort>() : keys;
    }
}

public readonly struct StepperValue
{
    public int Value { get; }
    public int Min { get; }
    public int Max { get; }
    public int Step { get; }
    public int[] Steps { get; }
    public int? DefaultValue { get; }

    public StepperValue(int value, int min, int max, int step, int[] steps, int? defaultValue)
    {
        Value = value;
        Min = min;
        Max = max;
        Step = step;
        Steps = steps;
        DefaultValue = defaultValue;
    }

    public StepperValue WithValue(int value) => new(value, Min, Max, Step, Steps, DefaultValue);

    public override string ToString() =>
        $"{nameof(Value)}: {Value}," +
        $" {nameof(Min)}: {Min}," +
        $" {nameof(Max)}: {Max}," +
        $" {nameof(Step)}: {Step}," +
        $" {nameof(Steps)}: [{string.Join(", ", Steps)}]," +
        $" {nameof(DefaultValue)} : {DefaultValue}";
}

public readonly struct Time
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

public readonly struct Update
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

public readonly struct WarrantyInfo
{
    public DateTime? Start { get; init; }
    public DateTime? End { get; init; }
    public Uri? Link { get; init; }
}

public readonly struct WindowSize
{
    public double Width { get; }
    public double Height { get; }

    public WindowSize(double width, double height)
    {
        Width = width;
        Height = height;
    }
}
