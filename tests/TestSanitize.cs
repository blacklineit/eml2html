// Verifies that SanitizeFileName blocks path traversal attacks.
// Run via: dotnet run --project tests/TestSanitize.csproj

var failures = 0;

string[] tests =
[
    @"..\..\..\Windows\System32\evil.exe",
    @"C:\Windows\System32\evil.exe",
    @"..\..\evil.txt",
    @"folder\..\..\..\evil.txt",
    "/etc/passwd",
    "../../etc/passwd",
    "normal.txt",
    @"..\parent\file.txt",
    "",
    "   "
];

foreach (var test in tests)
{
    var sanitized = SanitizeFileName(test);
    var safe = !sanitized.Contains('\\') && !sanitized.Contains('/') && !sanitized.Contains("..");

    Console.Write(safe ? "PASS" : "FAIL");
    Console.WriteLine($"  \"{test}\" -> \"{sanitized}\"");

    if (!safe) failures++;
}

Console.WriteLine();
Console.WriteLine(failures == 0 ? "All tests passed." : $"{failures} test(s) FAILED.");
return failures;

// Mirror of EmlParser.SanitizeFileName — kept in sync for standalone testing
static string SanitizeFileName(string name)
{
    name = Path.GetFileName(name);

    foreach (var c in Path.GetInvalidFileNameChars())
    {
        name = name.Replace(c, '_');
    }

    name = name.Replace("..", "_");

    if (string.IsNullOrWhiteSpace(name))
        name = "unnamed";

    return name;
}
