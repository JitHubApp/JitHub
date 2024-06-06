namespace JitHub.Services.Interfaces;

public interface INavigationService
{
    public void Unauthorized();
    public void NavigateTo(string title, Type page);
    public void NavigateTo(string title, Type page, object parameter);
    public void GoHome();
    public void RepoNagivateTo(Type page);
    public void RepoNagivateTo(Type page, object parameter);
}
