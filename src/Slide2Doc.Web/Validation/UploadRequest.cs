using Microsoft.AspNetCore.Components.Forms;

namespace Slide2Doc.Web.Validation;

public class UploadRequest
{
    public IReadOnlyList<IBrowserFile> DocxFiles { get; init; } = Array.Empty<IBrowserFile>();

    public IBrowserFile? PptxFile { get; init; }
}
