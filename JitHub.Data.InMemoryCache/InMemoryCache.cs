using JitHub.Services.GitHub;

namespace JitHub.Data.Caching;

public delegate void OnCacheInvalidation(object value);

public readonly struct CacheObject
{
    public readonly object? Value { get; init; }
    public readonly TimeSpan RefreshPeriod { get; init; }
    public readonly OnCacheInvalidation OnCacheInvalidation { get; init; }
}

public sealed class InMemoryCache(IGitHubService _gitHubService)
{
    private Dictionary<string, CacheObject> _cache = new Dictionary<string, CacheObject>();
    private Dictionary<string, Timer> _timers = new Dictionary<string, Timer>();
    
    public void Subscribe<T>(string requestUrl, TimeSpan interval, OnCacheInvalidation onCacheInvalidation)
    {
        if (interval.TotalNanoseconds != 0)
        {
            var intervalMs = (int)interval.TotalMilliseconds;
            var timer = new Timer(RefreshCache, requestUrl, 0, intervalMs);
        }
        _cache.Add(requestUrl, new CacheObject
        {
            RefreshPeriod = interval,
            OnCacheInvalidation = onCacheInvalidation,
        });
    }

    public void UnSubscribe(string requestUrl)
    {
        if (_timers.ContainsKey(requestUrl))
        {
            var timer = _timers[requestUrl];
            timer.Dispose();
        }
        _cache.Remove(requestUrl);
    }

    private async void RefreshCache(object requestUrl)
    {
        var requestUrlString = requestUrl.ToString();
        if (_cache.ContainsKey(requestUrlString))
        {
            var original = _cache[requestUrlString];
            // make the request to the service layer
            // update cache
            // invalidate cache by calling the registered callback
            // might need to use the UI thread's dispatch queue to call it
            var result = await _gitHubService.RunRequest(requestUrlString);
            var current = original with
            {
                Value = result
            };
            _cache[requestUrlString] = current;
            current.OnCacheInvalidation(result);
        }
        else
        {
            if (_timers.ContainsKey(requestUrlString))
            {
                var timer = _timers[requestUrlString];
                timer.Dispose();
            }
        }
    }
}
