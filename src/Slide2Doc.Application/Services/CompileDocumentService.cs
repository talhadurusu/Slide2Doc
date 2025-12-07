using Microsoft.Extensions.Logging;
using Slide2Doc.Application.Interfaces;
using Slide2Doc.Application.Models;
using Slide2Doc.Domain.Entities;

namespace Slide2Doc.Application.Services;

public class CompileDocumentService : ICompileDocumentService
{
    private readonly IPptxTopicExtractor _pptxTopicExtractor;
    private readonly IDocxSectionIndexer _docxSectionIndexer;
    private readonly ITopicMatcher _topicMatcher;
    private readonly IDocxComposer _docxComposer;
    private readonly ILogger<CompileDocumentService> _logger;

    public CompileDocumentService(
        IPptxTopicExtractor pptxTopicExtractor,
        IDocxSectionIndexer docxSectionIndexer,
        ITopicMatcher topicMatcher,
        IDocxComposer docxComposer,
        ILogger<CompileDocumentService> logger)
    {
        _pptxTopicExtractor = pptxTopicExtractor;
        _docxSectionIndexer = docxSectionIndexer;
        _topicMatcher = topicMatcher;
        _docxComposer = docxComposer;
        _logger = logger;
    }

    public async Task<CompilationPreparationResult> PrepareAsync(Stream pptx, IReadOnlyList<SourceDocument> sources, CompileOptions options, CancellationToken cancellationToken)
    {
        if (sources.Count == 0)
        {
            throw new ArgumentException("At least one source document is required.", nameof(sources));
        }

        _logger.LogInformation("Starting topic extraction from PPTX with {Count} sources", sources.Count);
        var topics = await _pptxTopicExtractor.ExtractAsync(pptx, cancellationToken);

        var sections = new List<DocumentSection>();
        foreach (var source in sources)
        {
            await using var stream = source.OpenRead();
            var indexed = await _docxSectionIndexer.IndexAsync(stream, source.Name, cancellationToken);
            sections.AddRange(indexed);
        }

        var matches = _topicMatcher.Match(topics, sections);
        return new CompilationPreparationResult(topics, sections, matches, options);
    }

    public Task<Stream> ComposeAsync(IReadOnlyList<TopicMatch> matches, IReadOnlyList<SourceDocument> sources, CompileOptions options, CancellationToken cancellationToken)
    {
        return _docxComposer.ComposeAsync(matches, sources, options, cancellationToken);
    }
}
