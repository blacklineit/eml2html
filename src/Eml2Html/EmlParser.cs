using MimeKit;

namespace Eml2Html;

/// <summary>
/// Parsed representation of an .eml file's content.
/// </summary>
public sealed class ParsedEmail
{
    public string HtmlBody { get; init; } = "";
    public List<InlineImage> InlineImages { get; init; } = [];
    public List<Attachment> Attachments { get; init; } = [];
}

public sealed class InlineImage
{
    public required string ContentId { get; init; }
    public required string MimeType { get; init; }
    public required byte[] Data { get; init; }
    public required string FileName { get; init; }
}

public sealed class Attachment
{
    public required string FileName { get; init; }
    public required byte[] Data { get; init; }
}

public static class EmlParser
{
    public static ParsedEmail Parse(string emlPath)
    {
        using var stream = File.OpenRead(emlPath);
        var message = MimeMessage.Load(stream);

        var inlineImages = new List<InlineImage>();
        var attachments = new List<Attachment>();
        var htmlBody = "";

        // Walk the MIME tree
        var mimeIterator = new MimeIterator(message);
        while (mimeIterator.MoveNext())
        {
            switch (mimeIterator.Current)
            {
                case TextPart { IsHtml: true } textPart:
                    htmlBody = textPart.Text ?? "";
                    break;
                case MimePart part:
                {
                    var isInline = part.ContentDisposition?.Disposition == ContentDisposition.Inline
                                   || part.ContentId != null;

                    if (isInline && part is { ContentId: not null, ContentType.MediaType: "image" })
                    {
                        using var ms = new MemoryStream();
                        part.Content?.DecodeTo(ms);
                        inlineImages.Add(new InlineImage
                        {
                            ContentId = part.ContentId.Trim('<', '>'),
                            MimeType = part.ContentType.MimeType,
                            Data = ms.ToArray(),
                            FileName = SanitizeFileName(part.FileName ?? $"image_{inlineImages.Count}.{GetExtension(part.ContentType)}")
                        });
                    }
                    else if (part.ContentDisposition?.Disposition == ContentDisposition.Attachment
                             || (part.FileName != null && part.ContentType.MediaType != "image"))
                    {
                        using var ms = new MemoryStream();
                        part.Content?.DecodeTo(ms);
                        attachments.Add(new Attachment
                        {
                            FileName = SanitizeFileName(part.FileName ?? $"attachment_{attachments.Count}"),
                            Data = ms.ToArray()
                        });
                    }

                    break;
                }
            }
        }

        return new ParsedEmail
        {
            HtmlBody = htmlBody,
            InlineImages = inlineImages,
            Attachments = attachments
        };
    }

    private static string SanitizeFileName(string name)
    {
        // Strip directory components to prevent path traversal
        name = Path.GetFileName(name);

        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        // Reject remaining traversal patterns
        name = name.Replace("..", "_");

        if (string.IsNullOrWhiteSpace(name))
            name = "unnamed";

        return name;
    }

    private static string GetExtension(ContentType contentType)
    {
        return contentType.MediaSubtype switch
        {
            "png" => "png",
            "jpeg" or "jpg" => "jpg",
            "gif" => "gif",
            "bmp" => "bmp",
            "webp" => "webp",
            "svg+xml" => "svg",
            _ => "bin"
        };
    }
}
