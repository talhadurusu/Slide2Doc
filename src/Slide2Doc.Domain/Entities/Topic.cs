namespace Slide2Doc.Domain.Entities;

/// <summary>
/// Represents a topic extracted from a presentation slide deck.
/// </summary>
public record Topic
{
    public int Order { get; init; }

    public string Title { get; init; } = string.Empty;

    public int? SlideNumber { get; init; }
}
