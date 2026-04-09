# Lane Checkbox Slider Suppression Pattern

Reference note for future Codex lane work.

Use this pattern when a lane sidecar checkbox should suppress prompt output from existing sliders. Do not add a hidden builder-only suppression path if the user wants the existing slider popout "exclude from prompt" behavior to be the source of truth.

## Proven Example

Editorial Illustration:

- Sidecar checkbox: `Black and White / Monochrome`
- Config property: `EditorialIllustrationBlackAndWhiteMonochrome`
- Overlay phrase: `black-and-white monochrome treatment`
- Suppressed sliders: `Saturation`, `Temperature`

The checkbox adds its overlay phrase through the lane module and toggles the existing slider exclusion flags in `MainWindowViewModel`.

## Files Involved

- `PromptForge.Core/Services/LaneRegistry.cs`
  - Add the checkbox as a normal lane `Modifier(...)`.
  - Point it at a boolean property on `PromptConfiguration`.

- `PromptForge.Core/Models/PromptConfiguration.cs`
- `PromptForge.App/Models/PromptConfiguration.cs`
  - Add the boolean checkbox state and clone/copy it.

- `PromptForge.Core/Services/Lanes/[Lane]Lane.cs`
  - Implement `ILanePresentationOverlayProvider` for overlay phrases.
  - Implement `ILaneSliderSuppressionProvider` to return the slider keys the checkbox wants suppressed.
  - This provider describes desired suppression; it should not itself mutate prompt slider exclusion flags.

- `PromptForge.Core/Services/SliderLanguageCatalog.[Lane].cs`
  - Keep the overlay phrase authority here.

- `PromptForge.App/ViewModels/MainWindowViewModel.cs`
  - Add the ViewModel property for the checkbox.
  - Use the normal standard-lane modifier path so the checkbox appears in the sidecar.
  - On checkbox changes, sync the provider's desired slider keys into the existing `Exclude[Slider]FromPrompt` flags.
  - Track the keys applied by that checkbox/lane so unchecking clears only exclusions it applied.
  - Preserve user-manual exclusions.

## Important Rule

Visible slider exclusion state should be the actual suppression mechanism.

Correct:

- Checkbox checked -> set existing slider exclude flag(s) to `true`.
- Checkbox unchecked -> clear only flags this checkbox applied.
- Prompt builder naturally skips those sliders because the existing exclude flags are true.

Avoid:

- Adding a parallel hidden suppression branch in `PromptBuilderService`.
- Mutating slider values.
- Clearing a user exclusion that existed before the checkbox applied its own suppression.

## Minimal ViewModel Shape

```csharp
private readonly HashSet<string> _editorialIllustrationAppliedSliderSuppressions = new(StringComparer.Ordinal);

private void SyncEditorialIllustrationSliderSuppressions()
{
    var desiredSuppressions = EditorialIllustrationLane.Instance.GetSuppressedSliders(CaptureConfiguration());

    foreach (var sliderKey in EditorialIllustrationLane.Instance.GetSuppressibleSliderKeys())
    {
        if (desiredSuppressions.Contains(sliderKey))
        {
            if (!GetSliderExclusionFlag(sliderKey))
            {
                SetSliderExclusionFlag(sliderKey, true);
                _editorialIllustrationAppliedSliderSuppressions.Add(sliderKey);
            }

            continue;
        }

        if (_editorialIllustrationAppliedSliderSuppressions.Remove(sliderKey) &&
            GetSliderExclusionFlag(sliderKey))
        {
            SetSliderExclusionFlag(sliderKey, false);
        }
    }
}
```

When adding new suppressible slider keys, make sure `GetSliderExclusionFlag(...)` and `SetSliderExclusionFlag(...)` know how to read/write the matching existing `Exclude[Slider]FromPrompt` property.
