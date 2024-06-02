using Octokit.GraphQL;
using static Octokit.GraphQL.Variable;

namespace JitHub.Services.GitHub;

public struct Repo
{
    public ID Id { get; set; }
    public string Name { get; set; }
    public string Owner { get; set; }
    public bool IsFork { get; set; }
    public bool IsPrivate { get; set; }
}

public sealed class GitHubService : IGitHubService
{
    private string _token;
    public GitHubService(string token)
    {
        _token = token;
    }

    public async Task<Repo> GetRepo(string @params)
    {
        var parts = @params.Split('&');
        var owner = parts[0].Split('=')[1];
        var name = parts[1].Split('=')[1];
        return await GetRepo(owner, name);
    }

    public async Task<Repo> GetRepo(string owner, string name)
    {
        var productInformation = new ProductHeaderValue("JitHub", "V3");
        var connection = new Connection(productInformation, _token);

        var query = new Query()
            .RepositoryOwner(Var("owner"))
            .Repository(Var("name"))
            .Select(repo => new
            {
                repo.Id,
                repo.Name,
                Owner = repo.Owner.Login,
                repo.IsFork,
                repo.IsPrivate,
            }).Compile();

        var vars = new Dictionary<string, object>
        {
            { "owner",  owner },
            { "name",  name },
        };

        var result = await connection.Run(query, vars);
        return new()
        {
            Id = result.Id,
            Name = result.Name,
            Owner = result.Owner,
            IsFork = result.IsFork,
            IsPrivate = result.IsPrivate,
        };
    }

    public async Task<object> RunRequest(string requestUrl)
    {
        var parts = requestUrl.Split('?');
        var functionName = parts?[0];
        if (string.IsNullOrWhiteSpace(functionName) && parts?.Length < 2)
        {
            throw new ArgumentException($"Invalid Request URL {requestUrl}");
        }
        switch (functionName)
        {
            case nameof(GetRepo):
                return await GetRepo(parts[1]);
            default:
                throw new ArgumentException($"Unknown function name {requestUrl}");
        }
    }
}
