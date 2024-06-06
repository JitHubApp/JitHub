namespace JitHub.Services.Interfaces;

public interface IAccountService
{
    bool Authenticated { get; set; }
    void RemoveUser();
    void SaveUser(int userId);
    int GetUser();
    Task Authenticate(IEnumerable<string> scopes);
    public bool Authorize(string token, string clientId, int userId);
    string GetToken(int userId);
    bool CheckAuth(int userId);
    void SignOut();
}
