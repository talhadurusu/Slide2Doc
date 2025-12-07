using FluentAssertions;
using Slide2Doc.Application.Services;
using Slide2Doc.Domain.Entities;
using Slide2Doc.Domain.Enums;

namespace Slide2Doc.Application.Tests;

public class TopicMatcherTests
{
    [Fact]
    public void Match_ShouldPairTopicsWithBestSection()
    {
        var topics = new List<Topic>
        {
            new() { Order = 1, Title = "Introduction" },
            new() { Order = 2, Title = "Solution Overview" }
        };

        var sections = new List<DocumentSection>
        {
            new() { HeadingText = "Solution Overview", HeadingLevel = 1, StartIndex = 0, EndIndex = 1, SourceDocName = "a.docx" },
            new() { HeadingText = "Other", HeadingLevel = 1, StartIndex = 2, EndIndex = 3, SourceDocName = "a.docx" },
            new() { HeadingText = "Introduction to Slide2Doc", HeadingLevel = 1, StartIndex = 4, EndIndex = 5, SourceDocName = "b.docx" }
        };

        var matcher = new TopicMatcher();

        var result = matcher.Match(topics, sections);

        result.Should().HaveCount(2);
        result[0].Section?.HeadingText.Should().Be("Introduction to Slide2Doc");
        result[0].Status.Should().Be(MatchStatus.Matched);
        result[1].Section?.HeadingText.Should().Be("Solution Overview");
    }
}
