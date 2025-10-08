# Fix xUnit references - Remove xunit.runner.visualstudio from all project files
# This script removes xUnit v2 runner references since we're using Microsoft Testing Platform

Write-Host "Fixing xUnit references in project files..." -ForegroundColor Green

# Find all .csproj files
$projectFiles = Get-ChildItem -Path . -Filter "*.csproj" -Recurse

foreach ($projectFile in $projectFiles) {
    Write-Host "Processing: $($projectFile.FullName)" -ForegroundColor Yellow

    $content = Get-Content $projectFile.FullName -Raw
    $originalContent = $content

    # Remove xunit.runner.visualstudio references
    $content = $content -replace '\s*<PackageReference Include="xunit\.runner\.visualstudio"[^>]*>\s*', ''

    # Remove empty ItemGroup tags that might be left behind
    $content = $content -replace '\s*<ItemGroup>\s*</ItemGroup>\s*', ''

    # If content changed, write it back
    if ($content -ne $originalContent) {
        Set-Content -Path $projectFile.FullName -Value $content -NoNewline
        Write-Host "  ✓ Updated" -ForegroundColor Green
    } else {
        Write-Host "  - No changes needed" -ForegroundColor Gray
    }
}

Write-Host "Done fixing xUnit references!" -ForegroundColor Green
