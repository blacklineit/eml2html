<#
.SYNOPSIS
    Installs eml2html shell extension for .eml files.
.DESCRIPTION
    Builds the eml2html project (if needed), publishes it, and registers
    the Windows shell context menu entries for .eml files.
    Does not require administrator privileges.
.PARAMETER InstallDir
    Directory to install the executable to. Defaults to %LOCALAPPDATA%\eml2html.
#>
param(
    [string]$InstallDir = "$env:LOCALAPPDATA\eml2html"
)

$ErrorActionPreference = 'Stop'

$projectDir = Join-Path $PSScriptRoot "..\src\Eml2Html"
$csproj = Join-Path $projectDir "Eml2Html.csproj"

if (-not (Test-Path $csproj)) {
    Write-Error "Project file not found: $csproj"
    exit 1
}

# Publish the project
Write-Host "Publishing eml2html..." -ForegroundColor Cyan
dotnet publish $csproj -c Release -o $InstallDir --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed."
    exit 1
}

$exePath = Join-Path $InstallDir "eml2html.exe"
if (-not (Test-Path $exePath)) {
    Write-Error "Published executable not found: $exePath"
    exit 1
}

Write-Host "Installed to: $exePath" -ForegroundColor Green

# Register shell extension under SystemFileAssociations (works regardless of default app)
Write-Host "Registering shell extension..." -ForegroundColor Cyan

$shellKey = "HKCU:\Software\Classes\SystemFileAssociations\.eml\shell\eml2html"

# Clean up any old registration under the wrong path
Remove-Item -Path "HKCU:\Software\Classes\.eml\shell\eml2html" -Recurse -Force -ErrorAction SilentlyContinue

# Create the flyout menu
New-Item -Path $shellKey -Force | Out-Null
Set-ItemProperty -Path $shellKey -Name "MUIVerb" -Value "eml2html"
Set-ItemProperty -Path $shellKey -Name "SubCommands" -Value ""
Set-ItemProperty -Path $shellKey -Name "Icon" -Value "$exePath"

# Extract to HTML
$htmlKey = "$shellKey\shell\html"
$htmlCmdKey = "$htmlKey\command"
New-Item -Path $htmlCmdKey -Force | Out-Null
Set-ItemProperty -Path $htmlKey -Name "(default)" -Value "Extract to HTML"
Set-ItemProperty -Path $htmlCmdKey -Name "(default)" -Value "`"$exePath`" --mode html `"%1`""

# Extract to Folder
$folderKey = "$shellKey\shell\folder"
$folderCmdKey = "$folderKey\command"
New-Item -Path $folderCmdKey -Force | Out-Null
Set-ItemProperty -Path $folderKey -Name "(default)" -Value "Extract to Folder"
Set-ItemProperty -Path $folderCmdKey -Name "(default)" -Value "`"$exePath`" --mode folder `"%1`""

Write-Host "Shell extension registered successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Right-click any .eml file to see the 'eml2html' context menu." -ForegroundColor Yellow
