using CommunityToolkit.Mvvm.Input;
using JitHub.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace JitHub.ViewModels;

internal partial class LoginPageViewModel
{
    private IAccountService _accountService;

    public LoginPageViewModel()
    {
        _accountService = ((App)App.Current).ServiceProvider.GetService<IAccountService>();
    }

    [RelayCommand]
    public void Login()
    {
        _accountService.Authenticate(["user", "repo", "delete_repo"]);
    }
}
