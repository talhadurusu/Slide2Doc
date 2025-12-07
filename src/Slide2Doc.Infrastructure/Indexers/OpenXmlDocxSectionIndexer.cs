using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Slide2Doc.Application.Interfaces;
using Slide2Doc.Domain.Entities;

namespace Slide2Doc.Infrastructure.Indexers;

public class OpenXmlDocxSectionIndexer : IDocxSectionIndexer
{
    public async Task<IReadOnlyList<DocumentSection>> IndexAsync(Stream docx, string name, CancellationToken cancellationToken)
    {
        await using var cloned = new MemoryStream();
        docx.Position = 0;
        await docx.CopyToAsync(cloned, cancellationToken);
        cloned.Position = 0;

        using var document = WordprocessingDocument.Open(cloned, false);
        var body = document.MainDocumentPart?.Document?.Body;
        if (body == null)
        {
            return Array.Empty<DocumentSection>();
        }

        var bodyElements = body.Elements<OpenXmlElement>().ToList();
        var sections = new List<DocumentSection>();

        for (int i = 0; i < bodyElements.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (bodyElements[i] is Paragraph paragraph)
            {
                var headingLevel = GetHeadingLevel(paragraph);
                if (headingLevel.HasValue)
                {
                    sections.Add(new DocumentSection
                    {
                        HeadingText = paragraph.InnerText.Trim(),
                        HeadingLevel = headingLevel.Value,
                        StartIndex = i,
                        EndIndex = bodyElements.Count - 1,
                        SourceDocName = name
                    });
                }
            }
        }

        for (int index = 0; index < sections.Count; index++)
        {
            var current = sections[index];
            var next = sections.Skip(index + 1)
                .FirstOrDefault(s => s.SourceDocName == current.SourceDocName && s.HeadingLevel <= current.HeadingLevel);
            sections[index] = current with { EndIndex = next != null ? next.StartIndex - 1 : current.EndIndex };
        }

        return sections;
    }

    private static int? GetHeadingLevel(Paragraph paragraph)
    {
        var styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
        if (string.IsNullOrWhiteSpace(styleId))
        {
            return null;
        }

        if (styleId.StartsWith("Heading1", StringComparison.OrdinalIgnoreCase))
        {
            return 1;
        }

        if (styleId.StartsWith("Heading2", StringComparison.OrdinalIgnoreCase))
        {
            return 2;
        }

        if (styleId.StartsWith("Heading3", StringComparison.OrdinalIgnoreCase))
        {
            return 3;
        }

        return null;
    }
}
