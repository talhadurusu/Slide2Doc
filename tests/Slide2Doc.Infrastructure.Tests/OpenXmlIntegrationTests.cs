using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Slide2Doc.Application.Interfaces;
using Slide2Doc.Application.Models;
using Slide2Doc.Application.Services;
using Slide2Doc.Domain.Entities;
using Slide2Doc.Infrastructure.Composers;
using Slide2Doc.Infrastructure.Extractors;
using Slide2Doc.Infrastructure.Indexers;

namespace Slide2Doc.Infrastructure.Tests;

public class OpenXmlIntegrationTests
{
    [Fact]
    public async Task DocxSectionIndexer_ShouldFindHeadings()
    {
        using var docStream = CreateSampleDocx();
        IDocxSectionIndexer indexer = new OpenXmlDocxSectionIndexer();

        var sections = await indexer.IndexAsync(docStream, "sample.docx", CancellationToken.None);

        sections.Should().NotBeEmpty();
        sections.First().HeadingText.Should().Be("Introduction");
        sections.First().HeadingLevel.Should().Be(1);
    }

    [Fact]
    public async Task Pipeline_ShouldComposeDocument()
    {
        using var pptx = CreateSamplePptx();
        using var docx = CreateSampleDocx();

        IPptxTopicExtractor extractor = new OpenXmlPptxTopicExtractor();
        IDocxSectionIndexer indexer = new OpenXmlDocxSectionIndexer();
        ITopicMatcher matcher = new TopicMatcher();
        IDocxComposer composer = new OpenXmlDocxComposer();
        var compileService = new CompileDocumentService(extractor, indexer, matcher, composer, NullLogger<CompileDocumentService>.Instance);

        var source = new SourceDocument("sample.docx", docx.ToArray());
        var prep = await compileService.PrepareAsync(pptx, new List<SourceDocument> { source }, new CompileOptions(), CancellationToken.None);
        var output = await compileService.ComposeAsync(prep.Matches, new List<SourceDocument> { source }, new CompileOptions(), CancellationToken.None);

        using var resultDoc = WordprocessingDocument.Open(output, false);
        var text = resultDoc.MainDocumentPart!.Document!.Body!.InnerText;
        text.Should().Contain("Introduction");
    }

    private static MemoryStream CreateSampleDocx()
    {
        var stream = new MemoryStream();
        using var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true);
        var mainPart = document.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var body = mainPart.Document.Body!;

        var heading1 = new Paragraph(new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" }), new Run(new Text("Introduction")));
        var para1 = new Paragraph(new Run(new Text("Welcome to Slide2Doc.")));
        body.Append(heading1, para1);

        var heading2 = new Paragraph(new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" }), new Run(new Text("Details")));
        var para2 = new Paragraph(new Run(new Text("Details go here.")));
        body.Append(heading2, para2);

        mainPart.Document.Save();
        stream.Position = 0;
        return stream;
    }

    private static MemoryStream CreateSamplePptx()
    {
        var stream = new MemoryStream();
        using var presentation = PresentationDocument.Create(stream, PresentationDocumentType.Presentation);
        var presentationPart = presentation.AddPresentationPart();
        presentationPart.Presentation = new Presentation();
        var slideIdList = new SlideIdList();
        presentationPart.Presentation.Append(slideIdList);

        for (int i = 0; i < 2; i++)
        {
            var slidePart = presentationPart.AddNewPart<SlidePart>($"rId{i + 1}");
            slidePart.Slide = new Slide(new CommonSlideData(new ShapeTree()));
            var shapeTree = slidePart.Slide.CommonSlideData!.ShapeTree!;
            var titleShape = new Shape(
                new NonVisualShapeProperties(
                    new NonVisualDrawingProperties { Id = (UInt32Value)(2u), Name = "Title" },
                    new NonVisualShapeDrawingProperties(),
                    new ApplicationNonVisualDrawingProperties(new PlaceholderShape { Type = PlaceholderValues.Title })),
                new ShapeProperties(),
                new TextBody(new BodyProperties(), new ListStyle(), new Paragraph(new DocumentFormat.OpenXml.Drawing.Run(new DocumentFormat.OpenXml.Drawing.Text(i == 0 ? "Introduction" : "Details")))));
            shapeTree.AppendChild(titleShape);

            slidePart.Slide.Save();
            var slideId = new SlideId { Id = (UInt32Value)(256u + (uint)i), RelationshipId = presentationPart.GetIdOfPart(slidePart) };
            slideIdList.Append(slideId);
        }

        presentationPart.Presentation.Save();
        stream.Position = 0;
        return stream;
    }
}
