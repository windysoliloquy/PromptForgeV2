using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PromptForge.App.Views.CompactWorkstation;

public partial class HoverDeckArtistPhraseEditorHost : UserControl
{
    private bool _isDraggingArtistPhraseEditor;
    private Point _artistPhraseEditorDragStart;
    private double _artistPhraseEditorStartHorizontalOffset;
    private double _artistPhraseEditorStartVerticalOffset;

    public HoverDeckArtistPhraseEditorHost()
    {
        InitializeComponent();
        SizeChanged += OnHostSizeChanged;
    }

    private void OnHostSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ClampArtistPhraseEditorPopupToBounds();
    }

    private void OnArtistPhraseEditorPopupOpened(object sender, EventArgs e)
    {
        CenterArtistPhraseEditorPopup();
    }

    private void OnArtistPhraseEditorPopupClosed(object sender, EventArgs e)
    {
        _isDraggingArtistPhraseEditor = false;
        ArtistPhraseEditorDragHandle.ReleaseMouseCapture();
        Keyboard.ClearFocus();
    }

    private void OnArtistPhraseEditorDragHandleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!HoverDeckArtistPhraseEditorPopup.IsOpen)
        {
            return;
        }

        _isDraggingArtistPhraseEditor = true;
        _artistPhraseEditorDragStart = e.GetPosition(HostRoot);
        _artistPhraseEditorStartHorizontalOffset = HoverDeckArtistPhraseEditorPopup.HorizontalOffset;
        _artistPhraseEditorStartVerticalOffset = HoverDeckArtistPhraseEditorPopup.VerticalOffset;
        ArtistPhraseEditorDragHandle.CaptureMouse();
        e.Handled = true;
    }

    private void OnArtistPhraseEditorDragHandleMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDraggingArtistPhraseEditor || !ArtistPhraseEditorDragHandle.IsMouseCaptured)
        {
            return;
        }

        var currentPosition = e.GetPosition(HostRoot);
        var delta = currentPosition - _artistPhraseEditorDragStart;
        HoverDeckArtistPhraseEditorPopup.HorizontalOffset = _artistPhraseEditorStartHorizontalOffset + delta.X;
        HoverDeckArtistPhraseEditorPopup.VerticalOffset = _artistPhraseEditorStartVerticalOffset + delta.Y;
    }

    private void OnArtistPhraseEditorDragHandleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDraggingArtistPhraseEditor)
        {
            return;
        }

        _isDraggingArtistPhraseEditor = false;
        ArtistPhraseEditorDragHandle.ReleaseMouseCapture();
        e.Handled = true;
    }

    private void CenterArtistPhraseEditorPopup()
    {
        if (HostRoot is null || ArtistPhraseEditorPopupCard is null)
        {
            return;
        }

        Dispatcher.BeginInvoke(() =>
        {
            ArtistPhraseEditorPopupCard.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var popupSize = ArtistPhraseEditorPopupCard.DesiredSize;
            var availableWidth = Math.Max(0d, HostRoot.ActualWidth);
            var availableHeight = Math.Max(0d, HostRoot.ActualHeight);

            HoverDeckArtistPhraseEditorPopup.HorizontalOffset = Math.Max(0d, (availableWidth - popupSize.Width) * 0.5);
            HoverDeckArtistPhraseEditorPopup.VerticalOffset = Math.Max(0d, (availableHeight - popupSize.Height) * 0.5);
            ClampArtistPhraseEditorPopupToBounds();
        }, DispatcherPriority.Loaded);
    }

    private void ClampArtistPhraseEditorPopupToBounds()
    {
        if (HostRoot is null || ArtistPhraseEditorPopupCard is null || !HoverDeckArtistPhraseEditorPopup.IsOpen)
        {
            return;
        }

        ArtistPhraseEditorPopupCard.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var popupSize = ArtistPhraseEditorPopupCard.DesiredSize;
        var maxHorizontalOffset = Math.Max(0d, HostRoot.ActualWidth - popupSize.Width);
        var maxVerticalOffset = Math.Max(0d, HostRoot.ActualHeight - popupSize.Height);

        HoverDeckArtistPhraseEditorPopup.HorizontalOffset = Math.Clamp(HoverDeckArtistPhraseEditorPopup.HorizontalOffset, 0d, maxHorizontalOffset);
        HoverDeckArtistPhraseEditorPopup.VerticalOffset = Math.Clamp(HoverDeckArtistPhraseEditorPopup.VerticalOffset, 0d, maxVerticalOffset);
    }
}
