# Prompt Forge

Prompt Forge is a native Windows desktop app for building high-quality image prompts through structured controls instead of manual prompt writing.

## Stack

- C#
- .NET 8
- WPF
- MVVM

## Open in Visual Studio

Open [PromptForge.sln](C:/Users/windy/OneDrive/Desktop/Prompt%20Forge/PromptForge.sln) in Visual Studio 2022 or later with the .NET 8 desktop workload installed.

## Build and run

1. Restore and build the solution.
2. Set `PromptForge.App` as the startup project if needed.
3. Run with `F5` or `Ctrl+F5`.

## Presets

Presets are saved as JSON in `%AppData%\PromptForge\Presets`.

## Notes

This workspace does not currently expose a local `.NET` SDK or MSBuild toolchain in the shell, so I could not execute a local compile step here. The solution and project files are set up for Visual Studio/.NET 8 desktop development.
