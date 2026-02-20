<#
.SYNOPSIS
    Uninstalls eml2html shell extension.
.DESCRIPTION
    Removes the Windows shell context menu entries and optionally deletes the installed executable.
    Does not require administrator privileges.
.PARAMETER InstallDir
    Directory where eml2html was installed. Defaults to %LOCALAPPDATA%\eml2html.
.PARAMETER RemoveFiles
    If specified, also deletes the installed executable and directory.
#>
param(
    [string]$InstallDir = "$env:LOCALAPPDATA\eml2html",
    [switch]$RemoveFiles
)

$ErrorActionPreference = 'Stop'

# Remove shell extension registry keys
$shellKey = "HKCU:\Software\Classes\SystemFileAssociations\.eml\shell\eml2html"
$oldShellKey = "HKCU:\Software\Classes\.eml\shell\eml2html"

# Remove current registration
if (Test-Path $shellKey) {
    Remove-Item -Path $shellKey -Recurse -Force
    Write-Host "Shell extension removed." -ForegroundColor Green
} else {
    Write-Host "Shell extension registry keys not found (already removed?)." -ForegroundColor Yellow
}

# Also clean up legacy path if present
if (Test-Path $oldShellKey) {
    Remove-Item -Path $oldShellKey -Recurse -Force
    Write-Host "Legacy shell extension keys removed." -ForegroundColor Green
}

# Optionally remove installed files
if ($RemoveFiles) {
    if (Test-Path $InstallDir) {
        Remove-Item -Path $InstallDir -Recurse -Force
        Write-Host "Install directory removed: $InstallDir" -ForegroundColor Green
    } else {
        Write-Host "Install directory not found: $InstallDir" -ForegroundColor Yellow
    }
}

Write-Host "Uninstall complete." -ForegroundColor Green
