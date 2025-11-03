# Fix Duplicate Paths Script
# This script fixes paths that have duplicate segments like ..\..\code\X\..\..\code\X

Write-Host "Fixing duplicate paths..." -ForegroundColor Green

$basePath = $PSScriptRoot

# Get all .csproj files
$csprojFiles = Get-ChildItem -Path $basePath -Recurse -Filter "*.csproj" -File | Where-Object { 
    $_.FullName -notmatch "Test Project|examples|docs|bin|obj|\.vs" 
}

$updatedCount = 0
foreach ($file in $csprojFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $fileUpdated = $false
    
    # Fix patterns like: ..\..\code\X\..\..\code\X -> ..\..\code\X
    $pattern1 = '\.\.\\\.\.\\code\\([^\\]+)\\\.\.\\\.\.\\code\\\1'
    if ($content -match $pattern1) {
        $content = $content -replace $pattern1, '..\..\code\$1'
        $fileUpdated = $true
    }
    
    # Fix patterns like: ..\code\X\..\code\X -> ..\code\X
    $pattern2 = '\.\.\\code\\([^\\]+)\\\.\.\\code\\\1'
    if ($content -match $pattern2) {
        $content = $content -replace $pattern2, '..\code\$1'
        $fileUpdated = $true
    }
    
    # Fix patterns like: X\X\X.csproj -> X\X.csproj (same folder)
    $pattern3 = '([^"\\]+)\\\1\\(\1\.csproj)'
    if ($content -match $pattern3) {
        $content = $content -replace $pattern3, '$1\$2'
        $fileUpdated = $true
    }
    
    # Fix patterns like: ..\X\X\X.csproj -> X\X.csproj (sibling folder)
    $pattern4 = '\.\.\\([^"\\]+)\\\1\\(\1\.csproj)'
    if ($content -match $pattern4) {
        $content = $content -replace $pattern4, '$1\$2'
        $fileUpdated = $true
    }
    
    if ($fileUpdated) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Fixed: $($file.Name)" -ForegroundColor Cyan
        $updatedCount++
    }
}

Write-Host "`nFixed $updatedCount files." -ForegroundColor Green

