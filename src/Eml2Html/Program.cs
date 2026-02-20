using Eml2Html;

if (args.Length < 2 || args[0] is not ("--mode" or "-m"))
{
    PrintUsage();
    return 1;
}

var mode = args[1].ToLowerInvariant();
if (mode is not ("html" or "folder"))
{
    Console.Error.WriteLine($"Error: Unknown mode '{args[1]}'. Use 'html' or 'folder'.");
    return 1;
}

if (args.Length < 3)
{
    Console.Error.WriteLine("Error: No input file specified.");
    return 1;
}

var emlPath = args[2];
if (!File.Exists(emlPath))
{
    Console.Error.WriteLine($"Error: File not found: {emlPath}");
    return 1;
}

try
{
    var email = EmlParser.Parse(emlPath);

    if (string.IsNullOrWhiteSpace(email.HtmlBody))
    {
        Console.Error.WriteLine("Warning: No HTML body found in the email.");
        return 2;
    }

    switch (mode)
    {
        case "html":
            HtmlExtractor.Extract(emlPath, email);
            break;
        case "folder":
            FolderExtractor.Extract(emlPath, email);
            break;
    }

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

static void PrintUsage()
{
    Console.WriteLine("""
        eml2html - Convert .eml files to HTML

        Usage:
          eml2html --mode html   <file.eml>   Extract to a single self-contained HTML file
          eml2html --mode folder <file.eml>   Extract to a folder with HTML, images, and attachments

        Options:
          --mode, -m    Extraction mode: 'html' or 'folder'
        """);
}
