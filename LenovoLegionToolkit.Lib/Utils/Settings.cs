using System.Collections.Generic;
using System.IO;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
            public LegionRGBKey RgbProfile { get; set; } = KeyboardData.BlankLegionRGBKey;
        }

        private static Settings? _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new();
                return _instance;
            }
        }

        private readonly SettingsStore _settingsStore;

        private readonly JsonSerializerSettings _jsonSerializerSettings;
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

        public LegionRGBKey RgbProfile
        {
            get => _settingsStore.RgbProfile;
            set => _settingsStore.RgbProfile = value;
        }

        private Settings()
        {
            _jsonSerializerSettings = new()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                Converters =
                {
                    new StringEnumConverter(),
                }
            };
            _settingsStorePath = Path.Combine(Folders.AppData, "settings.json");

            try
            {
                var settingsSerialized = File.ReadAllText(_settingsStorePath);
                _settingsStore = JsonConvert.DeserializeObject<SettingsStore>(settingsSerialized, _jsonSerializerSettings) ?? new();
            }
            catch
            {
                _settingsStore = new();
                Synchronize();
            }
        }

        public void Synchronize()
        {
            var settingsSerialized = JsonConvert.SerializeObject(_settingsStore, _jsonSerializerSettings);
            File.WriteAllText(_settingsStorePath, settingsSerialized);
        }
    }
}
