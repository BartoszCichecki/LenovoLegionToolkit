using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LenovoLegionToolkit.Lib
{
    public class Settings
    {
        private class SettingsStore
        {
            public Theme Theme { get; set; } = Theme.System;
            public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
            public bool MinimizeOnClose { get; set; } = true;
            public bool ActivatePowerProfilesWithVantageEnabled { get; set; } = false;
        }

        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new();
                return _instance;
            }
        }

        private SettingsStore _settingsStore;

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly string _settingsStorePath;

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
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folderPath = Path.Combine(appData, "LenovoLegionToolkit");
            Directory.CreateDirectory(folderPath);

            _jsonSerializerOptions = new() { WriteIndented = true };
            _settingsStorePath = Path.Combine(folderPath, "settings.json");

            Deserialize();
        }

        public void Synchronize() => Serialize();

        private void Deserialize()
        {
            try
            {
                var settingsSerialized = File.ReadAllText(_settingsStorePath);
                _settingsStore = JsonSerializer.Deserialize<SettingsStore>(settingsSerialized, _jsonSerializerOptions);
            }
            catch
            {
                _settingsStore = new();
                Serialize();
            }
        }

        private void Serialize()
        {
            var settingsSerialized = JsonSerializer.Serialize(_settingsStore, _jsonSerializerOptions);
            File.WriteAllText(_settingsStorePath, settingsSerialized);
        }
    }
}
