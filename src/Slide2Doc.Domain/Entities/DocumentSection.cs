namespace Slide2Doc.Domain.Entities;

/// <summary>
/// Represents a heading section extracted from a Word document.
/// </summary>
public record DocumentSection
{
    public string HeadingText { get; init; } = string.Empty;

    public int HeadingLevel { get; init; }

    public int StartIndex { get; init; }

    public int EndIndex { get; init; }

    public string SourceDocName { get; init; } = string.Empty;
}
