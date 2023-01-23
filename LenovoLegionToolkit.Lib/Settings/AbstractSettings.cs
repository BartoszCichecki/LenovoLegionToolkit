using System;
using System.IO;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LenovoLegionToolkit.Lib.Settings;

public abstract class AbstractSettings<T> where T : new()
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    private readonly string _settingsStorePath;

    protected abstract string FileName { get; }

    protected abstract T Default { get; }

    public T Store { get; }

    protected AbstractSettings()
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

        _settingsStorePath = Path.Combine(Folders.AppData, GetFileName());

        try
        {
            var settingsSerialized = File.ReadAllText(_settingsStorePath);
            var store = JsonConvert.DeserializeObject<T>(settingsSerialized, _jsonSerializerSettings);

            if (store is null)
            {
                TryBackup();
                store = GetDefault();
            }

            Store = store;
        }
        catch
        {
            TryBackup();
            Store = GetDefault();
        }

        SynchronizeStore();
    }

    public void SynchronizeStore()
    {
        var settingsSerialized = JsonConvert.SerializeObject(Store, _jsonSerializerSettings);
        File.WriteAllText(_settingsStorePath, settingsSerialized);
    }

    private string GetFileName() => FileName;

    private T GetDefault() => Default;

    private void TryBackup()
    {
        try
        {
            if (!File.Exists(_settingsStorePath))
                return;

            var backupFileName = $"{Path.GetFileNameWithoutExtension(FileName)}_backup_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(FileName)}";
            var backupFilePath = Path.Combine(Folders.AppData, backupFileName);
            File.Copy(_settingsStorePath, backupFilePath);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unable to create backup for {FileName}", ex);
        }
    }
}