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

public readonly struct BatteryInformation(
    bool isCharging,
    int batteryPercentage,
    int batteryLifeRemaining,
    int fullBatteryLifeRemaining,
    int dischargeRate,
    int estimateChargeRemaining,
    int designCapacity,
    int fullChargeCapacity,
    int cycleCount,
    bool isLowBattery,
    double? batteryTemperatureC,
    DateTime? manufactureDate,
    DateTime? firstUseDate)
{
    public bool IsCharging { get; } = isCharging;
    public int BatteryPercentage { get; } = batteryPercentage;
    public int BatteryLifeRemaining { get; } = batteryLifeRemaining;
    public int FullBatteryLifeRemaining { get; init; } = fullBatteryLifeRemaining;
    public int DischargeRate { get; } = dischargeRate;
    public int EstimateChargeRemaining { get; } = estimateChargeRemaining;
    public int DesignCapacity { get; } = designCapacity;
    public int FullChargeCapacity { get; } = fullChargeCapacity;
    public int CycleCount { get; } = cycleCount;
    public bool IsLowBattery { get; } = isLowBattery;
    public double? BatteryTemperatureC { get; } = batteryTemperatureC;
    public DateTime? ManufactureDate { get; } = manufactureDate;
    public DateTime? FirstUseDate { get; } = firstUseDate;
}

public readonly struct BiosVersion(string prefix, int? version)
{
    public string Prefix { get; } = prefix;
    public int? Version { get; } = version;

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

public readonly struct Brightness(byte value)
{
    public byte Value { get; } = value;
}

public readonly struct DiscreteCapability(CapabilityID id, int value)
{
    public CapabilityID Id { get; } = id;
    public int Value { get; } = value;
}

public readonly struct DisplayAdvancedColorInfo(bool advancedColorSupported, bool advancedColorEnabled, bool wideColorEnforced, bool advancedColorForceDisabled)
{
    public bool AdvancedColorSupported { get; } = advancedColorSupported;
    public bool AdvancedColorEnabled { get; } = advancedColorEnabled;
    public bool WideColorEnforced { get; } = wideColorEnforced;
    public bool AdvancedColorForceDisabled { get; } = advancedColorForceDisabled;
}

public struct Device(
    string name,
    string description,
    string busReportedDeviceDescription,
    string deviceInstanceId,
    Guid classGuid,
    string className,
    bool isRemovable,
    bool isDisconnected)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string BusReportedDeviceDescription { get; } = busReportedDeviceDescription;
    public string DeviceInstanceId { get; } = deviceInstanceId;
    public Guid ClassGuid { get; } = classGuid;
    public string ClassName { get; } = className;
    public bool IsRemovable { get; } = isRemovable;
    public bool IsDisconnected { get; } = isDisconnected;

    private string? _index;

    public string Index
    {
        get
        {
            _index ??= new StringBuilder()
                .Append(ClassName)
                .Append(ClassGuid)
                .Append(BusReportedDeviceDescription)
                .Append(Description)
                .Append(Name)
                .Append(DeviceInstanceId)
                .ToString();
            return _index;
        }
    }
}

public readonly struct DriverInfo(string deviceId, string hardwareId, Version? version, DateTime? date)
{
    public string DeviceId { get; } = deviceId;
    public string HardwareId { get; } = hardwareId;
    public Version? Version { get; } = version;
    public DateTime? Date { get; } = date;
}

public readonly struct FanTableData(FanTableType type, byte fanId, byte sensorId, ushort[] fanSpeeds, ushort[] temps)
{
    public FanTableType Type { get; } = type;
    public byte FanId { get; } = fanId;
    public byte SensorId { get; } = sensorId;
    public ushort[] FanSpeeds { get; } = fanSpeeds;
    public ushort[] Temps { get; } = temps;

    public override string ToString() =>
        $"{nameof(Type)}: {Type}," +
        $" {nameof(FanId)}: {FanId}," +
        $" {nameof(SensorId)}: {SensorId}," +
        $" {nameof(FanSpeeds)}: [{string.Join(", ", FanSpeeds)}]," +
        $" {nameof(Temps)}: [{string.Join(", ", Temps)}]";
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
            throw new ArgumentException("Fan table length must be 10", nameof(fanTable));

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

    public ushort[] GetTable() => [FSS0, FSS1, FSS2, FSS3, FSS4, FSS5, FSS6, FSS7, FSS8, FSS9];

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

public readonly struct FanTableInfo(FanTableData[] data, FanTable table)
{
    public FanTableData[] Data { get; } = data;
    public FanTable Table { get; } = table;

    public override string ToString() =>
        $"{nameof(Data)}: [{string.Join(", ", Data)}]," +
        $" {nameof(Table)}: {Table}";
}

public readonly struct GPUOverclockInfo(int coreDeltaMhz, int memoryDeltaMhz)
{
    public static readonly GPUOverclockInfo Zero = new();

    public int CoreDeltaMhz { get; } = coreDeltaMhz;
    public int MemoryDeltaMhz { get; } = memoryDeltaMhz;

    #region Equality

    public override bool Equals(object? obj) => obj is GPUOverclockInfo other && CoreDeltaMhz == other.CoreDeltaMhz && MemoryDeltaMhz == other.MemoryDeltaMhz;

    public override int GetHashCode() => HashCode.Combine(CoreDeltaMhz, MemoryDeltaMhz);

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

public readonly struct GPUStatus(GPUState state, string? performanceState, List<Process> processes)
{
    public GPUState State { get; } = state;
    public string? PerformanceState { get; } = performanceState;
    public List<Process> Processes { get; } = processes;
    public int ProcessCount => Processes.Count;
}

public readonly struct HardwareId(string vendor, string device)
{
    public static readonly HardwareId Empty = new();

    public string Vendor { get; } = vendor;
    public string Device { get; } = device;

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
    public readonly struct FeatureData(FeatureData.SourceType sourceType, IEnumerable<CapabilityID> capabilities)
    {
        public static readonly FeatureData Unknown = new(SourceType.Unknown);

        public enum SourceType
        {
            Unknown,
            Flags,
            CapabilityData
        }

        private readonly HashSet<CapabilityID> _capabilities = [.. capabilities];

        public SourceType Source { get; } = sourceType;

        public IEnumerable<CapabilityID> All => _capabilities.Order().AsEnumerable();

        public FeatureData(SourceType sourceType) : this(sourceType, []) { }

        public bool this[CapabilityID key]
        {
            get => _capabilities.Contains(key);
            init
            {
                if (value)
                    _capabilities.Add(key);
                else
                    _capabilities.Remove(key);
            }
        }
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

public readonly struct WindowsPowerPlan(Guid guid, string name, bool isActive)
{
    public Guid Guid { get; } = guid;
    public string Name { get; } = name;
    public bool IsActive { get; } = isActive;

    public override string ToString() => $"{nameof(Guid)}: {Guid}, {nameof(Name)}: {Name}, {nameof(IsActive)}: {IsActive}";

    #region Equality

    public override bool Equals(object? obj) => obj is WindowsPowerPlan other && Guid.Equals(other.Guid);

    public override int GetHashCode() => Guid.GetHashCode();

    public static bool operator ==(WindowsPowerPlan left, WindowsPowerPlan right) => left.Equals(right);

    public static bool operator !=(WindowsPowerPlan left, WindowsPowerPlan right) => !left.Equals(right);

    #endregion
}

[method: JsonConstructor]
public readonly struct ProcessInfo(string name, string? executablePath) : IComparable
{
    public static ProcessInfo FromPath(string path) => new(Path.GetFileNameWithoutExtension(path), path);

    public string Name { get; } = name;

    public string? ExecutablePath { get; } = executablePath;

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

public readonly struct RangeCapability(CapabilityID id, int defaultValue, int min, int max, int step)
{
    public CapabilityID Id { get; } = id;
    public int DefaultValue { get; } = defaultValue;
    public int Min { get; } = min;
    public int Max { get; } = max;
    public int Step { get; } = step;
}

[method: JsonConstructor]
public readonly struct RGBColor(byte r, byte g, byte b)
{
    public static readonly RGBColor Green = new(142, 255, 0);
    public static readonly RGBColor Pink = new(186, 0, 255);
    public static readonly RGBColor Purple = new(101, 0, 255);
    public static readonly RGBColor Red = new(255, 0, 0);
    public static readonly RGBColor Teal = new(0, 212, 255);
    public static readonly RGBColor White = new(255, 255, 255);

    public byte R { get; } = r;
    public byte G { get; } = g;
    public byte B { get; } = b;

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

[method: JsonConstructor]
public readonly struct RGBKeyboardBacklightBacklightPresetDescription(
    RGBKeyboardBacklightEffect effect,
    RGBKeyboardBacklightSpeed speed,
    RGBKeyboardBacklightBrightness brightness,
    RGBColor zone1,
    RGBColor zone2,
    RGBColor zone3,
    RGBColor zone4)
{
    public static readonly RGBKeyboardBacklightBacklightPresetDescription Default = new(RGBKeyboardBacklightEffect.Static, RGBKeyboardBacklightSpeed.Slowest, RGBKeyboardBacklightBrightness.High, RGBColor.White, RGBColor.White, RGBColor.White, RGBColor.White);

    public RGBKeyboardBacklightEffect Effect { get; } = effect;
    public RGBKeyboardBacklightSpeed Speed { get; } = speed;
    public RGBKeyboardBacklightBrightness Brightness { get; } = brightness;
    public RGBColor Zone1 { get; } = zone1;
    public RGBColor Zone2 { get; } = zone2;
    public RGBColor Zone3 { get; } = zone3;
    public RGBColor Zone4 { get; } = zone4;

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

    public static bool operator ==(RGBKeyboardBacklightBacklightPresetDescription left, RGBKeyboardBacklightBacklightPresetDescription right) => left.Equals(right);

    public static bool operator !=(RGBKeyboardBacklightBacklightPresetDescription left, RGBKeyboardBacklightBacklightPresetDescription right) => !(left == right);

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

[method: JsonConstructor]
public readonly struct RGBKeyboardBacklightState(
    RGBKeyboardBacklightPreset selectedPreset,
    Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightBacklightPresetDescription> presets)
{
    public RGBKeyboardBacklightPreset SelectedPreset { get; } = selectedPreset;
    public Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightBacklightPresetDescription> Presets { get; } = presets;
}

public readonly struct SensorData(
    int utilization,
    int maxUtilization,
    int coreClock,
    int maxCoreClock,
    int memoryClock,
    int maxMemoryClock,
    int temperature,
    int maxTemperature,
    int fanSpeed,
    int maxFanSpeed)
{
    public static readonly SensorData Empty = new(-1, -1, -1, -1, -1, -1, -1, -1, -1, -1);

    public int Utilization { get; } = utilization;
    public int MaxUtilization { get; } = maxUtilization;
    public int CoreClock { get; } = coreClock;
    public int MaxCoreClock { get; } = maxCoreClock;
    public int MemoryClock { get; } = memoryClock;
    public int MaxMemoryClock { get; } = maxMemoryClock;
    public int Temperature { get; } = temperature;
    public int MaxTemperature { get; } = maxTemperature;
    public int FanSpeed { get; } = fanSpeed;
    public int MaxFanSpeed { get; } = maxFanSpeed;

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

public readonly struct SensorsData(SensorData cpu, SensorData gpu)
{
    public static readonly SensorsData Empty = new(SensorData.Empty, SensorData.Empty);

    public SensorData CPU { get; } = cpu;
    public SensorData GPU { get; } = gpu;

    public override string ToString() => $"{nameof(CPU)}: {CPU}, {nameof(GPU)}: {GPU}";
}

[method: JsonConstructor]
public readonly struct DpiScale(int scale) : IDisplayName, IEquatable<DpiScale>
{
    public int Scale { get; } = scale;

    [JsonIgnore]
    public string DisplayName => $"{Scale}%";

    #region Equality

    public override bool Equals(object? obj) => obj is DpiScale rate && Equals(rate);

    public bool Equals(DpiScale other) => Scale == other.Scale;

    public override int GetHashCode() => HashCode.Combine(Scale);

    public static bool operator ==(DpiScale left, DpiScale right) => left.Equals(right);

    public static bool operator !=(DpiScale left, DpiScale right) => !(left == right);

    #endregion
}

[method: JsonConstructor]
public readonly struct RefreshRate(int frequency) : IDisplayName, IEquatable<RefreshRate>
{
    public int Frequency { get; } = frequency;

    [JsonIgnore]
    public string DisplayName => $"{Frequency} Hz";

    public override string ToString() => $"{Frequency}Hz";

    #region Equality

    public override bool Equals(object? obj) => obj is RefreshRate rate && Equals(rate);

    public bool Equals(RefreshRate other) => Frequency == other.Frequency;

    public override int GetHashCode() => HashCode.Combine(Frequency);

    public static bool operator ==(RefreshRate left, RefreshRate right) => left.Equals(right);

    public static bool operator !=(RefreshRate left, RefreshRate right) => !(left == right);

    #endregion
}

[method: JsonConstructor]
public readonly struct Resolution(int width, int height) : IDisplayName, IEquatable<Resolution>, IComparable<Resolution>
{
    [JsonProperty]
    public int Width { get; } = width;

    [JsonProperty]
    public int Height { get; } = height;

    [JsonIgnore]
    public string DisplayName => $"{Width} × {Height}";

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

public readonly struct SpectrumKeyboardBacklightEffect(
    SpectrumKeyboardBacklightEffectType type,
    SpectrumKeyboardBacklightSpeed speed,
    SpectrumKeyboardBacklightDirection direction,
    SpectrumKeyboardBacklightClockwiseDirection clockwiseDirection,
    RGBColor[] colors,
    ushort[] keys)
{
    public SpectrumKeyboardBacklightEffectType Type { get; } = type;
    public SpectrumKeyboardBacklightSpeed Speed { get; } = speed;
    public SpectrumKeyboardBacklightDirection Direction { get; } = direction;
    public SpectrumKeyboardBacklightClockwiseDirection ClockwiseDirection { get; } = clockwiseDirection;
    public RGBColor[] Colors { get; } = colors;
    public ushort[] Keys { get; } = type.IsAllLightsEffect() ? [] : keys;
}

public readonly struct StepperValue(int value, int min, int max, int step, int[] steps, int? defaultValue)
{
    public int Value { get; } = value;
    public int Min { get; } = min;
    public int Max { get; } = max;
    public int Step { get; } = step;
    public int[] Steps { get; } = steps;
    public int? DefaultValue { get; } = defaultValue;

    public StepperValue WithValue(int value) => new(value, Min, Max, Step, Steps, DefaultValue);

    public override string ToString() =>
        $"{nameof(Value)}: {Value}," +
        $" {nameof(Min)}: {Min}," +
        $" {nameof(Max)}: {Max}," +
        $" {nameof(Step)}: {Step}," +
        $" {nameof(Steps)}: [{string.Join(", ", Steps)}]," +
        $" {nameof(DefaultValue)} : {DefaultValue}";
}

public readonly struct Time(int hour, int minute)
{
    public int Hour { get; } = hour;
    public int Minute { get; } = minute;

    #region Equality

    public override bool Equals(object? obj) => obj is Time time && Hour == time.Hour && Minute == time.Minute;

    public override int GetHashCode() => HashCode.Combine(Hour, Minute);

    public static bool operator ==(Time left, Time right) => left.Equals(right);

    public static bool operator !=(Time left, Time right) => !(left == right);

    #endregion
}

public readonly struct Update(Release release)
{
    public Version Version { get; } = Version.Parse(release.TagName);
    public string Title { get; } = release.Name;
    public string Description { get; } = release.Body;
    public DateTimeOffset Date { get; } = release.PublishedAt ?? release.CreatedAt;
    public string? Url { get; } = release.Assets
        .Where(ra => ra.Name.EndsWith("setup.exe", StringComparison.InvariantCultureIgnoreCase))
        .Select(ra => ra.BrowserDownloadUrl)
        .FirstOrDefault();

    #region Equality

    public override bool Equals(object? obj) => obj is Update other && Version.Equals(other.Version);

    public override int GetHashCode() => Version.GetHashCode();

    public static bool operator ==(Update left, Update right) => left.Equals(right);

    public static bool operator !=(Update left, Update right) => !left.Equals(right);

    #endregion
}

public readonly struct WarrantyInfo(DateTime? start, DateTime? end, Uri? link)
{
    public DateTime? Start { get; } = start;
    public DateTime? End { get; } = end;
    public Uri? Link { get; } = link;
}

public readonly struct WindowSize(double width, double height)
{
    public double Width { get; } = width;
    public double Height { get; } = height;
}
