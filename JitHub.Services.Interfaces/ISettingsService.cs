namespace JitHub.Services.Interfaces;

public interface ISettingsService
{
    public T Get<T>(string key);
    public void Save(string key, object value);
}
