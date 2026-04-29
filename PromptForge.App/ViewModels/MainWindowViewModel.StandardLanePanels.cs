using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PromptForge.App.Services;
using PromptForge.App.ViewModels.Lanes;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel
{
    public StandardLanePanelViewModel? ActiveStandardLanePanel => _sharedLanePanels.TryGetValue(IntentMode, out var panel) ? panel : null;
    public bool ShowActiveStandardLanePanel => !IsDemoExpired && !IsLockedLaneActive && ActiveStandardLanePanel is not null;

    private StandardLanePanelViewModel BuildStandardLanePanel(string intentName)
    {
        var definition = LaneRegistry.GetByIntentName(intentName)
            ?? throw new InvalidOperationException($"Lane definition for intent '{intentName}' was not found.");

        var laneState = _ordinaryLaneStates.GetOrAddLane(definition.Id);
        var laneStateViewModel = new StandardLaneStateViewModel(
            laneState,
            SetCompatibilityStringProperty,
            SetCompatibilityBoolProperty);

        return new StandardLanePanelViewModel(definition, laneStateViewModel);
    }

    private IReadOnlyDictionary<string, StandardLanePanelViewModel> BuildSharedLanePanels()
    {
        return new Dictionary<string, StandardLanePanelViewModel>(StringComparer.OrdinalIgnoreCase)
        {
            [IntentModeCatalog.CinematicName] = BuildStandardLanePanel(IntentModeCatalog.CinematicName),
            [IntentModeCatalog.ChildrensBookName] = BuildStandardLanePanel(IntentModeCatalog.ChildrensBookName),
            [IntentModeCatalog.ConceptArtName] = BuildStandardLanePanel(IntentModeCatalog.ConceptArtName),
            [IntentModeCatalog.ArchitectureArchvizName] = BuildStandardLanePanel(IntentModeCatalog.ArchitectureArchvizName),
            [IntentModeCatalog.FoodPhotographyName] = BuildStandardLanePanel(IntentModeCatalog.FoodPhotographyName),
            [IntentModeCatalog.InfographicDataVisualizationName] = BuildStandardLanePanel(IntentModeCatalog.InfographicDataVisualizationName),
            [IntentModeCatalog.LifestyleAdvertisingPhotographyName] = BuildStandardLanePanel(IntentModeCatalog.LifestyleAdvertisingPhotographyName),
            [IntentModeCatalog.PhotographyName] = BuildStandardLanePanel(IntentModeCatalog.PhotographyName),
            [IntentModeCatalog.ProductPhotographyName] = BuildStandardLanePanel(IntentModeCatalog.ProductPhotographyName),
            [IntentModeCatalog.PixelArtName] = BuildStandardLanePanel(IntentModeCatalog.PixelArtName),
            [IntentModeCatalog.FantasyIllustrationName] = BuildStandardLanePanel(IntentModeCatalog.FantasyIllustrationName),
            [IntentModeCatalog.EditorialIllustrationName] = BuildStandardLanePanel(IntentModeCatalog.EditorialIllustrationName),
            [IntentModeCatalog.GraphicDesignName] = BuildStandardLanePanel(IntentModeCatalog.GraphicDesignName),
            [IntentModeCatalog.TattooArtName] = BuildStandardLanePanel(IntentModeCatalog.TattooArtName),
            [IntentModeCatalog.ThreeDRenderName] = BuildStandardLanePanel(IntentModeCatalog.ThreeDRenderName),
            [IntentModeCatalog.WatercolorName] = BuildStandardLanePanel(IntentModeCatalog.WatercolorName),
        };
    }

    private static ObservableCollection<string> BuildSubtypeCollection(string intentName, string selectorKey = SharedLaneKeys.StyleSelector)
    {
        return new ObservableCollection<string>(LaneRegistry.GetSubtypeLabels(intentName, selectorKey));
    }

    private void SetCompatibilityStringProperty(string propertyName, string value)
    {
        var property = GetType().GetProperty(propertyName)
            ?? throw new InvalidOperationException($"View-model property '{propertyName}' was not found.");

        property.SetValue(this, value);
    }

    private void SetCompatibilityBoolProperty(string propertyName, bool value)
    {
        var property = GetType().GetProperty(propertyName)
            ?? throw new InvalidOperationException($"View-model property '{propertyName}' was not found.");

        property.SetValue(this, value);
    }

    private void SyncStandardLanePanels()
    {
        SyncStandardLanePanelStates();
        foreach (var panel in _sharedLanePanels.Values)
        {
            panel.SyncFromSource();
        }
    }

    private void SyncStandardLanePanelStates()
    {
        foreach (var panel in _sharedLanePanels.Values)
        {
            panel.ReplaceState(_ordinaryLaneStates.GetOrAddLane(panel.LaneId));
        }
    }
}
