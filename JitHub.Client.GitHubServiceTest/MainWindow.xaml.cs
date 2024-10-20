using JitHub.Services.GitHub;
using Microsoft.UI.Xaml;

namespace JitHub.Client.GitHubServiceTest;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var token = TokenService.Token;
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }
        //var githubService = new GitHubService(token);
        //var input = RepoInput.Text;
        //var textParts = input.Split('/');
        //var owner = textParts[0];
        //var name = textParts[1];
        //var repo = await githubService.GetRepo(owner, name);
        //Id.Text = repo.Id.ToString();
        //Owner.Text = repo.Owner;
        //Name.Text = repo.Name;
        //IsFork.Text = repo.IsFork ? "Fork" : "Original";
        //IsPrivate.Text = repo.IsPrivate ? "Private" : "Public";
    }
}
