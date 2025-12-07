using System.Globalization;
using System.Text;
using Slide2Doc.Application.Interfaces;
using Slide2Doc.Domain.Entities;
using Slide2Doc.Domain.Enums;

namespace Slide2Doc.Application.Services;

public class TopicMatcher : ITopicMatcher
{
    public IReadOnlyList<TopicMatch> Match(IReadOnlyList<Topic> topics, IReadOnlyList<DocumentSection> sections)
    {
        var matches = new List<TopicMatch>();

        foreach (var topic in topics.OrderBy(t => t.Order))
        {
            var normalizedTopic = Normalize(topic.Title);
            var best = sections
                .Select(section => new
                {
                    Section = section,
                    Score = CalculateScore(normalizedTopic, Normalize(section.HeadingText))
                })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            if (best != null && best.Score > 0)
            {
                matches.Add(new TopicMatch
                {
                    Topic = topic,
                    Section = best.Section,
                    Score = best.Score,
                    Status = MatchStatus.Matched
                });
            }
            else
            {
                matches.Add(new TopicMatch
                {
                    Topic = topic,
                    Section = null,
                    Score = 0,
                    Status = MatchStatus.NotFound
                });
            }
        }

        return matches;
    }

    private static int CalculateScore(string topic, string heading)
    {
        if (string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(heading))
        {
            return 0;
        }

        if (topic.Equals(heading, StringComparison.OrdinalIgnoreCase))
        {
            return 100;
        }

        if (heading.Contains(topic, StringComparison.OrdinalIgnoreCase))
        {
            return 75;
        }

        if (topic.Contains(heading, StringComparison.OrdinalIgnoreCase))
        {
            return 60;
        }

        return 0;
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        value = value.Normalize(NormalizationForm.FormKD);
        var builder = new StringBuilder();
        foreach (var c in value)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            {
                builder.Append(c);
            }
        }

        return builder.ToString().ToLower(CultureInfo.InvariantCulture).Trim();
    }
}
