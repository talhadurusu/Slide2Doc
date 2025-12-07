using Slide2Doc.Domain.Entities;

namespace Slide2Doc.Application.Interfaces;

public interface ITopicMatcher
{
    IReadOnlyList<TopicMatch> Match(IReadOnlyList<Topic> topics, IReadOnlyList<DocumentSection> sections);
}
