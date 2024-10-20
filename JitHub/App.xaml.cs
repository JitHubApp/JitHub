using ABI.System;
using JitHub.Pages;
using JitHub.Services.Accounts;
using JitHub.Services.AI;
using JitHub.Services.Common;
using JitHub.Services.GitHub;
using JitHub.Services.Interfaces;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Web;
using Windows.ApplicationModel.Activation;
using WinUIEx;

namespace JitHub;
public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }
    public App()
    {
        this.InitializeComponent();
        ServiceProvider = RegisterServices();
    }

    private IServiceProvider RegisterServices()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("Category", LogLevel.Information));
        services.AddApplicationInsightsTelemetryWorkerService((ApplicationInsightsServiceOptions options) => options.ConnectionString = "InstrumentationKey=a6ae7767-cb9e-4394-ae9e-e71a3b815df7;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/;LiveEndpoint=https://westus.livediagnostics.monitor.azure.com/;ApplicationId=69a80d62-b1fb-4c02-9a1b-b979cd72a297");

        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAccountService, LocalAccountService>();
        services.AddSingleton<IGitHubService, GitHubService>();
        services.AddScoped<LocalSLMService>();

        return services.BuildServiceProvider();
    }

    public void FocusMainWindow()
    {
        if (m_window != null)
        {
            try
            {
                var dispatcherQueue = m_window.DispatcherQueue;
                dispatcherQueue.TryEnqueue(() =>
                {
                    m_window.BringToFront();
                });
            }
            catch(System.Exception e)
            {
            }
        }
    }

    public void FocusMainWindowWithArguments(AppActivationArguments args)
    {
        FocusMainWindow();
        HandleActivation(args);
    }

    private void HandleActivation(AppActivationArguments activatedEventArgs)
    {
        var navigationService = ServiceProvider.GetService<INavigationService>();
        var accountService = ServiceProvider.GetService<IAccountService>();
        ((NavigationService)navigationService).Init(((MainWindow)m_window).GetRootFrame());

        var authenticated = false;
        
        if (activatedEventArgs.Kind == ExtendedActivationKind.Protocol && activatedEventArgs.Data is ProtocolActivatedEventArgs protocolArgs)
        {
            var query = protocolArgs.Uri.Query;
            var queryParameters = HttpUtility.ParseQueryString(query);

            // Access individual parameters by key
            string token = queryParameters["token"];
            string clientId = queryParameters["clientId"];
            string userId = queryParameters["userId"];
            long userIdVal;
            long.TryParse(userId, out userIdVal);
            
            authenticated = accountService.Authorize(token, clientId, userIdVal);
        }
        else
        {
            authenticated = accountService.Authenticated;
        }

        var dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();


        if (authenticated)
        {
            dispatcherQueue.TryEnqueue(() => {
                navigationService.NavigateTo("Home", typeof(ShellPage));
            });
        }
        else
        {
            dispatcherQueue.TryEnqueue(() => {
                navigationService.NavigateTo("Login", typeof(LoginPage));
            });
        }
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        if (m_window == null)
        {
            m_window = new MainWindow();
        }
        m_window.Activate();
        var activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        HandleActivation(activatedEventArgs);
    }

    private WindowEx m_window;
}
