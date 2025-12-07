using Slide2Doc.Domain.Entities;

namespace Slide2Doc.Application.Interfaces;

public interface IDocxSectionIndexer
{
    Task<IReadOnlyList<DocumentSection>> IndexAsync(Stream docx, string name, CancellationToken cancellationToken);
}
