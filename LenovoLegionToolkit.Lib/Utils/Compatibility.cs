using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        "16ARH",
        "16ARX",
        "16IAH",
        "16IAX",
        "16IRH",
        "16IRX",
        "16ITH",

        "15ACH",
        "15ARH",
        "15IAH",
        "15IHU",
        "15IMH",
        "15ITH",

        // Chinese variants
        "IRH8",
        "R9000",
        "R7000",
        "Y9000",
        "Y7000",
            
        // Limited compatibility
        "17IR",
        "15IR",
        "15ICH"
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
        if (!_machineInformation.HasValue)
        {
            var (vendor, machineType, model, serialNumber) = await GetModelDataAsync().ConfigureAwait(false);
            var (biosVersion, biosVersionRaw) = await GetBIOSVersionAsync().ConfigureAwait(false);

            var machineInformation = new MachineInformation
            {
                Vendor = vendor,
                MachineType = machineType,
                Model = model,
                SerialNumber = serialNumber,
                BiosVersion = biosVersion,
                BiosVersionRaw = biosVersionRaw,
                Properties = new()
                {
                    SupportsAlwaysOnAc = GetAlwaysOnAcStatus(),
                    SupportsGodModeV1 = GetSupportsGodModeV1(biosVersion),
                    SupportsGodModeV2 = GetSupportsGodModeV2(biosVersion),
                    SupportsExtendedHybridMode = await GetSupportsExtendedHybridModeAsync().ConfigureAwait(false),
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
                Log.Instance.Trace($" * Properties.SupportsAlwaysOnAc: '{machineInformation.Properties.SupportsAlwaysOnAc.status}, {machineInformation.Properties.SupportsAlwaysOnAc.connectivity}'");
                Log.Instance.Trace($" * Properties.SupportsGodModeV1: '{machineInformation.Properties.SupportsGodModeV1}'");
                Log.Instance.Trace($" * Properties.SupportsGodModeV2: '{machineInformation.Properties.SupportsGodModeV2}'");
                Log.Instance.Trace($" * Properties.SupportsExtendedHybridMode: '{machineInformation.Properties.SupportsExtendedHybridMode}'");
                Log.Instance.Trace($" * Properties.SupportsIntelligentSubMode: '{machineInformation.Properties.SupportsIntelligentSubMode}'");
                Log.Instance.Trace($" * Properties.HasQuietToPerformanceModeSwitchingBug: '{machineInformation.Properties.HasQuietToPerformanceModeSwitchingBug}'");
                Log.Instance.Trace($" * Properties.HasGodModeToOtherModeSwitchingBug: '{machineInformation.Properties.HasGodModeToOtherModeSwitchingBug}'");
                Log.Instance.Trace($" * Properties.IsExcludedFromLenovoLighting: '{machineInformation.Properties.IsExcludedFromLenovoLighting}'");
                Log.Instance.Trace($" * Properties.IsExcludedFromPanelLogoLenovoLighting: '{machineInformation.Properties.IsExcludedFromPanelLogoLenovoLighting}'");
            }

            _machineInformation = machineInformation;
        }

        return _machineInformation.Value;
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

    private static bool GetSupportsGodModeV1(BiosVersion? biosVersion)
    {
        BiosVersion[] supportedBiosVersions =
        {
            new("GKCN", 49),
            new("G9CN", 30),
            new("H1CN", 49),
            new("HACN", 31),
            new("HHCN", 23),
            new("K1CN", 31),
            new("K9CN", 34),
            new("KFCN", 32),
            new("J2CN", 40),
            new("JUCN", 51),
            new("JYCN", 39)
        };

        return supportedBiosVersions.Any(bv => biosVersion?.IsHigherOrEqualThan(bv) ?? false);
    }

    private static bool GetSupportsGodModeV2(BiosVersion? biosVersion)
    {
        BiosVersion[] supportedBiosVersions =
        {
            new("KWCN", 28),
            new("LPCN", 27),
            new("M3CN", 32),
            new("M0CN", 27)
        };

        return supportedBiosVersions.Any(bv => biosVersion?.IsHigherOrEqualThan(bv) ?? false);
    }

    private static async Task<bool> GetSupportsExtendedHybridModeAsync()
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

    private static bool GetHasQuietToPerformanceModeSwitchingBug(BiosVersion? biosVersion)
    {
        BiosVersion[] affectedBiosVersions =
        {
            new("J2CN", null)
        };

        return affectedBiosVersions.Any(bv => biosVersion?.IsHigherOrEqualThan(bv) ?? false);
    }

    private static bool GetHasGodModeToOtherModeSwitchingBug(BiosVersion? biosVersion)
    {
        BiosVersion[] affectedBiosVersions =
        {
            new("K1CN", null)
        };

        return affectedBiosVersions.Any(bv => biosVersion?.IsHigherOrEqualThan(bv) ?? false);
    }

    private static bool GetIsExcludedFromLenovoLighting(BiosVersion? biosVersion)
    {
        BiosVersion[] affectedBiosVersions =
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
