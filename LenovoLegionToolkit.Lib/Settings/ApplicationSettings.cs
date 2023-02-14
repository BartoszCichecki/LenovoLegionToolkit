using System;
using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings;

public class ApplicationSettings : AbstractSettings<ApplicationSettings.ApplicationSettingsStore>
{
    public class Notifications
    {
        public bool CapsNumLock { get; set; }
        public bool FnLock { get; set; }
        public bool TouchpadLock { get; set; } = true;
        public bool KeyboardBacklight { get; set; } = true;
        public bool CameraLock { get; set; } = true;
        public bool Microphone { get; set; } = true;
        public bool PowerMode { get; set; }
        public bool RefreshRate { get; set; } = true;
        public bool ACAdapter { get; set; }
        public bool SmartKey { get; set; }
    }

    public class ApplicationSettingsStore
    {
        public Theme Theme { get; set; }
        public RGBColor? AccentColor { get; set; }
        public AccentColorSource AccentColorSource { get; set; }
        public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
        public bool MinimizeOnClose { get; set; }
        public WindowSize? WindowSize { get; set; }
        public bool ActivatePowerProfilesWithVantageEnabled { get; set; }
        public bool DontShowNotifications { get; set; }
        public NotificationPosition NotificationPosition { get; set; }
        public Notifications Notifications { get; set; } = new();
        public TemperatureUnit TemperatureUnit { get; set; }
        public List<RefreshRate> ExcludedRefreshRates { get; set; } = new();
        public WarrantyInfo? WarrantyInfo { get; set; }
        public Guid? SmartKeySinglePressActionId { get; set; }
        public Guid? SmartKeyDoublePressActionId { get; set; }
        public List<Guid> SmartKeySinglePressActionList { get; set; } = new();
        public List<Guid> SmartKeyDoublePressActionList { get; set; } = new();
        public bool SynchronizeBrightnessToAllPowerPlans { get; set; }
    }

    public ApplicationSettings() : base("settings.json") { }
}