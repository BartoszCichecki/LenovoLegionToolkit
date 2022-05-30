using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Utils
{
    internal class AutomationSettings
    {
        public class AutomationSettingsStore
        {
            public bool IsEnabled { get; set; } = false;

            public List<AutomationPipeline> Pipelines { get; set; } = new()
            {
                new()
                {
                    Triggers = { AutomationPipelineTrigger.ACAdapterConnected },
                    Steps = { new PowerModeAutomationStep(PowerModeState.Balance) },
                },
                new()
                {
                    Triggers = { AutomationPipelineTrigger.ACAdapterDisconnected },
                    Steps = { new PowerModeAutomationStep(PowerModeState.Quiet) },
                },
            };
        }

        private static AutomationSettings? _instance;
        public static AutomationSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new();
                return _instance;
            }
        }

        private readonly AutomationSettingsStore _settingsStore;

        private readonly JsonSerializerOptions _jsonSerializerOptions;
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
            _jsonSerializerOptions = new() { WriteIndented = true };
            _automationStorePath = Path.Combine(Folders.AppData, "automation.json");

            try
            {
                var settingsSerialized = File.ReadAllText(_automationStorePath);
                _settingsStore = JsonSerializer.Deserialize<AutomationSettingsStore>(settingsSerialized, _jsonSerializerOptions) ?? new();
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
            File.WriteAllText(_automationStorePath, settingsSerialized);
        }
    }
}
