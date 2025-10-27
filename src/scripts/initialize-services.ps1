# Initialize IndFusion Semantic RAG Platform Services
# This script sets up the development environment and populates sample data

param(
    [string]$Environment = "Development",
    [switch]$SkipDocker = $false,
    [switch]$PopulateSampleData = $true
)

Write-Host "🚀 Initializing IndFusion Semantic RAG Platform..." -ForegroundColor Green

# Check if Docker is running
if (-not $SkipDocker) {
    Write-Host "📦 Checking Docker services..." -ForegroundColor Yellow
    
    try {
        docker-compose --version | Out-Null
        Write-Host "✅ Docker Compose is available" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Docker Compose not found. Please install Docker Desktop." -ForegroundColor Red
        exit 1
    }
    
    # Start infrastructure services
    Write-Host "🐳 Starting infrastructure services..." -ForegroundColor Yellow
    docker-compose up -d qdrant ollama neo4j redis
    
    # Wait for services to be ready
    Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    # Check service health
    Write-Host "🔍 Checking service health..." -ForegroundColor Yellow
    
    # Check Qdrant
    try {
        $qdrantResponse = Invoke-RestMethod -Uri "http://localhost:6333/health" -Method Get
        Write-Host "✅ Qdrant is healthy" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Qdrant is not responding" -ForegroundColor Red
    }
    
    # Check Ollama
    try {
        $ollamaResponse = Invoke-RestMethod -Uri "http://localhost:11434/api/tags" -Method Get
        Write-Host "✅ Ollama is healthy" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Ollama is not responding" -ForegroundColor Red
    }
    
    # Check Neo4j
    try {
        $neo4jResponse = Invoke-RestMethod -Uri "http://localhost:7474" -Method Get
        Write-Host "✅ Neo4j is healthy" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Neo4j is not responding" -ForegroundColor Red
    }
}

# Build and run the application
Write-Host "🔨 Building IndFusion Semantic RAG Platform..." -ForegroundColor Yellow

try {
    # Restore packages
    Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
    dotnet restore IndFusion.sln
    
    # Build the solution
    Write-Host "🔨 Building solution..." -ForegroundColor Yellow
    dotnet build IndFusion.sln -c Release --no-restore
    
    Write-Host "✅ Build completed successfully" -ForegroundColor Green
}
catch {
    Write-Host "❌ Build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Populate sample data if requested
if ($PopulateSampleData) {
    Write-Host "📚 Populating sample data..." -ForegroundColor Yellow
    
    # Create a simple script to populate sample patterns
    $sampleDataScript = @"
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) => {
        services.AddSemanticRagServices(context.Configuration);
    })
    .Build();

var vectorSearchService = host.Services.GetRequiredService<IVectorSearchService>();

// Sample pattern documents
var samplePatterns = new[]
{
    new { Id = "pattern_001", Content = "Use Result<T> pattern for error handling instead of exceptions", Type = "pattern", Category = "functional_programming" },
    new { Id = "pattern_002", Content = "Implement async methods with CancellationToken parameter", Type = "pattern", Category = "async_programming" },
    new { Id = "pattern_003", Content = "Use dependency injection for service dependencies", Type = "pattern", Category = "architecture" },
    new { Id = "pattern_004", Content = "Avoid magic numbers and strings in code", Type = "pattern", Category = "code_quality" },
    new { Id = "pattern_005", Content = "Validate null parameters at method entry", Type = "pattern", Category = "null_safety" }
};

foreach (var pattern in samplePatterns)
{
    var metadata = new Dictionary<string, object>
    {
        ["type"] = pattern.Type,
        ["category"] = pattern.Category,
        ["source"] = "sample_data"
    };
    
    await vectorSearchService.StoreDocumentAsync(pattern.Id, pattern.Content, metadata);
    Console.WriteLine($"Stored pattern: {pattern.Id}");
}

Console.WriteLine("Sample data populated successfully!");
"@

    $sampleDataScript | Out-File -FilePath "populate-sample-data.cs" -Encoding UTF8
    
    try {
        Write-Host "📝 Running sample data population..." -ForegroundColor Yellow
        dotnet run --project code/IndFusion.SemanticRag.WebAPI -- --populate-sample-data
        Write-Host "✅ Sample data populated successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "⚠️ Sample data population failed, but continuing..." -ForegroundColor Yellow
    }
    finally {
        Remove-Item "populate-sample-data.cs" -ErrorAction SilentlyContinue
    }
}

Write-Host "🎉 IndFusion Semantic RAG Platform initialization completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next steps:" -ForegroundColor Cyan
Write-Host "1. Run the application: dotnet run --project code/IndFusion.SemanticRag.WebAPI" -ForegroundColor White
Write-Host "2. Open http://localhost:5000/swagger to explore the API" -ForegroundColor White
Write-Host "3. Test the RAG endpoint: POST /api/rag/query" -ForegroundColor White
Write-Host "4. Test vector search: GET /api/rag/search?query=error handling" -ForegroundColor White
Write-Host ""
Write-Host "🔧 Infrastructure services:" -ForegroundColor Cyan
Write-Host "- Qdrant: http://localhost:6333" -ForegroundColor White
Write-Host "- Ollama: http://localhost:11434" -ForegroundColor White
Write-Host "- Neo4j: http://localhost:7474" -ForegroundColor White
Write-Host "- Redis: localhost:6379" -ForegroundColor White




