# Fix Case-Sensitive Paths Script
# This script fixes project references to use correct case for folder names

Write-Host "Fixing case-sensitive project reference paths..." -ForegroundColor Green

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
    
    # Fix MCP references: ..\Mcp\ -> ..\MCP\
    if ($content -match '\\Mcp\\') {
        $content = $content -replace '\\Mcp\\', '\MCP\'
        $fileUpdated = $true
    }
    
    # Fix CLI references: ..\Cli\ -> ..\CLI\
    if ($content -match '\\Cli\\') {
        $content = $content -replace '\\Cli\\', '\CLI\'
        $fileUpdated = $true
    }
    
    if ($fileUpdated) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Fixed: $($file.Name)" -ForegroundColor Cyan
        $updatedCount++
    }
}

Write-Host "`nFixed $updatedCount files." -ForegroundColor Green

