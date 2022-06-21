using System.IO;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LenovoLegionToolkit.Lib.Settings
{
    public abstract class AbstractSettings<T> where T : new()
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly string _settingsStorePath;

        protected abstract string FileName { get; }

        public T Store { get; }

        public abstract T Default { get; }

        public AbstractSettings()
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
            _settingsStorePath = Path.Combine(Folders.AppData, FileName);

            try
            {
                var settingsSerialized = File.ReadAllText(_settingsStorePath);
                Store = JsonConvert.DeserializeObject<T>(settingsSerialized, _jsonSerializerSettings) ?? Default;
            }
            catch
            {
                Store = Default;
                SynchronizeStore();
            }
        }

        public void SynchronizeStore()
        {
            var settingsSerialized = JsonConvert.SerializeObject(Store, _jsonSerializerSettings);
            File.WriteAllText(_settingsStorePath, settingsSerialized);
        }
    }
}
