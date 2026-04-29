using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PromptForge.App.Services;

namespace PromptForge.App.Views.CompactWorkstation;

public partial class CompactShelfShellStudy : UserControl
{
    public static readonly DependencyProperty IsHoverDeckLauncherEnabledProperty =
        DependencyProperty.Register(
            nameof(IsHoverDeckLauncherEnabled),
            typeof(bool),
            typeof(CompactShelfShellStudy),
            new PropertyMetadata(false));

    private Window? _ownerWindow;
    public CompactShelfShellStudy()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public bool IsHoverDeckLauncherEnabled
    {
        get => (bool)GetValue(IsHoverDeckLauncherEnabledProperty);
        set => SetValue(IsHoverDeckLauncherEnabledProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _ownerWindow = Window.GetWindow(this);
        LogCompanionState("loaded");
        if (_ownerWindow is not null)
        {
            _ownerWindow.Deactivated += OnOwnerWindowDeactivated;
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        LogCompanionState("unloaded");
        DetachOwnerWindow();
    }

    private void OnSteerStubClick(object sender, RoutedEventArgs e)
    {
        LogCompanionState($"steer-click handledBefore={e.Handled}");
        if (!IsHoverDeckLauncherEnabled)
        {
            LogCompanionState("steer-click-blocked reason='launcher-disabled'");
            return;
        }

        LogCompanionState("popup-open-requested");
        LaneCardCompanionPopup.IsOpen = true;
        e.Handled = true;
        LogCompanionState($"popup-open-request-complete handledAfter={e.Handled}");
    }

    private void OnCloseLaneCardCompanionClick(object sender, RoutedEventArgs e)
    {
        LogCompanionState("popup-close-button");
        LaneCardCompanionPopup.IsOpen = false;
    }

    private void OnActionStubClick(object sender, RoutedEventArgs e)
    {
        LogCompanionState($"action-click handledBefore={e.Handled}");
        if (!IsHoverDeckLauncherEnabled)
        {
            LogCompanionState("action-click-blocked reason='launcher-disabled'");
            return;
        }

        LogCompanionState("actions-popup-open-requested");
        ActionsCompanionPopup.IsOpen = true;
        e.Handled = true;
        LogCompanionState($"actions-popup-open-request-complete handledAfter={e.Handled}");
    }

    private void OnCloseActionsCompanionClick(object sender, RoutedEventArgs e)
    {
        LogCompanionState("actions-popup-close-button");
        ActionsCompanionPopup.IsOpen = false;
    }

    private void OnOwnerWindowDeactivated(object? sender, EventArgs e)
    {
        LogCompanionState("owner-deactivated-closing-popup");
        LaneCardCompanionPopup.IsOpen = false;
        ActionsCompanionPopup.IsOpen = false;
    }

    private void OnLaneCardCompanionPopupOpened(object sender, EventArgs e)
    {
        LogCompanionState("popup-opened");
    }

    private void OnLaneCardCompanionPopupClosed(object sender, EventArgs e)
    {
        LogCompanionState("popup-closed");
        Keyboard.ClearFocus();
    }

    private void OnActionsCompanionPopupOpened(object sender, EventArgs e)
    {
        LogCompanionState("actions-popup-opened");
    }

    private void OnActionsCompanionPopupClosed(object sender, EventArgs e)
    {
        LogCompanionState("actions-popup-closed");
        Keyboard.ClearFocus();
    }
    private void DetachOwnerWindow()
    {
        if (_ownerWindow is not null)
        {
            _ownerWindow.Deactivated -= OnOwnerWindowDeactivated;
            _ownerWindow = null;
        }
    }

    private void LogCompanionState(string eventName)
    {
        UiEventLog.Write(
            $"hoverdeck-companion event='{eventName}' enabled={IsHoverDeckLauncherEnabled} lanePopupOpen={LaneCardCompanionPopup?.IsOpen ?? false} actionsPopupOpen={ActionsCompanionPopup?.IsOpen ?? false} owner='{_ownerWindow?.GetType().Name ?? "none"}' ownerState='{_ownerWindow?.WindowState.ToString() ?? "none"}' ownerVisible={_ownerWindow?.IsVisible.ToString() ?? "none"} ownerActive={_ownerWindow?.IsActive.ToString() ?? "none"}");
    }
}
