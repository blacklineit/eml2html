# eml2html

[![Release](https://img.shields.io/github/v/release/blacklineit/eml2html?style=flat-square)](https://github.com/blacklineit/eml2html/releases/latest)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)

A Windows shell extension that adds right-click context menu options for `.eml` files (emails exported from Outlook).

Built by [Blackline IT](https://blacklineit.com).

## Features

- **Extract to HTML** — Creates a single self-contained `.html` file with all inline images embedded as base64 data URIs
- **Extract to Folder** — Creates a subfolder containing the HTML (with relative image links), extracted images, and file attachments

## Install

Download the latest installer from the [Releases page](https://github.com/blacklineit/eml2html/releases/latest) and run it. No prerequisites required — the installer includes everything needed.

The installer will:

1. Install `eml2html.exe` to `%LOCALAPPDATA%\eml2html\`
2. Register the right-click context menu for `.eml` files

To uninstall, use **Add or Remove Programs** in Windows Settings.

## Usage

### Via Context Menu (after install)

Right-click any `.eml` file and choose:

- **eml2html → Extract to HTML** — creates `<filename>.html` next to the `.eml` file
- **eml2html → Extract to Folder** — creates a `<filename>/` subfolder with:
  - `<filename>.html` — the email body with relative image links
  - `images/` — inline images from the email
  - `attachments/` — file attachments (PDFs, etc.)

> **Note:** On Windows 11, these options appear under **Show more options** in the context menu.

### Via Command Line

```sh
eml2html --mode html   <file.eml>
eml2html --mode folder <file.eml>
```

## Build from Source

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```sh
cd src\Eml2Html
dotnet build
dotnet run -- --mode html "path\to\email.eml"
```

### Developer Install (without Inno Setup)

PowerShell install/uninstall scripts are included for development use:

```powershell
.\install\install.ps1       # Build, publish, and register shell extension
.\install\uninstall.ps1     # Remove shell extension registry entries
```

## Project Structure

```txt
eml2html/
├── .github/workflows/
│   └── release.yml            # CI: build + installer + GitHub Release
├── src/Eml2Html/
│   ├── Eml2Html.csproj        # .NET 10 project with MimeKit
│   ├── Program.cs             # CLI entry point
│   ├── EmlParser.cs           # MIME parsing (body, images, attachments)
│   ├── HtmlExtractor.cs       # Self-contained HTML output
│   └── FolderExtractor.cs     # Folder extraction with assets
├── tests/
│   ├── TestSanitize.csproj    # Filename sanitization tests
│   └── TestSanitize.cs        # Path traversal test cases
├── install/
│   ├── eml2html.iss           # Inno Setup installer script
│   ├── install.ps1            # PowerShell installer (dev use)
│   ├── uninstall.ps1          # PowerShell uninstaller (dev use)
│   └── register.reg           # Manual registry file
├── LICENSE                    # MIT License
└── README.md
```

## License

[MIT](LICENSE) © [Blackline IT](https://blacklineit.com)
