using JitHub.Services.Interfaces;
using Microsoft.UI.Xaml.Controls;
using System;

namespace JitHub.Services.Common;

public class NavigationService : INavigationService
{
    private Frame _rootFrame;

    public void Init(Frame rootFrame)
    {
        _rootFrame = rootFrame;
    }

    public void NavigateTo(string title, Type page)
    {
        // TODO: do something with the title
        _rootFrame.Navigate(page);
    }

    public void NavigateTo(string title, Type page, object parameter)
    {
        throw new NotImplementedException();
    }

    public void RepoNagivateTo(Type page)
    {
        throw new NotImplementedException();
    }

    public void RepoNagivateTo(Type page, object parameter)
    {
        throw new NotImplementedException();
    }

    public void Unauthorized()
    {
        throw new NotImplementedException();
    }
}
