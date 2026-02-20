using System.Text.RegularExpressions;

namespace Eml2Html;

public static partial class HtmlExtractor
{
    /// <summary>
    /// Creates a self-contained HTML file with all inline images embedded as base64 data URIs.
    /// </summary>
    public static void Extract(string emlPath, ParsedEmail email)
    {
        var outputPath = Path.ChangeExtension(emlPath, ".html");
        var html = email.HtmlBody;

        // Build a lookup of Content-ID → inline image
        var cidMap = email.InlineImages.ToDictionary(i => i.ContentId, StringComparer.OrdinalIgnoreCase);

        // Replace cid: references with data URIs
        html = CidRegex().Replace(html, match =>
        {
            var cid = match.Groups[1].Value;
            if (!cidMap.TryGetValue(cid, out var img))
            {
                return match.Value;
            }
            var base64 = Convert.ToBase64String(img.Data);
            return $"data:{img.MimeType};base64,{base64}";
        });

        // Inject CSP to block script execution in the output HTML
        html = InjectContentSecurityPolicy(html);

        File.WriteAllText(outputPath, html);
        Console.WriteLine($"Created: {outputPath}");
    }

    private static string InjectContentSecurityPolicy(string html)
    {
        const string cspTag = "<meta http-equiv=\"Content-Security-Policy\" content=\"script-src 'none'\">";

        // Insert after <head> if present
        var headIndex = html.IndexOf("<head>", StringComparison.OrdinalIgnoreCase);
        if (headIndex >= 0)
        {
            return html.Insert(headIndex + "<head>".Length, "\n" + cspTag);
        }

        headIndex = html.IndexOf("<head ", StringComparison.OrdinalIgnoreCase);
        if (headIndex < 0)
        {
            return cspTag + "\n" + html;
        }
        var closeTag = html.IndexOf('>', headIndex);
        if (closeTag >= 0)
        {
            return html.Insert(closeTag + 1, "\n" + cspTag);
        }

        // Fallback: prepend if no <head> found
        return cspTag + "\n" + html;
    }

    [GeneratedRegex(@"cid:([^""'\s>]+)", RegexOptions.IgnoreCase)]
    private static partial Regex CidRegex();
}
