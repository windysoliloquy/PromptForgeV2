using System;
using System.Collections.Generic;
using PromptForge.App.Commands;
using PromptForge.App.Models;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel
{
    private ArtistPhraseOverride _primaryArtistPhraseOverride = new();
    private ArtistPhraseOverride _secondaryArtistPhraseOverride = new();
    private bool _isArtistPhraseEditorOpen;
    private bool _isEditingPrimaryArtistPhrase;
    private IReadOnlyList<ArtistPhraseQuickInsertGroup> _artistPhraseQuickInsertGroups = Array.Empty<ArtistPhraseQuickInsertGroup>();
    private IReadOnlyList<ArtistPhraseSuffixRoleGroup> _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
    private string _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
    private string _artistPhraseEditorStructuredSuffixTrailingText = string.Empty;

    public bool IsArtistPhraseEditorOpen
    {
        get => _isArtistPhraseEditorOpen;
        private set
        {
            if (SetProperty(ref _isArtistPhraseEditorOpen, value))
            {
                SaveArtistPhraseEditorCommand.RaiseCanExecuteChanged();
                ResetArtistPhraseEditorCommand.RaiseCanExecuteChanged();
                CancelArtistPhraseEditorCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string ArtistPhraseEditorTitle => _isEditingPrimaryArtistPhrase
        ? "Edit Primary Artist Phrase"
        : "Edit Secondary Artist Phrase";

    public string ArtistPhraseEditorPrefix
    {
        get => _artistPhraseEditorPrefix;
        set
        {
            if (SetProperty(ref _artistPhraseEditorPrefix, value))
            {
                OnPropertyChanged(nameof(ArtistPhraseEditorPreview));
            }
        }
    }

    public string ArtistPhraseEditorArtistName
    {
        get => _artistPhraseEditorArtistName;
        private set
        {
            if (SetProperty(ref _artistPhraseEditorArtistName, value))
            {
                OnPropertyChanged(nameof(ArtistPhraseEditorPreview));
                SaveArtistPhraseEditorCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string ArtistPhraseEditorSuffix
    {
        get => _artistPhraseEditorSuffix;
        set
        {
            if (SetProperty(ref _artistPhraseEditorSuffix, value))
            {
                if (!string.Equals(value, _artistPhraseEditorStructuredSuffixLastRendered, StringComparison.Ordinal))
                {
                    _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
                    _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
                    _artistPhraseEditorStructuredSuffixTrailingText = value?.Trim() ?? string.Empty;
                }

                OnPropertyChanged(nameof(ArtistPhraseEditorPreview));
            }
        }
    }

    public string ArtistPhraseEditorGeneratedPhrase
    {
        get => _artistPhraseEditorGeneratedPhrase;
        private set => SetProperty(ref _artistPhraseEditorGeneratedPhrase, value);
    }

    public string ArtistPhraseEditorPreview => ArtistPhraseComposer.Combine(ArtistPhraseEditorPrefix, ArtistPhraseEditorArtistName, ArtistPhraseEditorSuffix);

    public IReadOnlyList<ArtistPhraseQuickInsertGroup> ArtistPhraseQuickInsertGroups
    {
        get => _artistPhraseQuickInsertGroups;
        private set => SetProperty(ref _artistPhraseQuickInsertGroups, value);
    }

    public bool CanEditPrimaryArtistPhrase => TryGetArtistPhraseSlotState(isPrimary: true, out _);
    public bool CanEditSecondaryArtistPhrase => TryGetArtistPhraseSlotState(isPrimary: false, out _);

    public RelayCommand EditPrimaryArtistPhraseCommand { get; }
    public RelayCommand EditSecondaryArtistPhraseCommand { get; }
    public RelayCommand InsertArtistPhraseQuickInsertCommand { get; }
    public RelayCommand SaveArtistPhraseEditorCommand { get; }
    public RelayCommand ResetArtistPhraseEditorCommand { get; }
    public RelayCommand CancelArtistPhraseEditorCommand { get; }

    private void OpenPrimaryArtistPhraseEditor()
    {
        OpenArtistPhraseEditor(isPrimary: true);
    }

    private void OpenSecondaryArtistPhraseEditor()
    {
        OpenArtistPhraseEditor(isPrimary: false);
    }

    private void OpenArtistPhraseEditor(bool isPrimary)
    {
        if (!TryGetArtistPhraseSlotState(isPrimary, out var slotState))
        {
            StatusMessage = "Select an active artist influence before editing its phrase.";
            return;
        }

        var parts = ArtistPhraseComposer.SplitPhrase(slotState.CurrentPhrase, slotState.DisplayName);
        _isEditingPrimaryArtistPhrase = isPrimary;
        OnPropertyChanged(nameof(ArtistPhraseEditorTitle));

        ArtistPhraseEditorPrefix = parts.Prefix;
        ArtistPhraseEditorArtistName = slotState.DisplayName;
        ArtistPhraseEditorSuffix = parts.Suffix;
        ArtistPhraseEditorGeneratedPhrase = slotState.GeneratedPhrase;
        _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
        _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
        _artistPhraseEditorStructuredSuffixTrailingText = parts.Suffix;
        ArtistPhraseQuickInsertGroups = ArtistPhraseQuickInsertService.BuildGroups(isPrimary, _currentArtistPairLookup);
        IsArtistPhraseEditorOpen = true;
    }

    private void InsertArtistPhraseQuickInsert(object? parameter)
    {
        if (parameter is not ArtistPhraseQuickInsert insert)
        {
            return;
        }

        switch (insert.Target)
        {
            case ArtistPhraseInsertTarget.Prefix:
                ArtistPhraseEditorPrefix = ArtistPhraseComposer.AppendFragment(string.Empty, insert.Fragment, ArtistPhraseInsertTarget.Prefix);
                break;
            case ArtistPhraseInsertTarget.Suffix:
                if (!string.IsNullOrWhiteSpace(insert.RoleStem) && !string.IsNullOrWhiteSpace(insert.DomainLabel))
                {
                    _artistPhraseEditorStructuredSuffixGroups = ArtistPhraseComposer.AddStructuredSuffixInsert(
                        _artistPhraseEditorStructuredSuffixGroups,
                        insert.RoleStem,
                        insert.DomainLabel);
                    _artistPhraseEditorStructuredSuffixLastRendered = ArtistPhraseComposer.RenderStructuredSuffix(
                        _artistPhraseEditorStructuredSuffixGroups,
                        _artistPhraseEditorStructuredSuffixTrailingText);
                    ArtistPhraseEditorSuffix = _artistPhraseEditorStructuredSuffixLastRendered;
                }
                else
                {
                    ArtistPhraseEditorSuffix = ArtistPhraseComposer.AppendFragment(ArtistPhraseEditorSuffix, insert.Fragment, ArtistPhraseInsertTarget.Suffix);
                }
                break;
        }
    }

    private void SaveArtistPhraseEditor()
    {
        var normalizedPreview = ArtistPhraseEditorPreview;
        var useGeneratedPhrase = string.Equals(
            normalizedPreview,
            ArtistPhraseEditorGeneratedPhrase,
            StringComparison.OrdinalIgnoreCase);

        if (_isEditingPrimaryArtistPhrase)
        {
            _primaryArtistPhraseOverride = useGeneratedPhrase
                ? new ArtistPhraseOverride()
                : new ArtistPhraseOverride
                {
                    IsEnabled = true,
                    ArtistName = ArtistPhraseEditorArtistName,
                    Prefix = ArtistPhraseEditorPrefix,
                    Suffix = ArtistPhraseEditorSuffix,
                };
            StatusMessage = useGeneratedPhrase
                ? "Primary artist phrase reset to generated."
                : "Primary artist phrase updated.";
        }
        else
        {
            _secondaryArtistPhraseOverride = useGeneratedPhrase
                ? new ArtistPhraseOverride()
                : new ArtistPhraseOverride
                {
                    IsEnabled = true,
                    ArtistName = ArtistPhraseEditorArtistName,
                    Prefix = ArtistPhraseEditorPrefix,
                    Suffix = ArtistPhraseEditorSuffix,
                };
            StatusMessage = useGeneratedPhrase
                ? "Secondary artist phrase reset to generated."
                : "Secondary artist phrase updated.";
        }

        IsArtistPhraseEditorOpen = false;
        RegeneratePrompt();
    }

    private void ResetArtistPhraseEditorToGenerated()
    {
        if (!TryGetArtistPhraseSlotState(_isEditingPrimaryArtistPhrase, out var slotState))
        {
            return;
        }

        var parts = ArtistPhraseComposer.SplitPhrase(slotState.GeneratedPhrase, slotState.DisplayName);
        ArtistPhraseEditorPrefix = parts.Prefix;
        ArtistPhraseEditorArtistName = slotState.DisplayName;
        ArtistPhraseEditorSuffix = parts.Suffix;
        ArtistPhraseEditorGeneratedPhrase = slotState.GeneratedPhrase;
        _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
        _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
        _artistPhraseEditorStructuredSuffixTrailingText = parts.Suffix;
    }

    private void CancelArtistPhraseEditor()
    {
        IsArtistPhraseEditorOpen = false;
        ArtistPhraseQuickInsertGroups = Array.Empty<ArtistPhraseQuickInsertGroup>();
        _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
        _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
        _artistPhraseEditorStructuredSuffixTrailingText = string.Empty;
    }

    private bool TryGetArtistPhraseSlotState(bool isPrimary, out ArtistPhraseSlotState slotState)
    {
        var selectedArtist = isPrimary ? ArtistInfluencePrimary : ArtistInfluenceSecondary;
        var strength = isPrimary ? InfluenceStrengthPrimary : InfluenceStrengthSecondary;
        var phraseOverride = isPrimary ? _primaryArtistPhraseOverride : _secondaryArtistPhraseOverride;

        if (!HasActiveArtist(selectedArtist, strength))
        {
            slotState = new ArtistPhraseSlotState(string.Empty, 0, string.Empty, string.Empty);
            return false;
        }

        var profile = _artistProfileService.GetProfile(selectedArtist);
        var displayName = profile?.Name ?? selectedArtist;
        var generatedPhrase = ArtistPhraseComposer.BuildGeneratedPhrase(displayName, strength, profile is not null, IntentMode);
        var currentPhrase = ArtistPhraseComposer.BuildFinalPhrase(displayName, strength, profile is not null, phraseOverride, IntentMode);

        slotState = new ArtistPhraseSlotState(displayName, strength, generatedPhrase, currentPhrase);
        return true;
    }

    private void HandleArtistSelectionChanged(bool isPrimary)
    {
        if (_isApplyingConfiguration || _suspendArtistOverrideReset)
        {
            return;
        }

        if (isPrimary)
        {
            _primaryArtistPhraseOverride = new ArtistPhraseOverride();
        }
        else
        {
            _secondaryArtistPhraseOverride = new ArtistPhraseOverride();
        }

        if (IsArtistPhraseEditorOpen && _isEditingPrimaryArtistPhrase == isPrimary)
        {
            IsArtistPhraseEditorOpen = false;
            ArtistPhraseQuickInsertGroups = Array.Empty<ArtistPhraseQuickInsertGroup>();
        }

        RaiseArtistPhraseEditorAvailabilityChanged();
    }

    private void RaiseArtistPhraseEditorAvailabilityChanged()
    {
        OnPropertyChanged(nameof(CanEditPrimaryArtistPhrase));
        OnPropertyChanged(nameof(CanEditSecondaryArtistPhrase));
        OnPropertyChanged(nameof(CanSwapArtistInfluences));
        EditPrimaryArtistPhraseCommand.RaiseCanExecuteChanged();
        EditSecondaryArtistPhraseCommand.RaiseCanExecuteChanged();
        SwapArtistInfluencesCommand.RaiseCanExecuteChanged();
    }

    private void UpdateArtistPhraseOverrideTargetNames()
    {
        UpdateArtistPhraseOverrideTargetName(_primaryArtistPhraseOverride, ArtistInfluencePrimary);
        UpdateArtistPhraseOverrideTargetName(_secondaryArtistPhraseOverride, ArtistInfluenceSecondary);
    }

    private void UpdateArtistPhraseOverrideTargetName(ArtistPhraseOverride phraseOverride, string selectedArtist)
    {
        if (phraseOverride is null || !phraseOverride.IsEnabled)
        {
            return;
        }

        if (!HasSelectedArtistValue(selectedArtist))
        {
            phraseOverride.IsEnabled = false;
            phraseOverride.ArtistName = string.Empty;
            phraseOverride.Prefix = string.Empty;
            phraseOverride.Suffix = string.Empty;
            return;
        }

        var profile = _artistProfileService.GetProfile(selectedArtist);
        phraseOverride.ArtistName = profile?.Name ?? selectedArtist;
    }

    private sealed record ArtistPhraseSlotState(string DisplayName, int Strength, string GeneratedPhrase, string CurrentPhrase);
}
