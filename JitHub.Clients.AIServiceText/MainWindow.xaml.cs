using Microsoft.UI.Xaml;
using JitHub.Services.AI;
using JitHub.Services.Common;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JitHub.Clients.AIServiceText;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private LocalSLMService _aiService;
    private DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public MainWindow()
    {
        this.InitializeComponent();
        ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        _aiService = new LocalSLMService(new SettingsService());
    }

    private void AppendText(string text)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            var existingText = ChatText.Text;
            ChatText.Text = $"{existingText}{text}";
        });   
    }

    private void PromptBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        var ctrlState = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control);
        if (ctrlState.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down) && e.Key == Windows.System.VirtualKey.Enter)
        {
            e.Handled = true;
            if (string.IsNullOrWhiteSpace(PromptBox.Text))
            {
                return;
            }
            ChatText.Text = "";
            var prompt = PromptBox.Text;
            PromptBox.Text = "";
            Task.Run(() =>
            {
                _aiService.SetModelPath(SLMName.PHI3_MINI_4K, "E:\\\\Phi-3-mini-4k-instruct-onnx\\\\directml\\\\directml-int4-awq-block-128");
                using var session = _aiService.LoadModel(SLMName.PHI3_MINI_4K);
                session.WriteNextToken = AppendText;
                session.Complete(prompt);
            });
        }
    }
}
