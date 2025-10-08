# Script to find C# files that lack XML documentation
param(
    [string]$RootPath = "/mnt/e/Dynamic/IndTrace/IndTraceV2025/Src"
)

# Function to check if a file has XML documentation
function Test-HasXmlDocumentation {
    param([string]$FilePath)

    $content = Get-Content -Path $FilePath -Raw

    # Check for XML documentation patterns
    $hasClassDoc = $content -match '///\s*<summary>'
    $hasMethodDoc = $content -match '///\s*<summary>[\s\S]*?</summary>'

    return $hasClassDoc -or $hasMethodDoc
}

# Get all C# files
$csFiles = Get-ChildItem -Path $RootPath -Filter "*.cs" -Recurse |
    Where-Object { $_.Length -gt 0 } |
    Where-Object { $_.Name -notmatch "^(Global|Assembly|Temp|.*\.designer\.cs|.*\.g\.cs)" }

Write-Host "Found $($csFiles.Count) C# files to analyze..." -ForegroundColor Green

$undocumentedFiles = @()

foreach ($file in $csFiles) {
    if (-not (Test-HasXmlDocumentation -FilePath $file.FullName)) {
        $undocumentedFiles += $file.FullName
        Write-Host "UNDOCUMENTED: $($file.FullName)" -ForegroundColor Yellow
    }
    else {
        Write-Host "DOCUMENTED: $($file.FullName)" -ForegroundColor Green
    }
}

Write-Host "`nSummary:" -ForegroundColor Cyan
Write-Host "Total files analyzed: $($csFiles.Count)" -ForegroundColor White
Write-Host "Documented files: $($csFiles.Count - $undocumentedFiles.Count)" -ForegroundColor Green
Write-Host "Undocumented files: $($undocumentedFiles.Count)" -ForegroundColor Yellow

# Save undocumented files list
$undocumentedFiles | Out-File -FilePath "$RootPath/UndocumentedFiles.txt" -Encoding UTF8
Write-Host "`nUndocumented files list saved to: $RootPath/UndocumentedFiles.txt" -ForegroundColor Cyan
