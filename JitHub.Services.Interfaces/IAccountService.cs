namespace JitHub.Services.Interfaces;

public interface IAccountService
{
    bool Authenticated { get; set; }
    void RemoveUser();
    void SaveUser(long userId);
    long GetUser();
    Task Authenticate(IEnumerable<string> scopes);
    public bool Authorize(string token, string clientId, long userId);
    string GetToken(long userId);
    bool CheckAuth(long userId);
    void SignOut();
}
