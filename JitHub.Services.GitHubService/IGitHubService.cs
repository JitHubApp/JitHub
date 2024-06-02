namespace JitHub.Services.GitHub;

public interface IGitHubService
{
    public Task<object> RunRequest(string requestUrl);
}
