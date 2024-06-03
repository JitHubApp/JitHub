using JitHub.Services.Interfaces;
using Windows.Storage;

namespace JitHub.Services.Common;

public class SettingsService : ISettingsService
{
    private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

    public T Get<T>(string key)
    {
        if (_localSettings.Values.ContainsKey(key))
        {
            return (T)_localSettings.Values[key];
        }
        return default;
    }

    public void Save(string key, object value)
    {
        _localSettings.Values[key] = value;
    }
}
