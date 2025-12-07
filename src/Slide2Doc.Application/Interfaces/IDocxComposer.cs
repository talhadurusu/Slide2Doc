using Slide2Doc.Domain.Entities;
using Slide2Doc.Application.Models;

namespace Slide2Doc.Application.Interfaces;

public interface IDocxComposer
{
    Task<Stream> ComposeAsync(IReadOnlyList<TopicMatch> matches, IReadOnlyList<SourceDocument> sources, CompileOptions options, CancellationToken cancellationToken);
}
