namespace PromptForge.App.Models;

public sealed record IntentModeDefinition(
    string Name,
    int Whimsy,
    int Tension,
    int Awe,
    int Chaos,
    int MotionEnergy,
    int AtmosphericDepth,
    int NarrativeDensity,
    int Symbolism,
    int Saturation,
    int Contrast,
    string Lighting,
    string Summary);
