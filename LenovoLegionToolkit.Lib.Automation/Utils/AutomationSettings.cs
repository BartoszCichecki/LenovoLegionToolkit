using System.Collections.Generic;
using System.IO;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LenovoLegionToolkit.Lib.Automation.Utils
{
    internal class AutomationSettings
    {
        public class AutomationSettingsStore
        {
            public bool IsEnabled { get; set; } = false;

            public List<AutomationPipeline> Pipelines { get; set; } = new();
        }

        private static AutomationSettings? _instance;
        public static AutomationSettings Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new();
                return _instance;
            }
        }

        private readonly AutomationSettingsStore _settingsStore;

        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly string _automationStorePath;

        public bool IsEnabled
        {
            get => _settingsStore.IsEnabled;
            set => _settingsStore.IsEnabled = value;
        }

        public List<AutomationPipeline> Pipeliness
        {
            get => _settingsStore.Pipelines;
            set => _settingsStore.Pipelines = value;
        }

        private AutomationSettings()
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
            _automationStorePath = Path.Combine(Folders.AppData, "automation.json");

            try
            {
                var settingsSerialized = File.ReadAllText(_automationStorePath);
                _settingsStore = JsonConvert.DeserializeObject<AutomationSettingsStore>(settingsSerialized, _jsonSerializerSettings) ?? new();
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
            File.WriteAllText(_automationStorePath, settingsSerialized);
        }
    }
}
