# Update Project References Script
# This script updates all ProjectReference paths in .csproj files after reorganization
# It calculates relative paths dynamically based on the actual location of each file

Write-Host "Updating project references..." -ForegroundColor Green

$basePath = $PSScriptRoot
$codePath = Join-Path $basePath "code"
$testPath = Join-Path $basePath "test"

# Map of project names to their new locations
$projectLocations = @{
    "IndFusion.Analyzer.csproj" = "code\Analyzer\IndFusion.Analyzer"
    "IndFusion.Fixer.csproj" = "code\Analyzer\IndFusion.Fixer"
    "IndFusion.Tools.Cli.csproj" = "code\Cli\IndFusion.Tools.Cli"
    "IndFusion.Tools.Cli.Core.csproj" = "code\Cli\IndFusion.Tools.Cli.Core"
    "IndFusion.Tools.Cli.Console.csproj" = "code\Cli\IndFusion.Tools.Cli.Console"
    "IndFusion.Mcp.Core.csproj" = "code\Mcp\IndFusion.Mcp.Core"
    "IndFusion.Mcp.Server.csproj" = "code\Mcp\IndFusion.Mcp.Server"
    "IndFusion.Mcp.Web.csproj" = "code\Mcp\IndFusion.Mcp.Web"
    "IndFusion.SemanticRag.Domain.csproj" = "code\SemanticRag\IndFusion.SemanticRag.Domain"
    "IndFusion.SemanticRag.Application.csproj" = "code\SemanticRag\IndFusion.SemanticRag.Application"
    "IndFusion.SemanticRag.Infrastructure.csproj" = "code\SemanticRag\IndFusion.SemanticRag.Infrastructure"
    "IndFusion.SemanticRag.WebAPI.csproj" = "code\SemanticRag\IndFusion.SemanticRag.WebAPI"
    "IndFusion.Analyzer.Tests.csproj" = "test\AnalyzerTests\IndFusion.Analyzer.Tests"
    "IndFusion.Tools.Cli.Tests.csproj" = "test\CliTests\IndFusion.Tools.Cli.Tests"
    "IndFusion.Mcp.Core.Tests.csproj" = "test\McpTests\IndFusion.Mcp.Core.Tests"
    "IndFusion.Mcp.Server.Tests.csproj" = "test\McpTests\IndFusion.Mcp.Server.Tests"
    "IndFusion.Mcp.Tests.csproj" = "test\McpTests\IndFusion.Mcp.Tests"
    "IndFusion.Mcp.Tests.Integration.csproj" = "test\McpTests\IndFusion.Mcp.Tests.Integration"
    "IndFusion.Mcp.Web.Tests.csproj" = "test\McpTests\IndFusion.Mcp.Web.Tests"
    "IndFusion.SemanticRag.Tests.csproj" = "test\SemanticRagTests\IndFusion.SemanticRag.Tests"
    "IndFusion.SemanticRag.Tests.Unit.csproj" = "test\SemanticRagTests\IndFusion.SemanticRag.Tests.Unit"
    "IndFusion.SemanticRag.Tests.Integration.csproj" = "test\SemanticRagTests\IndFusion.SemanticRag.Tests.Integration"
    "IndFusion.SemanticRag.Tests.Architecture.csproj" = "test\SemanticRagTests\IndFusion.SemanticRag.Tests.Architecture"
    "IndFusion.SemanticRag.Tests.System.csproj" = "test\SemanticRagTests\IndFusion.SemanticRag.Tests.System"
}

# Function to calculate relative path
function Get-RelativePath {
    param(
        [string]$FromPath,
        [string]$ToPath
    )
    
    $fromUri = New-Object System.Uri((Resolve-Path $FromPath).Path)
    $toUri = New-Object System.Uri((Resolve-Path $ToPath).Path)
    $relativeUri = $fromUri.MakeRelativeUri($toUri)
    $relativePath = $relativeUri.ToString() -replace '/', '\'
    return $relativePath
}

# Get all .csproj files
$csprojFiles = Get-ChildItem -Path $basePath -Recurse -Filter "*.csproj" -File | Where-Object { 
    $_.FullName -notmatch "Test Project|examples|docs|bin|obj|\.vs" 
}

$updatedCount = 0
foreach ($file in $csprojFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $fileUpdated = $false
    
    # Find all ProjectReference elements
    $projectRefPattern = 'Include="([^"]+IndFusion\.(?:Analyzer|Fixer|Mcp|SemanticRag|Tools)\.[^"]+\.csproj)"'
    $matches = [regex]::Matches($content, $projectRefPattern)
    
    foreach ($match in $matches) {
        $oldRefPath = $match.Groups[1].Value
        $projectName = Split-Path $oldRefPath -Leaf
        
        if ($projectLocations.ContainsKey($projectName)) {
            $newProjectPath = Join-Path $basePath $projectLocations[$projectName]
            $newProjectFile = Join-Path $newProjectPath $projectName
            
            if (Test-Path $newProjectFile) {
                # Calculate relative path from current file to target file
                $relativePath = Get-RelativePath -FromPath $file.DirectoryName -ToPath $newProjectFile
                
                # Update the reference
                $newRefPath = $relativePath
                $content = $content -replace [regex]::Escape($oldRefPath), $newRefPath
                $fileUpdated = $true
                Write-Host "  Updated reference in $($file.Name): $oldRefPath -> $newRefPath" -ForegroundColor Cyan
            }
        }
    }
    
    # Also update any None Include paths that reference projects
    $nonePattern = 'Include="([^"]*IndFusion\.(?:Analyzer|Fixer|Mcp|SemanticRag|Tools)[^"]*\.csproj)"'
    $noneMatches = [regex]::Matches($content, $nonePattern)
    
    foreach ($match in $noneMatches) {
        $oldPath = $match.Groups[1].Value
        if ($oldPath -match 'IndFusion\.(Analyzer|Fixer|Mcp|SemanticRag|Tools)\.[^"]+\.csproj') {
            $projectName = Split-Path $oldPath -Leaf
            if ($projectLocations.ContainsKey($projectName)) {
                $newProjectPath = Join-Path $basePath $projectLocations[$projectName]
                $newProjectFile = Join-Path $newProjectPath $projectName
                
                if (Test-Path $newProjectFile) {
                    $relativePath = Get-RelativePath -FromPath $file.DirectoryName -ToPath $newProjectFile
                    $newPath = $oldPath -replace [regex]::Escape((Split-Path $oldPath -Leaf)), $relativePath
                    $content = $content -replace [regex]::Escape($oldPath), $newPath
                    $fileUpdated = $true
                    Write-Host "  Updated None Include in $($file.Name): $oldPath -> $newPath" -ForegroundColor Cyan
                }
            }
        }
    }
    
    if ($fileUpdated) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Updated: $($file.Name)" -ForegroundColor Green
        $updatedCount++
    }
}

# Also update solution file
$slnFile = Join-Path $basePath "IndFusion.sln"
if (Test-Path $slnFile) {
    Write-Host "`nSolution file already updated." -ForegroundColor Yellow
}

Write-Host "`nUpdated $updatedCount .csproj files." -ForegroundColor Green
Write-Host "`nPlease verify the solution loads correctly in Visual Studio." -ForegroundColor Yellow
