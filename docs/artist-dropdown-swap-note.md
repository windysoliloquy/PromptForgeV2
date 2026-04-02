# Artist Dropdown Swap Note

## Problem

In the artist swap feature, the prompt state could swap correctly while the visible editable `ComboBox` fields still appeared unchanged.

This happened in the primary and secondary artist selectors in:

- `PromptForge.App/MainWindow.xaml`
- `PromptForge.App/ViewModels/MainWindowViewModel.cs`

## Why the usual swap was not enough

Swapping through the normal property setters alone was not reliably updating the visible text in the editable artist dropdowns.

Even when:

- `ArtistInfluencePrimary` was set to the previous secondary value
- `ArtistInfluenceSecondary` was set to the previous primary value

the UI could keep showing the old text.

The important detail is that these controls are editable `ComboBox` controls, so their displayed text can drift from the underlying selected item during fast back-to-back slot updates.

## Fix that worked

The reliable fix was:

1. Update the backing fields directly inside the swap routine:
   - `_artistInfluencePrimary`
   - `_artistInfluenceSecondary`
2. After both fields are updated, explicitly raise:
   - `OnPropertyChanged(nameof(ArtistInfluencePrimary))`
   - `OnPropertyChanged(nameof(ArtistInfluenceSecondary))`
3. Then run the normal downstream refresh steps:
   - `ApplyArtistNegativeConstraintDefaults()`
   - prompt regeneration
   - tooltip refresh
   - phrase override retargeting

This avoids intermediate setter churn and forces the UI to redraw from the final swapped state.

## Supporting XAML note

The editable artist `ComboBox` controls were also bound with both:

- `SelectedItem="{Binding ArtistInfluencePrimary/Secondary}"`
- `Text="{Binding ArtistInfluencePrimary/Secondary, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"`

That helps keep the editable text aligned with the bound property, but the decisive fix for the visible swap issue was the explicit field swap plus manual `PropertyChanged` notifications.

## If this comes up again

If another editable dropdown or slot-swap feature behaves similarly:

- prefer updating both backing fields first
- then raise property-changed notifications once for the final values
- avoid relying only on sequential setter calls when two editable slot controls are swapping values with each other

## Relevant code

- `PromptForge.App/ViewModels/MainWindowViewModel.cs`
- `PromptForge.App/MainWindow.xaml`
