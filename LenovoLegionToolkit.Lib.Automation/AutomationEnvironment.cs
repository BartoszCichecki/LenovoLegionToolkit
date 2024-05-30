using System;
using System.Collections.Generic;
using System.Linq;

namespace LenovoLegionToolkit.Lib.Automation;

public class AutomationEnvironment
{
    private const string AC_ADAPTER_CONNECTED = "LLT_IS_AC_ADAPTER_CONNECTED";
    private const string LOW_POWER_AC_ADAPTER = "LLT_IS_AC_ADAPTER_LOW_POWER";
    private const string DISPLAY_ON = "LLT_IS_DISPLAY_ON";
    private const string EXTERNAL_DISPLAY_CONNECTED = "LLT_IS_EXTERNAL_DISPLAY_CONNECTED";
    private const string GAME_RUNNING = "LLT_IS_GAME_RUNNING";
    private const string HDR_ON = "LLT_IS_HDR_ON";
    private const string LID_OPEN = "LLT_IS_LID_OPEN";
    private const string STARTUP = "LLT_STARTUP";
    private const string RESUME = "LLT_RESUME";
    private const string POWER_MODE = "LLT_POWER_MODE";
    private const string POWER_MODE_NAME = "LLT_POWER_MODE_NAME";
    private const string PROCESSES_STARTED = "LLT_PROCESSES_STARTED";
    private const string PROCESSES = "LLT_PROCESSES";
    private const string DEVICE_CONNECTED = "LLT_DEVICE_CONNECTED";
    private const string DEVICE_INSTANCE_IDS = "LLT_DEVICE_INSTANCE_IDS";
    private const string IS_SUNSET = "LLT_IS_SUNSET";
    private const string IS_SUNRISE = "LLT_IS_SUNRISE";
    private const string TIME = "LLT_TIME";
    private const string DAYS = "LLT_DAYS";
    private const string PERIOD = "LLT_PERIOD";
    private const string USER_ACTIVE = "LLT_IS_USER_ACTIVE";
    private const string WIFI_CONNECTED = "LLT_WIFI_CONNECTED";
    private const string WIFI_SSID = "LLT_WIFI_SSID";

    private const string VALUE_TRUE = "TRUE";
    private const string VALUE_FALSE = "FALSE";

    public bool AcAdapterConnected { set => _dictionary[AC_ADAPTER_CONNECTED] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool LowPowerAcAdapter { set => _dictionary[LOW_POWER_AC_ADAPTER] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool DisplayOn { set => _dictionary[DISPLAY_ON] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool ExternalDisplayConnected { set => _dictionary[EXTERNAL_DISPLAY_CONNECTED] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool GameRunning { set => _dictionary[GAME_RUNNING] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool HDROn { set => _dictionary[HDR_ON] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool LidOpen { set => _dictionary[LID_OPEN] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool Startup { set => _dictionary[STARTUP] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool Resume { set => _dictionary[RESUME] = value ? VALUE_TRUE : VALUE_FALSE; }

    public PowerModeState PowerMode
    {
        set
        {
            _dictionary[POWER_MODE] = value switch
            {
                PowerModeState.Quiet => "1",
                PowerModeState.Balance => "2",
                PowerModeState.Performance => "3",
                PowerModeState.GodMode => "255",
                _ => string.Empty
            };
            _dictionary[POWER_MODE_NAME] = value switch
            {
                PowerModeState.Quiet => "QUIET",
                PowerModeState.Balance => "BALANCE",
                PowerModeState.Performance => "PERFORMANCE",
                PowerModeState.GodMode => "CUSTOM",
                _ => string.Empty
            };
        }
    }

    public bool ProcessesStarted { set => _dictionary[PROCESSES_STARTED] = value ? VALUE_TRUE : VALUE_FALSE; }

    public ProcessInfo[] Processes { set => _dictionary[PROCESSES] = string.Join(",", value.Select(p => p.Name)); }

    public bool DeviceConnected { set => _dictionary[DEVICE_CONNECTED] = value ? VALUE_TRUE : VALUE_FALSE; }

    public string[] DeviceInstanceIds { set => _dictionary[DEVICE_INSTANCE_IDS] = string.Join(",", value); }

    public bool IsSunset { set => _dictionary[IS_SUNSET] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool IsSunrise { set => _dictionary[IS_SUNRISE] = value ? VALUE_TRUE : VALUE_FALSE; }

    public Time? Time { set => _dictionary[TIME] = value is null ? null : $"{value.Value.Hour}:{value.Value.Minute}"; }

    public DayOfWeek[] Days { set => _dictionary[DAYS] = value.Length < 1 ? null : string.Join(",", value.Select(v => v.ToString().ToUpperInvariant())); }

    public TimeSpan Period { set => _dictionary[PERIOD] = $"{(int)value.TotalSeconds}"; }

    public bool UserActive { set => _dictionary[USER_ACTIVE] = value ? VALUE_TRUE : VALUE_FALSE; }

    public bool WiFiConnected { set => _dictionary[WIFI_CONNECTED] = value ? VALUE_TRUE : VALUE_FALSE; }

    public string? WiFiSsid { set => _dictionary[WIFI_SSID] = value; }

    public Dictionary<string, string?> Dictionary => new(_dictionary);

    private readonly Dictionary<string, string?> _dictionary = [];
}
