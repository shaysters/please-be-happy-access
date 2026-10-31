# Builds the release zip into dist\PleaseBeHappyAccess.zip.
# Usage (from the repo root):
#   powershell -ExecutionPolicy Bypass -File scripts\package-release.ps1
# The zip mirrors the game folder, so extracting it into the game folder installs the mod
# (BepInEx itself is installed separately, by the installer or by hand):
#   BepInEx/plugins/PleaseBeHappyAccess/PleaseBeHappyAccess.dll, prism.dll,
#   descriptions/*.tsv, prism_licenses/**
#   PleaseBeHappyAccess-README.md, PleaseBeHappyAccess-CHANGELOG.md

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

# SkipDeploy: a release build shouldn't touch the local game install.
# DevTools=false: never ship the private test driver, even on a machine that has it.
dotnet build src\PleaseBeHappyAccess -c Release -p:SkipDeploy=true -p:DevTools=false
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

$plugin = "BepInEx/plugins/PleaseBeHappyAccess"
# Source path (relative to the repo root) -> path inside the zip.
$files = [ordered]@{
    "src\PleaseBeHappyAccess\bin\Release\PleaseBeHappyAccess.dll" = "$plugin/PleaseBeHappyAccess.dll"
    "lib\prism.dll"                                              = "$plugin/prism.dll"
    "README.md"                                                  = "PleaseBeHappyAccess-README.md"
    "CHANGELOG.md"                                               = "PleaseBeHappyAccess-CHANGELOG.md"
}
$folders = [ordered]@{
    "descriptions"        = "$plugin/descriptions"
    "lib\prism_licenses"  = "$plugin/prism_licenses"
}

$dist = Join-Path $root "dist"
New-Item -ItemType Directory -Path $dist -Force | Out-Null
$zip = Join-Path $dist "PleaseBeHappyAccess.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }

# Write entries with forward slashes (Windows PowerShell's ZipFile writes backslashes).
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::Open($zip, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($source in $files.Keys) {
        $full = Join-Path $root $source
        if (-not (Test-Path $full)) { throw "Missing release file: $full" }
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $full, $files[$source]) | Out-Null
    }
    foreach ($source in $folders.Keys) {
        $full = Join-Path $root $source
        if (-not (Test-Path $full)) { throw "Missing release folder: $full" }
        foreach ($item in Get-ChildItem $full -Recurse -File) {
            $name = $folders[$source] + "/" + $item.FullName.Substring($full.Length + 1).Replace([char]92, [char]47)
            [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $item.FullName, $name) | Out-Null
        }
    }
} finally {
    $archive.Dispose()
}
Write-Host "Built: $zip"
