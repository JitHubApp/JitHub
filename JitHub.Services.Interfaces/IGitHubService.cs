namespace JitHub.Services.Interfaces;

public interface IGitHubService
{
    public Task<object> RunRequest(string requestUrl);
}
