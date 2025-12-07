using Slide2Doc.Application.Models;
using Slide2Doc.Domain.Entities;
using Slide2Doc.Domain.Enums;

namespace Slide2Doc.Web.Services;

public class AppState
{
    public List<Topic> Topics { get; } = new();

    public List<DocumentSection> Sections { get; } = new();

    public List<TopicMatch> Matches { get; } = new();

    public List<SourceDocument> Sources { get; } = new();

    public CompileOptions Options { get; set; } = new CompileOptions { IncludeToc = false, MaxHeadingLevel = 3 };

    public void Reset()
    {
        Topics.Clear();
        Sections.Clear();
        Matches.Clear();
        Sources.Clear();
        Options = new CompileOptions { IncludeToc = false, MaxHeadingLevel = 3 };
    }
}
