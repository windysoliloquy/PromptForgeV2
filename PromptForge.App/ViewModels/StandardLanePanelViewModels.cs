using System.ComponentModel;
using System.Collections.ObjectModel;
using PromptForge.App.Models;

namespace PromptForge.App.ViewModels;

public sealed class StandardLanePanelViewModel : ViewModelBase
{
    private readonly StandardLaneStateViewModel _laneState;
    private readonly string _defaultDescription;

    public StandardLanePanelViewModel(LaneDefinition definition, StandardLaneStateViewModel laneState)
    {
        _laneState = laneState;
        LaneId = definition.Id;
        Title = definition.Panel.Title;
        _defaultDescription = definition.Panel.ModifierDescription;
        AccentSectionTitle = definition.Panel.AccentSectionTitle ?? string.Empty;
        Layout = definition.Panel.Layout;
        SubtypeSelectors = new ObservableCollection<StandardLaneSubtypeSelectorViewModel>(definition.SubtypeSelectors.Select(selector => new StandardLaneSubtypeSelectorViewModel(_laneState, selector)));
        foreach (var selector in SubtypeSelectors)
        {
            selector.PropertyChanged += OnSubtypeSelectorPropertyChanged;
        }

        Modifiers = new ObservableCollection<StandardLaneModifierViewModel>(definition.Modifiers.Select(modifier => new StandardLaneModifierViewModel(_laneState, modifier)));
    }

    public string LaneId { get; }
    public string Title { get; }
    public string Description => SubtypeSelectors
        .Select(selector => selector.SelectedOptionDescription)
        .FirstOrDefault(description => !string.IsNullOrWhiteSpace(description))
        ?? _defaultDescription;
    public string AccentSectionTitle { get; }
    public LanePanelLayout Layout { get; }
    public ObservableCollection<StandardLaneSubtypeSelectorViewModel> SubtypeSelectors { get; }
    public ObservableCollection<StandardLaneModifierViewModel> Modifiers { get; }
    public bool HasSubtypeSelectors => SubtypeSelectors.Count > 0;
    public bool HasModifiers => Modifiers.Count > 0;
    public bool HasAccentSectionTitle => HasModifiers && !string.IsNullOrWhiteSpace(AccentSectionTitle);

    public void ReplaceState(StandardLaneState state)
    {
        _laneState.ReplaceState(state);
        SyncFromSource();
    }

    public void SyncFromSource()
    {
        foreach (var selector in SubtypeSelectors)
        {
            selector.SyncFromSource();
        }

        foreach (var modifier in Modifiers)
        {
            modifier.SyncFromSource();
        }

        OnPropertyChanged(nameof(Description));
    }

    private void OnSubtypeSelectorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(StandardLaneSubtypeSelectorViewModel.SelectedValue), StringComparison.Ordinal) ||
            string.Equals(e.PropertyName, nameof(StandardLaneSubtypeSelectorViewModel.SelectedOptionDescription), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(Description));
        }
    }
}

public sealed class StandardLaneStateViewModel
{
    private readonly Action<string, string> _compatibilityStringSetter;
    private readonly Action<string, bool> _compatibilityBoolSetter;
    private StandardLaneState _state;

    public StandardLaneStateViewModel(
        StandardLaneState state,
        Action<string, string> compatibilityStringSetter,
        Action<string, bool> compatibilityBoolSetter)
    {
        _state = state;
        _compatibilityStringSetter = compatibilityStringSetter;
        _compatibilityBoolSetter = compatibilityBoolSetter;
    }

    public void ReplaceState(StandardLaneState state)
    {
        _state = state;
    }

    public IReadOnlyList<StandardLaneSubtypeOptionViewModel> GetSelectorOptions(LaneSubtypeSelectorDefinition selector)
    {
        return selector.Options.Select(static option => new StandardLaneSubtypeOptionViewModel(option.Key, option.Label, option.SupportDescriptorHint ?? string.Empty)).ToArray();
    }

    public string GetSelectorValue(LaneSubtypeSelectorDefinition selector)
    {
        return _state.GetSelector(selector.Key, GetDefaultSelectorValue(selector));
    }

    public void SetSelectorValue(LaneSubtypeSelectorDefinition selector, string value)
    {
        _state.SetSelector(selector.Key, value);
        _compatibilityStringSetter(selector.SelectedValuePropertyName, value);
    }

    public bool GetModifierValue(LaneModifierDefinition modifier)
    {
        return _state.GetModifier(modifier.Key, modifier.DefaultState);
    }

    public void SetModifierValue(LaneModifierDefinition modifier, bool value)
    {
        _state.SetModifier(modifier.Key, value);
        _compatibilityBoolSetter(modifier.StatePropertyName, value);
    }

    private static string GetDefaultSelectorValue(LaneSubtypeSelectorDefinition selector)
    {
        return selector.Options.FirstOrDefault(static option => option.IsDefault)?.Key
            ?? selector.Options.FirstOrDefault()?.Key
            ?? string.Empty;
    }
}

public sealed record StandardLaneSubtypeOptionViewModel(string Key, string Label, string Description);

public sealed class StandardLaneSubtypeSelectorViewModel : ViewModelBase
{
    private readonly StandardLaneStateViewModel _laneState;
    private readonly LaneSubtypeSelectorDefinition _selector;
    private string _selectedValue;

    public StandardLaneSubtypeSelectorViewModel(StandardLaneStateViewModel laneState, LaneSubtypeSelectorDefinition selector)
    {
        _laneState = laneState;
        _selector = selector;
        Label = selector.Label;
        Options = new ObservableCollection<StandardLaneSubtypeOptionViewModel>(laneState.GetSelectorOptions(selector));
        _selectedValue = laneState.GetSelectorValue(selector);
    }

    public string Label { get; }
    public ObservableCollection<StandardLaneSubtypeOptionViewModel> Options { get; }
    public string SelectedOptionDescription => Options.FirstOrDefault(option => string.Equals(option.Key, SelectedValue, StringComparison.Ordinal))?.Description ?? string.Empty;

    public string SelectedValue
    {
        get => _selectedValue;
        set
        {
            if (SetProperty(ref _selectedValue, value))
            {
                _laneState.SetSelectorValue(_selector, value);
                OnPropertyChanged(nameof(SelectedOptionDescription));
            }
        }
    }

    public void SyncFromSource()
    {
        var current = _laneState.GetSelectorValue(_selector);
        if (!string.Equals(_selectedValue, current, StringComparison.Ordinal))
        {
            _selectedValue = current;
            OnPropertyChanged(nameof(SelectedValue));
            OnPropertyChanged(nameof(SelectedOptionDescription));
        }
    }
}

public sealed class StandardLaneModifierViewModel : ViewModelBase
{
    private readonly StandardLaneStateViewModel _laneState;
    private readonly LaneModifierDefinition _modifier;
    private bool _isChecked;

    public StandardLaneModifierViewModel(StandardLaneStateViewModel laneState, LaneModifierDefinition modifier)
    {
        _laneState = laneState;
        _modifier = modifier;
        Label = modifier.Label;
        _isChecked = laneState.GetModifierValue(modifier);
    }

    public string Label { get; }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (SetProperty(ref _isChecked, value))
            {
                _laneState.SetModifierValue(_modifier, value);
            }
        }
    }

    public void SyncFromSource()
    {
        var current = _laneState.GetModifierValue(_modifier);
        if (_isChecked != current)
        {
            _isChecked = current;
            OnPropertyChanged(nameof(IsChecked));
        }
    }
}
