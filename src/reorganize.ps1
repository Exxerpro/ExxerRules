# Solution Reorganization Script
# This script reorganizes the solution structure to match Visual Studio logical folders

Write-Host "Starting solution reorganization..." -ForegroundColor Green

# Create folder structure
Write-Host "Creating folder structure..." -ForegroundColor Yellow

$basePath = Join-Path $PSScriptRoot "."
$codePath = Join-Path $basePath "code"
$testPath = Join-Path $basePath "test"

# Code folders
New-Item -ItemType Directory -Force -Path (Join-Path $codePath "Analyzer") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $codePath "Cli") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $codePath "Mcp") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $codePath "SemanticRag") | Out-Null

# Test folders
New-Item -ItemType Directory -Force -Path (Join-Path $testPath "AnalyzerTests") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $testPath "CliTests") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $testPath "McpTests") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $testPath "SemanticRagTests") | Out-Null

Write-Host "Folder structure created." -ForegroundColor Green

# Move code projects
Write-Host "Moving code projects..." -ForegroundColor Yellow

# Analyzer folder
Move-Item -Path (Join-Path $codePath "IndFusion.Analyzer") -Destination (Join-Path $codePath "Analyzer\IndFusion.Analyzer") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $codePath "IndFusion.Fixer") -Destination (Join-Path $codePath "Analyzer\IndFusion.Fixer") -Force -ErrorAction SilentlyContinue

# Cli folder
Move-Item -Path (Join-Path $codePath "IndFusion.Tools.Cli") -Destination (Join-Path $codePath "Cli\IndFusion.Tools.Cli") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $codePath "IndFusion.Tools.Cli.Core") -Destination (Join-Path $codePath "Cli\IndFusion.Tools.Cli.Core") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $codePath "IndFusion.Tools.Cli.Console") -Destination (Join-Path $codePath "Cli\IndFusion.Tools.Cli.Console") -Force -ErrorAction SilentlyContinue

# Mcp folder
Move-Item -Path (Join-Path $codePath "IndFusion.Mcp.Core") -Destination (Join-Path $codePath "Mcp\IndFusion.Mcp.Core") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $codePath "IndFusion.Mcp.Server") -Destination (Join-Path $codePath "Mcp\IndFusion.Mcp.Server") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $codePath "IndFusion.Mcp.Web") -Destination (Join-Path $codePath "Mcp\IndFusion.Mcp.Web") -Force -ErrorAction SilentlyContinue

# SemanticRag folder
Move-Item -Path (Join-Path $codePath "IndFusion.SemanticRag.Domain") -Destination (Join-Path $codePath "SemanticRag\IndFusion.SemanticRag.Domain") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $codePath "IndFusion.SemanticRag.Application") -Destination (Join-Path $codePath "SemanticRag\IndFusion.SemanticRag.Application") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $codePath "IndFusion.SemanticRag.Infrastructure") -Destination (Join-Path $codePath "SemanticRag\IndFusion.SemanticRag.Infrastructure") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $codePath "IndFusion.SemanticRag.WebAPI") -Destination (Join-Path $codePath "SemanticRag\IndFusion.SemanticRag.WebAPI") -Force -ErrorAction SilentlyContinue

Write-Host "Code projects moved." -ForegroundColor Green

# Move test projects
Write-Host "Moving test projects..." -ForegroundColor Yellow

# AnalyzerTests folder
Move-Item -Path (Join-Path $testPath "IndFusion.Analyzer.Tests") -Destination (Join-Path $testPath "AnalyzerTests\IndFusion.Analyzer.Tests") -Force -ErrorAction SilentlyContinue

# CliTests folder
Move-Item -Path (Join-Path $testPath "IndFusion.Tools.Cli.Tests") -Destination (Join-Path $testPath "CliTests\IndFusion.Tools.Cli.Tests") -Force -ErrorAction SilentlyContinue

# McpTests folder
Move-Item -Path (Join-Path $testPath "IndFusion.Mcp.Core.Tests") -Destination (Join-Path $testPath "McpTests\IndFusion.Mcp.Core.Tests") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $testPath "IndFusion.Mcp.Server.Tests") -Destination (Join-Path $testPath "McpTests\IndFusion.Mcp.Server.Tests") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $testPath "IndFusion.Mcp.Tests") -Destination (Join-Path $testPath "McpTests\IndFusion.Mcp.Tests") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $testPath "IndFusion.Mcp.Tests.Integration") -Destination (Join-Path $testPath "McpTests\IndFusion.Mcp.Tests.Integration") -Force -ErrorAction SilentlyContinue
Move-Item -Path (Join-Path $testPath "IndFusion.Mcp.Web.Tests") -Destination (Join-Path $testPath "McpTests\IndFusion.Mcp.Web.Tests") -Force -ErrorAction SilentlyContinue

# SemanticRagTests folder
Move-Item -Path (Join-Path $testPath "IndFusion.SemanticRag.Tests") -Destination (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests") -Force -ErrorAction SilentlyContinue

# Projects that need to be moved from src root
$srcPath = $basePath
if (Test-Path (Join-Path $srcPath "IndFusion.SemanticRag.Tests.Unit")) {
    Move-Item -Path (Join-Path $srcPath "IndFusion.SemanticRag.Tests.Unit") -Destination (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests.Unit") -Force -ErrorAction SilentlyContinue
}
if (Test-Path (Join-Path $srcPath "IndFusion.SemanticRag.Tests.Integration")) {
    Move-Item -Path (Join-Path $srcPath "IndFusion.SemanticRag.Tests.Integration") -Destination (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests.Integration") -Force -ErrorAction SilentlyContinue
}
if (Test-Path (Join-Path $srcPath "IndFusion.SemanticRag.Tests.Architecture")) {
    Move-Item -Path (Join-Path $srcPath "IndFusion.SemanticRag.Tests.Architecture") -Destination (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests.Architecture") -Force -ErrorAction SilentlyContinue
}
if (Test-Path (Join-Path $srcPath "IndFusion.SemanticRag.Tests.System")) {
    Move-Item -Path (Join-Path $srcPath "IndFusion.SemanticRag.Tests.System") -Destination (Join-Path $testPath "SemanticRagTests\IndFusion.SemanticRag.Tests.System") -Force -ErrorAction SilentlyContinue
}

Write-Host "Test projects moved." -ForegroundColor Green
Write-Host "Reorganization complete! Now update .csproj files and solution file." -ForegroundColor Green

