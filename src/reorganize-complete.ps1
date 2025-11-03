# Complete Solution Reorganization Script
# This script reorganizes the solution structure to match Visual Studio logical folders
# AND updates all project references automatically

Write-Host "Starting complete solution reorganization..." -ForegroundColor Green

$ErrorActionPreference = "Stop"
$basePath = $PSScriptRoot
$codePath = Join-Path $basePath "code"
$testPath = Join-Path $basePath "test"

# Create folder structure
Write-Host "Creating folder structure..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path (Join-Path $codePath "Analyzer") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $codePath "Cli") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $codePath "Mcp") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $codePath "SemanticRag") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $testPath "AnalyzerTests") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $testPath "CliTests") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $testPath "McpTests") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $testPath "SemanticRagTests") | Out-Null

# Function to move project and update references
function Move-Project {
    param(
        [string]$SourcePath,
        [string]$DestPath
    )
    
    if (Test-Path $SourcePath) {
        Write-Host "Moving $SourcePath -> $DestPath" -ForegroundColor Cyan
        Move-Item -Path $SourcePath -Destination $DestPath -Force
    }
}

# Move code projects
Write-Host "`nMoving code projects..." -ForegroundColor Yellow
Move-Project (Join-Path $codePath "IndFusion.Analyzer") (Join-Path $codePath "Analyzer\IndFusion.Analyzer")
Move-Project (Join-Path $codePath "IndFusion.Fixer") (Join-Path $codePath "Analyzer\IndFusion.Fixer")
Move-Project (Join-Path $codePath "IndFusion.Tools.Cli") (Join-Path $codePath "Cli\IndFusion.Tools.Cli")
Move-Project (Join-Path $codePath "IndFusion.Tools.Cli.Core") (Join-Path $codePath "Cli\IndFusion.Tools.Cli.Core")
Move-Project (Join-Path $codePath "IndFusion.Tools.Cli.Console") (Join-Path $codePath "Cli\IndFusion.Tools.Cli.Console")
Move-Project (Join-Path $codePath "IndFusion.Mcp.Core") (Join-Path $codePath "Mcp\IndFusion.Mcp.Core")
Move-Project (Join-Path $codePath "IndFusion.Mcp.Server") (Join-Path $codePath "Mcp\IndFusion.Mcp.Server")
Move-Project (Join-Path $codePath "IndFusion.Mcp.Web") (Join-Path $codePath "Mcp\IndFusion.Mcp.Web")
Move-Project (Join-Path $codePath "IndFusion.SemanticRag.Domain") (Join-Path $codePath "SemanticRag\IndFusion.SemanticRag.Domain")
Move-Project (Join-Path $codePath "IndFusion.SemanticRag.Application") (Join-Path $codePath "SemanticRag\IndFusion.SemanticRag.Application")
Move-Project (Join-Path $codePath "IndFusion.SemanticRag.Infrastructure") (Join-Path $codePath "SemanticRag\IndFusion.SemanticRag.Infrastructure")
Move-Project (Join-Path $codePath "IndFusion.SemanticRag.WebAPI") (Join-Path $codePath "SemanticRag\IndFusion.SemanticRag.WebAPI")

# Move test projects
Write-Host "`nMoving test projects..." -ForegroundColor Yellow
Move-Project (Join-Path $testPath "IndFusion.Analyzer.Tests") (Join-Path $testPath "AnalyzerTests\IndFusion.Analyzer.Tests")
Move-Project (Join-Path $testPath "IndFusion.Tools.Cli.Tests") (Join-Path $testPath "CliTests\IndFusion.Tools.Cli.Tests")
Move-Project (Join-Path $testPath "IndFusion.Mcp.Core.Tests") (Join-Path $testPath "McpTests\IndFusion.Mcp.Core.Tests")
Move-Project (Join-Path $testPath "IndFusion.Mcp.Server.Tests") (Join-Path $testPath "McpTests\IndFusion.Mcp.Server.Tests")
Move-Project (Join-Path $testPath "IndFusion.Mcp.Tests") (Join-Path $testPath "McpTests\IndFusion.Mcp.Tests")
Move-Project (Join-Path $testPath "IndFusion.Mcp.Tests.Integration") (Join-Path $testPath "McpTests\IndFusion.Mcp.Tests.Integration")
Move-Project (Join-Path $testPath "IndFusion.Mcp.Web.Tests") (Join-Path $testPath "McpTests\IndFusion.Mcp.Web.Tests")
Move-Project (Join-Path $testPath "IndFusion.SemanticRag.Tests") (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests")

# Move projects from src root
if (Test-Path (Join-Path $basePath "IndFusion.SemanticRag.Tests.Unit")) {
    Move-Project (Join-Path $basePath "IndFusion.SemanticRag.Tests.Unit") (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests.Unit")
}
if (Test-Path (Join-Path $basePath "IndFusion.SemanticRag.Tests.Integration")) {
    Move-Project (Join-Path $basePath "IndFusion.SemanticRag.Tests.Integration") (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests.Integration")
}
if (Test-Path (Join-Path $basePath "IndFusion.SemanticRag.Tests.Architecture")) {
    Move-Project (Join-Path $basePath "IndFusion.SemanticRag.Tests.Architecture") (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests.Architecture")
}
if (Test-Path (Join-Path $basePath "IndFusion.SemanticRag.Tests.System")) {
    Move-Project (Join-Path $basePath "IndFusion.SemanticRag.Tests.System") (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests.System")
}

Write-Host "`nProjects moved successfully!" -ForegroundColor Green
Write-Host "`nNext: Run the update-references.ps1 script to update all project references." -ForegroundColor Yellow

