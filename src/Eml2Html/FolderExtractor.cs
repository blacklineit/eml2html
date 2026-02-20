using System.Text.RegularExpressions;

namespace Eml2Html;

public static partial class FolderExtractor
{
    /// <summary>
    /// Extracts email content to a subfolder: HTML with relative image links, images/, and attachments/.
    /// </summary>
    public static void Extract(string emlPath, ParsedEmail email)
    {
        var baseName = Path.GetFileNameWithoutExtension(emlPath);
        var baseDir = Path.GetDirectoryName(emlPath)!;
        var outputDir = Path.Combine(baseDir, baseName);
        var imagesDir = Path.Combine(outputDir, "images");
        var attachmentsDir = Path.Combine(outputDir, "attachments");

        Directory.CreateDirectory(outputDir);

        var html = email.HtmlBody;
        var cidMap = email.InlineImages.ToDictionary(i => i.ContentId, StringComparer.OrdinalIgnoreCase);

        // Save inline images and rewrite cid: references
        if (email.InlineImages.Count > 0)
        {
            Directory.CreateDirectory(imagesDir);

            foreach (var img in email.InlineImages)
            {
                var imgPath = Path.Combine(imagesDir, img.FileName);
                File.WriteAllBytes(imgPath, img.Data);
            }

            html = CidRegex().Replace(html, match =>
            {
                var cid = match.Groups[1].Value;
                if (cidMap.TryGetValue(cid, out var img))
                {
                    return $"images/{img.FileName}";
                }
                return match.Value;
            });
        }

        // Save attachments
        if (email.Attachments.Count > 0)
        {
            Directory.CreateDirectory(attachmentsDir);

            foreach (var att in email.Attachments)
            {
                var attPath = Path.Combine(attachmentsDir, att.FileName);
                File.WriteAllBytes(attPath, att.Data);
            }
        }

        // Inject CSP to block script execution in the output HTML
        const string cspTag = "<meta http-equiv=\"Content-Security-Policy\" content=\"script-src 'none'\">";
        var headIndex = html.IndexOf("<head>", StringComparison.OrdinalIgnoreCase);
        if (headIndex >= 0)
        {
            html = html.Insert(headIndex + "<head>".Length, "\n" + cspTag);
        }
        else
        {
            headIndex = html.IndexOf("<head ", StringComparison.OrdinalIgnoreCase);
            if (headIndex >= 0)
            {
                var closeTag = html.IndexOf('>', headIndex);
                if (closeTag >= 0)
                {
                    html = html.Insert(closeTag + 1, "\n" + cspTag);
                }
                else
                {
                    html = cspTag + "\n" + html;
                }
            }
            else
            {
                html = cspTag + "\n" + html;
            }
        }

        // Write HTML
        var htmlPath = Path.Combine(outputDir, $"{baseName}.html");
        File.WriteAllText(htmlPath, html);

        Console.WriteLine($"Created: {outputDir}");
        Console.WriteLine($"  HTML:        {baseName}.html");
        if (email.InlineImages.Count > 0)
        {
            Console.WriteLine($"  Images:      {email.InlineImages.Count} file(s) in images/");
        }

        if (email.Attachments.Count > 0)
        {
            Console.WriteLine($"  Attachments: {email.Attachments.Count} file(s) in attachments/");
        }
    }

    [GeneratedRegex(@"cid:([^""'\s>]+)", RegexOptions.IgnoreCase)]
    private static partial Regex CidRegex();
}
