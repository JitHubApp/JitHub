using JitHub.Pages;
using Microsoft.UI.Xaml.Controls;

namespace JitHub;

public sealed partial class MainWindow : WinUIEx.WindowEx
{
    public MainWindow()
    {
        this.InitializeComponent();
        RootFrame.Navigate(typeof(ShellPage));
    }

    public Frame GetRootFrame()
    {
        return RootFrame;
    }
}
