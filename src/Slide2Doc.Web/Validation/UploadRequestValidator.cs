using System.IO;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace Slide2Doc.Web.Validation;

public class UploadRequestValidator : AbstractValidator<UploadRequest>
{
    private static readonly string[] AllowedDocxContentTypes =
    {
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/msword"
    };

    private static readonly string[] AllowedPptxContentTypes =
    {
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "application/vnd.ms-powerpoint"
    };

    public const long MaxFileSizeBytes = 15 * 1024 * 1024;
    public const int MaxDocCount = 5;

    public UploadRequestValidator()
    {
        RuleFor(x => x.PptxFile)
            .NotNull().WithMessage("PPTX file is required")
            .Must(file => file != null && IsValidPptx(file)).WithMessage("Invalid PPTX file");

        RuleFor(x => x.DocxFiles)
            .NotEmpty().WithMessage("At least one DOCX is required")
            .Must(files => files.Count <= MaxDocCount).WithMessage($"You can upload up to {MaxDocCount} DOCX files")
            .Must(files => files.All(IsValidDocx)).WithMessage("One or more DOCX files are invalid");
    }

    private static bool IsValidDocx(IBrowserFile file)
    {
        return file.Size <= MaxFileSizeBytes &&
               AllowedDocxContentTypes.Contains(file.ContentType) &&
               Path.GetExtension(file.Name).Equals(".docx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidPptx(IBrowserFile file)
    {
        return file.Size <= MaxFileSizeBytes &&
               AllowedPptxContentTypes.Contains(file.ContentType) &&
               Path.GetExtension(file.Name).Equals(".pptx", StringComparison.OrdinalIgnoreCase);
    }
}
