using System.Windows;
using System.Windows.Input;
using PromptForge.App.Services;

namespace PromptForge.App;

public partial class HoverDeckCardWindow : Window
{
    public const double PreferredWindowWidth = 522d;
    public const double PreferredWindowHeight = 925d;
    public const double PreferredContentWidth = 486d;
    public const double MaximumWindowScale = 2d;
    public const double MaximumWindowWidth = PreferredWindowWidth * MaximumWindowScale;
    public const double MaximumWindowHeight = PreferredWindowHeight * MaximumWindowScale;

    public HoverDeckCardWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
        LocationChanged += OnLocationChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LogSize("loaded");
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        LogSize("size-changed");
    }

    private void OnLocationChanged(object? sender, EventArgs e)
    {
        LogSize("location-changed");
    }

    private void OnTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState != MouseButtonState.Pressed)
        {
            return;
        }

        BeginDragMove(e);
    }

    private void OnContentMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        ShutdownFromHoverDeckClose();
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (!Application.Current.Dispatcher.HasShutdownStarted)
        {
            ShutdownFromHoverDeckClose();
        }

        base.OnClosing(e);
    }

    private static void ShutdownFromHoverDeckClose()
    {
        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.ShutdownFromHoverDeck();
            return;
        }

        Application.Current.Shutdown();
    }

    private void BeginDragMove(MouseButtonEventArgs e)
    {
        try
        {
            DragMove();
            e.Handled = true;
        }
        catch (InvalidOperationException)
        {
        }
    }

    private void LogSize(string eventName)
    {
        UiEventLog.Write(
            $"hoverdeck-size event='{eventName}' left={Left:0.##} top={Top:0.##} width={Width:0.##} height={Height:0.##} actualWidth={ActualWidth:0.##} actualHeight={ActualHeight:0.##} minWidth={MinWidth:0.##} minHeight={MinHeight:0.##} windowState='{WindowState}'");
    }
}
