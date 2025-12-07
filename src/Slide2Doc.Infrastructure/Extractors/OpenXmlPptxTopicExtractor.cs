using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Slide2Doc.Application.Interfaces;
using Slide2Doc.Domain.Entities;

namespace Slide2Doc.Infrastructure.Extractors;

public class OpenXmlPptxTopicExtractor : IPptxTopicExtractor
{
    public async Task<IReadOnlyList<Topic>> ExtractAsync(Stream pptx, CancellationToken cancellationToken)
    {
        var topics = new List<Topic>();
        pptx.Position = 0;
        await using var cloned = new MemoryStream();
        await pptx.CopyToAsync(cloned, cancellationToken);
        cloned.Position = 0;

        using var presentation = PresentationDocument.Open(cloned, false);
        var presentationPart = presentation.PresentationPart;
        if (presentationPart?.Presentation?.SlideIdList == null)
        {
            return topics;
        }

        int order = 1;
        foreach (var slideId in presentationPart.Presentation.SlideIdList.OfType<SlideId>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var slidePart = (SlidePart?)presentationPart.GetPartById(slideId.RelationshipId!);
            if (slidePart?.Slide == null)
            {
                continue;
            }

            var titleText = ExtractTitleFromSlide(slidePart.Slide);
            topics.Add(new Topic
            {
                Order = order++,
                Title = titleText,
                SlideNumber = order - 1
            });
        }

        return topics;
    }

    private static string ExtractTitleFromSlide(Slide slide)
    {
        var titleShape = slide.Descendants<Shape>()
            .FirstOrDefault(s => s.NonVisualShapeProperties?.ApplicationNonVisualDrawingProperties?.PlaceholderShape?.Type?.Value == PlaceholderValues.Title);

        var text = titleShape?.TextBody?.InnerText;
        if (!string.IsNullOrWhiteSpace(text))
        {
            return text.Trim();
        }

        var firstRunText = slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>()
            .Select(t => t.Text)
            .FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));

        return firstRunText?.Trim() ?? string.Empty;
    }
}
