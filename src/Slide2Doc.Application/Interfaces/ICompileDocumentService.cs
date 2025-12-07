using Slide2Doc.Application.Models;

namespace Slide2Doc.Application.Interfaces;

public interface ICompileDocumentService
{
    Task<CompilationPreparationResult> PrepareAsync(Stream pptx, IReadOnlyList<SourceDocument> sources, CompileOptions options, CancellationToken cancellationToken);

    Task<Stream> ComposeAsync(IReadOnlyList<TopicMatch> matches, IReadOnlyList<SourceDocument> sources, CompileOptions options, CancellationToken cancellationToken);
}
