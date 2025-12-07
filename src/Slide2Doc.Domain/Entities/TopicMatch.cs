using Slide2Doc.Domain.Enums;

namespace Slide2Doc.Domain.Entities;

/// <summary>
/// Represents the relationship between a topic and a document section.
/// </summary>
public record TopicMatch
{
    public Topic Topic { get; init; } = new();

    public DocumentSection? Section { get; init; }

    public int Score { get; init; }

    public MatchStatus Status { get; init; }
}
