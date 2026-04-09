using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

public interface ILaneSliderSuppressionProvider
{
    string IntentName { get; }

    IReadOnlySet<string> GetSuppressedSliders(PromptConfiguration configuration);

    IEnumerable<string> GetSuppressibleSliderKeys();
}
