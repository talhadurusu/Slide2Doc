namespace Slide2Doc.Application.Models;

/// <summary>
/// Represents a uploaded source document and provides stream access without touching disk.
/// </summary>
public class SourceDocument
{
    public SourceDocument(string name, byte[] data)
    {
        Name = name;
        Data = data;
    }

    public string Name { get; }

    public byte[] Data { get; }

    public Stream OpenRead() => new MemoryStream(Data, writable: false);
}
