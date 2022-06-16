using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class ApplicationSettings
    {
        private class ApplicationSettingsStore
        {
            public WindowSize WindowSize { get; set; }
            public Theme Theme { get; set; } = Theme.Dark;
            public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
            public bool MinimizeOnClose { get; set; } = false;
            public bool ActivatePowerProfilesWithVantageEnabled { get; set; } = false;
        }

        private readonly ApplicationSettingsStore _settingsStore;

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

        public ApplicationSettings()
        {
            _jsonSerializerSettings = new()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                Converters =
                {
                    new StringEnumConverter(),
                }
            };
            _settingsStorePath = Path.Combine(Folders.AppData, "settings.json");

            try
            {
                var settingsSerialized = File.ReadAllText(_settingsStorePath);
                _settingsStore = JsonConvert.DeserializeObject<ApplicationSettingsStore>(settingsSerialized, _jsonSerializerSettings) ?? new();
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
