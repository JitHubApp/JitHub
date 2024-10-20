﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Security.Credentials;
using CommunityToolkit.Mvvm.ComponentModel;
using JitHub.Services.Interfaces;

namespace JitHub.Services.Accounts;

public partial class LocalAccountService : ObservableObject, IAccountService
{
    [ObservableProperty]
    private bool _authenticated;
    private ISettingsService _settingsService;
    private INavigationService _navigationService;
    private PasswordVault _passwordVault;
    private static string userIdKey = "USER_ID";
    private static string doNotWarnDeleteRepoKey = "DO_NOT_WARN_DELETE_REPO";
    private AuthorizeService _authorize;
    public LocalAccountService(ISettingsService settingsService, INavigationService navigationService)
    {
        _settingsService = settingsService;
        _navigationService = navigationService;
        _passwordVault = new PasswordVault();
        var userId = GetUser();
        if (userId != 0)
        {
            Authenticated = CheckAuth(userId);
        }
    }

    public void RemoveUser()
    {
        _settingsService.Save(userIdKey, string.Empty);
    }

    public void SaveUser(long userId)
    {
        _settingsService.Save(userIdKey, userId);
        _settingsService.Save(doNotWarnDeleteRepoKey, false);
    }

    public long GetUser()
    {
        return _settingsService.Get<long>(userIdKey);
    }

    public bool Authorize(string token, string clientId, long userId)
    {
        try
        {
            _passwordVault.Add(new PasswordCredential(clientId, userId.ToString(), token));
            SaveUser(userId);
            Authenticated = true;
            _authorize?.Invoke(token);
        }
        catch
        {
            Authenticated = false;
        }
        return Authenticated;
    }

    public bool CheckAuth(long userId)
    {
        try
        {
            var token = GetToken(userId);
            return token != null;
        }
        catch
        {
            return false;
        }
    }

    public string GetToken(long userId)
    {
        try
        {
            var credentialList = _passwordVault.FindAllByUserName(userId.ToString());
            if (credentialList.Count > 0)
            {
                credentialList[0].RetrievePassword();
                return credentialList[0].Password;
            }
            else
            {
                return null;
            }
        }
        catch
        {
            return null;
        }
    }

    public void SignOut()
    {
        RemoveUser();
        Authenticated = false;
        _navigationService.Unauthorized();
    }

    public async Task Authenticate(IEnumerable<string> scopes)
    {
        using var httpClient = new HttpClient();
        // TODO: This is temp URL, need to change to config
        var url = $"https://localhost:7040/authenticate?scopes={string.Join(';', scopes)}";
        await Windows.System.Launcher.LaunchUriAsync(new(url));
    }

    public void RegisterAuthorizitableService(AuthorizeService authorize)
    {
        _authorize = authorize;
    }
}
