# Slide2Doc

Slide2Doc is a Blazor Server application that compiles a DOCX document from a PPTX outline and one or more DOCX source documents. It extracts topics from slides, indexes headings from sources, matches topics to sections, and produces a compiled DOCX without writing intermediate files to disk.

## Project Structure
```
/src
  /Slide2Doc.Domain        // Domain entities and enums
  /Slide2Doc.Application   // Use cases and ports
  /Slide2Doc.Infrastructure// OpenXML implementations
  /Slide2Doc.Web           // Blazor Server UI
/tests
  /Slide2Doc.Application.Tests
  /Slide2Doc.Infrastructure.Tests
.github/workflows/ci.yml
```

## How it works
1. **Upload**: The PPTX (outline) and one or more DOCX sources are uploaded. Inputs are validated (extension, content type, size, max file count) and kept in memory.
2. **Extract**: `OpenXmlPptxTopicExtractor` reads slide titles (title placeholder fallback to first text run).
3. **Index**: `OpenXmlDocxSectionIndexer` walks DOCX body paragraphs, capturing Heading1/2/3 sections with their element ranges and source file name.
4. **Match**: `TopicMatcher` normalizes text and performs exact/contains matching to pair topics with document sections.
5. **Compose**: `OpenXmlDocxComposer` builds a new DOCX, adds each topic as Heading1, and copies matched section content (paragraphs/tables). Content stays in streams—no disk writes.
6. **Download**: The composed DOCX is streamed to the browser via a Blazor page and JavaScript download helper.

## Run locally
1. Install .NET 8 SDK.
2. Restore and build:
   ```bash
   dotnet restore Slide2Doc.sln
   dotnet build Slide2Doc.sln
   ```
3. Run the Blazor Server app:
   ```bash
   dotnet run --project src/Slide2Doc.Web/Slide2Doc.Web.csproj
   ```
4. Open `https://localhost:5001` (or shown URL). Navigate through Upload → Mapping → Generate.

## Tests
Run unit and integration tests:
```bash
dotnet test Slide2Doc.sln
```

## CI
GitHub Actions workflow (`.github/workflows/ci.yml`) restores, builds, and tests the solution.

## Architecture Notes
- **Domain**: Topic, DocumentSection, TopicMatch, CompileOptions plus enums for heading and match status.
- **Application**: Interfaces for PPTX extraction, DOCX indexing, matching, composing, and a `CompileDocumentService` orchestrating prepare/compose steps.
- **Infrastructure**: OpenXML-based implementations for PPTX topic extraction, DOCX heading indexing, and DOCX composing (paragraph/table level only).
- **Web**: Blazor Server UI with in-memory state per session, FluentValidation for uploads, logging, and centralized error messaging.

## Limitations
- Images and advanced styling are not copied into the output (paragraphs and tables only).
- Fuzzy matching beyond exact/contains is not implemented in the MVP.
- Table of contents rendering is a placeholder.
- Upload size limits are enforced but can be tuned in `UploadRequestValidator`.

## Roadmap
- Preserve styles and media during composition.
- Add fuzzy matching and manual drag/drop mapping UX.
- Persist sessions or allow download of mapping configuration.
- Add timeout/streaming safeguards for very large files.
