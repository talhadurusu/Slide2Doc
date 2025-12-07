using Slide2Doc.Domain.Entities;

namespace Slide2Doc.Application.Models;

public record CompilationPreparationResult(
    IReadOnlyList<Topic> Topics,
    IReadOnlyList<DocumentSection> Sections,
    IReadOnlyList<TopicMatch> Matches,
    CompileOptions Options);
