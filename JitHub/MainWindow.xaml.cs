using JitHub.Pages;

namespace JitHub;

public sealed partial class MainWindow : WinUIEx.WindowEx
{
    public MainWindow()
    {
        this.InitializeComponent();
        ShellFrame.Navigate(typeof(ShellPage));
    }
}
