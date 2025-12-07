namespace Slide2Doc.Domain.Entities;

/// <summary>
/// Options that influence the compilation of the destination document.
/// </summary>
public record CompileOptions
{
    public bool IncludeToc { get; init; }

    public int MaxHeadingLevel { get; init; } = 3;

    public string OutputFileName { get; init; } = "Slide2DocOutput.docx";
}
