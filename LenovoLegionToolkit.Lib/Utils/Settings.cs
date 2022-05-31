using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib
{
    public class Settings
    {
        private class SettingsStore
        {
            public WindowSize WindowSize { get; set; }
            public Theme Theme { get; set; } = Theme.Dark;
            public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
            public bool MinimizeOnClose { get; set; } = false;
            public bool ActivatePowerProfilesWithVantageEnabled { get; set; } = false;
        }

        private static Settings? _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new();
                return _instance;
            }
        }

        private readonly SettingsStore _settingsStore;

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly string _settingsStorePath;

        public WindowSize WindowSize
        {
            get => _settingsStore.WindowSize;
            set => _settingsStore.WindowSize = value;
        }

        public Theme Theme
        {
            get => _settingsStore.Theme;
            set => _settingsStore.Theme = value;
        }

        public Dictionary<PowerModeState, string> PowerPlans
        {
            get => _settingsStore.PowerPlans;
            set => _settingsStore.PowerPlans = value;
        }

        public bool MinimizeOnClose
        {
            get => _settingsStore.MinimizeOnClose;
            set => _settingsStore.MinimizeOnClose = value;
        }

        public bool ActivatePowerProfilesWithVantageEnabled
        {
            get => _settingsStore.ActivatePowerProfilesWithVantageEnabled;
            set => _settingsStore.ActivatePowerProfilesWithVantageEnabled = value;
        }

        private Settings()
        {
            _jsonSerializerOptions = new() { WriteIndented = true };
            _settingsStorePath = Path.Combine(Folders.AppData, "settings.json");

            try
            {
                var settingsSerialized = File.ReadAllText(_settingsStorePath);
                _settingsStore = JsonSerializer.Deserialize<SettingsStore>(settingsSerialized, _jsonSerializerOptions) ?? new();
            }
            catch
            {
                _settingsStore = new();
                Synchronize();
            }
        }

        public void Synchronize()
        {
            var settingsSerialized = JsonSerializer.Serialize(_settingsStore, _jsonSerializerOptions);
            File.WriteAllText(_settingsStorePath, settingsSerialized);
        }
    }
}
