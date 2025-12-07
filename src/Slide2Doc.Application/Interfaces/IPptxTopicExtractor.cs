using Slide2Doc.Domain.Entities;

namespace Slide2Doc.Application.Interfaces;

public interface IPptxTopicExtractor
{
    Task<IReadOnlyList<Topic>> ExtractAsync(Stream pptx, CancellationToken cancellationToken);
}
