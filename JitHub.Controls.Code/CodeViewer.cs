using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEditor;

namespace JitHub.Controls.Code;

public sealed class CodeViewer : Control
{
    private CodeEditorControl _editor;

    DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(CodeViewer),
        new PropertyMetadata(default(string), new PropertyChangedCallback(OnTextChanged)));

    DependencyProperty CodeLanguageProperty = DependencyProperty.Register(
        nameof(CodeLanguage),
        typeof(string),
        typeof(CodeViewer),
        new PropertyMetadata(default(string), new PropertyChangedCallback(OnLanguageChanged)));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string CodeLanguage
    {
        set => SetValue(LanguageProperty, value);
        get => (string)GetValue(LanguageProperty);
    }

    public CodeViewer()
    {
        this.DefaultStyleKey = typeof(CodeViewer);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        this.Loaded += OnLoaded;
    }

    private void SetEditorText(string text)
    {
        if (_editor != null)
        {
            _editor.Editor.ReadOnly = false;
            _editor.Editor.SetText(text);
            _editor.Editor.ReadOnly = true;
        }
    }

    private void SetEditorLanguage(string language)
    {
        if (_editor != null)
        {
            _editor.HighlightingLanguage = language;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _editor = GetTemplateChild("Editor") as CodeEditorControl;
        _editor.Editor.CaretStyle = CaretStyle.Invisible;
        SetEditorText(Text);
        SetEditorLanguage(CodeLanguage);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CodeViewer self)
        {
            self.SetEditorText(e.NewValue as string);
        }
    }

    private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CodeViewer self)
        {
            self.SetEditorLanguage(e.NewValue as string);
        }
    }
}
