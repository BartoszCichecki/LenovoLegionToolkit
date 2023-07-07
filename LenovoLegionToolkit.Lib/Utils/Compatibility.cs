using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;

// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.Utils;

public static class Compatibility
{
    private const string ALLOWED_VENDOR = "LENOVO";

    private static readonly string[] AllowedModelsPrefix = {
        // Worldwide variants
        "17ACH",
        "17ARH",
        "17ITH",
        "17IMH",

        "16ACH",
        "16APH",
        "16ARH",
        "16ARX",
        "16IAH",
        "16IAX",
        "16IRH",
        "16IRX",
        "16ITH",

        "15ACH",
        "15APH",
        "15ARH",
        "15IAH",
        "15IHU",
        "15IMH",
        "15IRH",
        "15ITH",

        "14IRP",

        // Chinese variants
        "G5000",
        "R9000",
        "R7000",
        "Y9000",
        "Y7000",
            
        // Limited compatibility
        "17IR",
        "15IR",
        "15IC"
    };

    private static MachineInformation? _machineInformation;

    public static Task<bool> CheckBasicCompatibilityAsync() => WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_GAMEZONE_DATA");

    public static async Task<(bool isCompatible, MachineInformation machineInformation)> IsCompatibleAsync()
    {
        var mi = await GetMachineInformationAsync().ConfigureAwait(false);

        if (!await CheckBasicCompatibilityAsync().ConfigureAwait(false))
            return (false, mi);

        if (!mi.Vendor.Equals(ALLOWED_VENDOR, StringComparison.InvariantCultureIgnoreCase))
            return (false, mi);

        foreach (var allowedModel in AllowedModelsPrefix)
            if (mi.Model.Contains(allowedModel, StringComparison.InvariantCultureIgnoreCase))
                return (true, mi);

        return (false, mi);
    }

    public static async Task<MachineInformation> GetMachineInformationAsync()
    {
        if (_machineInformation.HasValue)
            return _machineInformation.Value;

        var (vendor, machineType, model, serialNumber) = await GetModelDataAsync().ConfigureAwait(false);
        var (biosVersion, biosVersionRaw) = await GetBIOSVersionAsync().ConfigureAwait(false);
        var supportedPowerModes = (await GetSupportedPowerModesAsync().ConfigureAwait(false)).ToArray();
        var smartFanVersion = await GetSmartFanVersionAsync().ConfigureAwait(false);
        var legionZoneVersion = await GetLegionZoneVersionAsync().ConfigureAwait(false);
        var features = await GetFeaturesAsync().ConfigureAwait(false);

        var machineInformation = new MachineInformation
        {
            Vendor = vendor,
            MachineType = machineType,
            Model = model,
            SerialNumber = serialNumber,
            BiosVersion = biosVersion,
            BiosVersionRaw = biosVersionRaw,
            SupportedPowerModes = supportedPowerModes,
            SmartFanVersion = smartFanVersion,
            LegionZoneVersion = legionZoneVersion,
            Features = features,
            Properties = new()
            {
                SupportsAlwaysOnAc = GetAlwaysOnAcStatus(),
                SupportsGodModeV1 = GetSupportsGodModeV1(supportedPowerModes, smartFanVersion, legionZoneVersion, biosVersion),
                SupportsGodModeV2 = GetSupportsGodModeV2(supportedPowerModes, smartFanVersion, legionZoneVersion),
                SupportsIGPUMode = await GetSupportsIGPUModeAsync().ConfigureAwait(false),
                SupportsIntelligentSubMode = await GetSupportsIntelligentSubModeAsync().ConfigureAwait(false),
                HasQuietToPerformanceModeSwitchingBug = GetHasQuietToPerformanceModeSwitchingBug(biosVersion),
                HasGodModeToOtherModeSwitchingBug = GetHasGodModeToOtherModeSwitchingBug(biosVersion),
                IsExcludedFromLenovoLighting = GetIsExcludedFromLenovoLighting(biosVersion),
                IsExcludedFromPanelLogoLenovoLighting = GetIsExcludedFromPanelLenovoLighting(machineType, model)
            }
        };

        if (Log.Instance.IsTraceEnabled)
        {
            Log.Instance.Trace($"Retrieved machine information:");
            Log.Instance.Trace($" * Vendor: '{machineInformation.Vendor}'");
            Log.Instance.Trace($" * Machine Type: '{machineInformation.MachineType}'");
            Log.Instance.Trace($" * Model: '{machineInformation.Model}'");
            Log.Instance.Trace($" * BIOS: '{machineInformation.BiosVersion}' [{machineInformation.BiosVersionRaw}]");
            Log.Instance.Trace($" * SupportedPowerModes: '{string.Join(",", machineInformation.SupportedPowerModes)}'");
            Log.Instance.Trace($" * SmartFanVersion: '{machineInformation.SmartFanVersion}'");
            Log.Instance.Trace($" * LegionZoneVersion: '{machineInformation.LegionZoneVersion}'");
            Log.Instance.Trace($" * Features:");
            Log.Instance.Trace($"     * Source: '{machineInformation.Features.Source}'");
            Log.Instance.Trace($"     * IGPUMode: '{machineInformation.Features.IGPUMode}'");
            Log.Instance.Trace($"     * FlipToStart: '{machineInformation.Features.FlipToStart}'");
            Log.Instance.Trace($"     * NvidiaGPUDynamicDisplaySwitching: '{machineInformation.Features.NvidiaGPUDynamicDisplaySwitching}'");
            Log.Instance.Trace($"     * InstantBootAc: '{machineInformation.Features.InstantBootAc}'");
            Log.Instance.Trace($"     * InstantBootUsbPowerDelivery: '{machineInformation.Features.InstantBootUsbPowerDelivery}'");
            Log.Instance.Trace($"     * AMDSmartShiftMode: '{machineInformation.Features.AMDSmartShiftMode}'");
            Log.Instance.Trace($"     * AMDSkinTemperatureTracking: '{machineInformation.Features.AMDSkinTemperatureTracking}'");
            Log.Instance.Trace($" * Properties:");
            Log.Instance.Trace($"     * SupportsAlwaysOnAc: '{machineInformation.Properties.SupportsAlwaysOnAc.status}, {machineInformation.Properties.SupportsAlwaysOnAc.connectivity}'");
            Log.Instance.Trace($"     * SupportsGodModeV1: '{machineInformation.Properties.SupportsGodModeV1}'");
            Log.Instance.Trace($"     * SupportsGodModeV2: '{machineInformation.Properties.SupportsGodModeV2}'");
            Log.Instance.Trace($"     * SupportsIGPUMode: '{machineInformation.Properties.SupportsIGPUMode}'");
            Log.Instance.Trace($"     * SupportsIntelligentSubMode: '{machineInformation.Properties.SupportsIntelligentSubMode}'");
            Log.Instance.Trace($"     * HasQuietToPerformanceModeSwitchingBug: '{machineInformation.Properties.HasQuietToPerformanceModeSwitchingBug}'");
            Log.Instance.Trace($"     * HasGodModeToOtherModeSwitchingBug: '{machineInformation.Properties.HasGodModeToOtherModeSwitchingBug}'");
            Log.Instance.Trace($"     * IsExcludedFromLenovoLighting: '{machineInformation.Properties.IsExcludedFromLenovoLighting}'");
            Log.Instance.Trace($"     * IsExcludedFromPanelLogoLenovoLighting: '{machineInformation.Properties.IsExcludedFromPanelLogoLenovoLighting}'");
        }

        return (_machineInformation = machineInformation).Value;
    }

    private static async Task<(string, string, string, string)> GetModelDataAsync()
    {
        var result = await WMI.ReadAsync("root\\CIMV2",
            $"SELECT * FROM Win32_ComputerSystemProduct",
            pdc =>
            {
                var machineType = (string)pdc["Name"].Value;
                var vendor = (string)pdc["Vendor"].Value;
                var model = (string)pdc["Version"].Value;
                var serialNumber = (string)pdc["IdentifyingNumber"].Value;
                return (vendor, machineType, model, serialNumber);
            }).ConfigureAwait(false);
        return result.First();
    }

    private static async Task<(BiosVersion?, string?)> GetBIOSVersionAsync()
    {
        var result = await WMI.ReadAsync("root\\CIMV2",
            $"SELECT * FROM Win32_BIOS",
            pdc => (string)pdc["Name"].Value).ConfigureAwait(false);
        var biosString = result.First();

        var prefixRegex = new Regex("^[A-Z0-9]{4}");
        var versionRegex = new Regex("[0-9]{2}");

        var prefix = prefixRegex.Match(biosString).Value;
        var versionString = versionRegex.Match(biosString).Value;

        if (!int.TryParse(versionRegex.Match(versionString).Value, out var version))
            return (null, null);

        return (new(prefix, version), biosString);
    }

    private static async Task<MachineInformation.FeatureData> GetFeaturesAsync()
    {
        try
        {
            var capabilities = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_CAPABILITY_DATA_00",
                pdc => (CapabilityID)Convert.ToInt32(pdc["IDs"].Value)).ConfigureAwait(false);
            capabilities = capabilities.ToArray();

            return new()
            {
                Source = MachineInformation.FeatureData.SourceType.CapabilityData,
                IGPUMode = capabilities.Contains(CapabilityID.IGPUMode),
                FlipToStart = capabilities.Contains(CapabilityID.FlipToStart),
                NvidiaGPUDynamicDisplaySwitching = capabilities.Contains(CapabilityID.NvidiaGPUDynamicDisplaySwitching),
                InstantBootAc = capabilities.Contains(CapabilityID.InstantBootAc),
                InstantBootUsbPowerDelivery = capabilities.Contains(CapabilityID.InstantBootUsbPowerDelivery),
                AMDSmartShiftMode = capabilities.Contains(CapabilityID.AMDSmartShiftMode),
                AMDSkinTemperatureTracking = capabilities.Contains(CapabilityID.AMDSkinTemperatureTracking),
            };
        }
        catch { /* Ignored. */ }

        try
        {
            var featureFlags = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "Get_Legion_Device_Support_Feature",
            new(),
            pdc => Convert.ToInt32(pdc["Status"].Value)).ConfigureAwait(false);

            return new()
            {
                Source = MachineInformation.FeatureData.SourceType.Flags,
                IGPUMode = featureFlags.IsBitSet(0),
                FlipToStart = true,
                NvidiaGPUDynamicDisplaySwitching = featureFlags.IsBitSet(4),
                InstantBootAc = featureFlags.IsBitSet(5),
                InstantBootUsbPowerDelivery = featureFlags.IsBitSet(6),
                AMDSmartShiftMode = featureFlags.IsBitSet(7),
                AMDSkinTemperatureTracking = featureFlags.IsBitSet(8)
            };
        }
        catch { /* Ignored. */ }

        return MachineInformation.FeatureData.Unknown;
    }

    private static async Task<IEnumerable<PowerModeState>> GetSupportedPowerModesAsync()
    {
        try
        {
            var powerModes = new List<PowerModeState>();

            var result = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_OTHER_METHOD",
                "GetFeatureValue",
                new() { { "IDs", (int)CapabilityID.SupportedPowerModes } },
                pdc => Convert.ToInt32(pdc["Value"].Value)).ConfigureAwait(false);

            if (result.IsBitSet(0))
                powerModes.Add(PowerModeState.Quiet);
            if (result.IsBitSet(1))
                powerModes.Add(PowerModeState.Balance);
            if (result.IsBitSet(2))
                powerModes.Add(PowerModeState.Performance);
            if (result.IsBitSet(16))
                powerModes.Add(PowerModeState.GodMode);

            return powerModes;
        }
        catch { /* Ignored. */ }

        try
        {
            var powerModes = new List<PowerModeState>();

            var result = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_OTHER_METHOD",
                "GetSupportThermalMode",
                new(),
                pdc => Convert.ToInt32(pdc["mode"].Value)).ConfigureAwait(false);

            if (result.IsBitSet(0))
                powerModes.Add(PowerModeState.Quiet);
            if (result.IsBitSet(1))
                powerModes.Add(PowerModeState.Balance);
            if (result.IsBitSet(2))
                powerModes.Add(PowerModeState.Performance);
            if (result.IsBitSet(16))
                powerModes.Add(PowerModeState.GodMode);

            return powerModes;
        }
        catch { /* Ignored. */ }

        return Array.Empty<PowerModeState>();
    }

    private static async Task<int> GetSmartFanVersionAsync()
    {
        try
        {
            var result = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                "IsSupportSmartFan",
                new(),
                pdc => Convert.ToInt32(pdc["Data"].Value)).ConfigureAwait(false);
            return result;
        }
        catch { /* Ignored. */ }

        return -1;
    }

    private static async Task<int> GetLegionZoneVersionAsync()
    {
        try
        {
            var result = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_OTHER_METHOD",
                "GetFeatureValue",
                new() { { "IDs", (int)CapabilityID.LegionZoneSupportVersion } },
                pdc => Convert.ToInt32(pdc["Value"].Value)).ConfigureAwait(false);
            return result;
        }
        catch { /* Ignored. */ }

        try
        {
            var result = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_OTHER_METHOD",
                "Get_Support_LegionZone_Version",
                new(),
                pdc => Convert.ToInt32(pdc["Version"].Value)).ConfigureAwait(false);
            return result;
        }
        catch { /* Ignored. */ }

        return -1;
    }

    private static unsafe (bool status, bool connectivity) GetAlwaysOnAcStatus()
    {
        var capabilities = new SYSTEM_POWER_CAPABILITIES();
        var result = PInvoke.CallNtPowerInformation(POWER_INFORMATION_LEVEL.SystemPowerCapabilities,
            null,
            0,
            &capabilities,
            (uint)Marshal.SizeOf<SYSTEM_POWER_CAPABILITIES>());

        if (result.SeverityCode == NTSTATUS.Severity.Success)
            return (false, false);

        return (capabilities.AoAc, capabilities.AoAcConnectivitySupported);
    }

    private static bool GetSupportsGodModeV1(IEnumerable<PowerModeState> supportedPowerModes, int smartFanVersion, int legionZoneVersion, BiosVersion? biosVersion)
    {
        if (!supportedPowerModes.Contains(PowerModeState.GodMode))
            return false;

        var affectedBiosVersions = new BiosVersion[]
        {
            new("G9CN", 24),
            new("GKCN", 46),
            new("H1CN", 39),
            new("HACN", 31),
            new("HHCN", 20)
        };

        if (affectedBiosVersions.Any(bv => biosVersion?.IsLowerThan(bv) ?? false))
            return false;

        return smartFanVersion is 4 or 5 || legionZoneVersion is 1 or 2;
    }

    private static bool GetSupportsGodModeV2(IEnumerable<PowerModeState> supportedPowerModes, int smartFanVersion, int legionZoneVersion)
    {
        if (!supportedPowerModes.Contains(PowerModeState.GodMode))
            return false;

        return smartFanVersion is 6 || legionZoneVersion is 3;
    }

    private static async Task<bool> GetSupportsIGPUModeAsync()
    {
        try
        {
            var result = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                "IsSupportIGPUMode",
                new(),
                pdc => (uint)pdc["Data"].Value).ConfigureAwait(false);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> GetSupportsIntelligentSubModeAsync()
    {
        try
        {
            _ = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                "GetIntelligentSubMode",
                new(),
                pdc => (uint)pdc["Data"].Value).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool GetHasQuietToPerformanceModeSwitchingBug(BiosVersion? biosVersion)
    {
        var affectedBiosVersions = new BiosVersion[]
        {
            new("J2CN", null)
        };

        return affectedBiosVersions.Any(bv => biosVersion?.IsHigherOrEqualThan(bv) ?? false);
    }

    private static bool GetHasGodModeToOtherModeSwitchingBug(BiosVersion? biosVersion)
    {
        var affectedBiosVersions = new BiosVersion[]
        {
            new("K1CN", null)
        };

        return affectedBiosVersions.Any(bv => biosVersion?.IsHigherOrEqualThan(bv) ?? false);
    }

    private static bool GetIsExcludedFromLenovoLighting(BiosVersion? biosVersion)
    {
        var affectedBiosVersions = new BiosVersion[]
        {
            new("GKCN", 54)
        };

        return affectedBiosVersions.Any(bv => biosVersion?.IsLowerThan(bv) ?? false);
    }

    private static bool GetIsExcludedFromPanelLenovoLighting(string machineType, string model)
    {
        (string machineType, string model)[] excludedModels =
        {
            ("82JH", "15ITH6H"),
            ("82JK", "15ITH6"),
            ("82JM", "17ITH6H"),
            ("82JN", "17ITH6"),
            ("82JU", "15ACH6H"),
            ("82JW", "15ACH6"),
            ("82JY", "17ACH6H"),
            ("82K0", "17ACH6"),
            ("82K1", "15IHU6"),
            ("82K2", "15ACH6"),
            ("82NW", "15ACH6A")
        };

        return excludedModels.Where(m =>
        {
            var result = machineType.Contains(m.machineType);
            result &= model.Contains(m.model);
            return result;
        }).Any();
    }
}
