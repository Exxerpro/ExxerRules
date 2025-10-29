# Populate Sample Documents for IndFusion Semantic RAG Platform
# This script creates sample documents and ingests them into the knowledge base

param(
    [string]$ApiBaseUrl = "http://localhost:5000",
    [string]$SampleDataPath = "./sample-data",
    [switch]$CreateSampleData = $true,
    [switch]$IngestDocuments = $true
)

Write-Host "📚 Populating sample documents for IndFusion Semantic RAG Platform..." -ForegroundColor Green

# Create sample data directory
if ($CreateSampleData) {
    Write-Host "📁 Creating sample data directory..." -ForegroundColor Yellow
    
    if (Test-Path $SampleDataPath) {
        Remove-Item $SampleDataPath -Recurse -Force
    }
    
    New-Item -ItemType Directory -Path $SampleDataPath -Force | Out-Null
    
    # Create sample C# code files
    $csharpCode = @"
using System;
using System.Threading.Tasks;

namespace SampleProject
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        
        public async Task<Result<User>> GetUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Failed to get user: {ex.Message}");
            }
        }
    }
}
"@

    $csharpCode | Out-File -FilePath "$SampleDataPath/UserService.cs" -Encoding UTF8

    # Create sample TypeScript code
    $typescriptCode = @"
interface User {
    id: number;
    name: string;
    email: string;
}

class UserService {
    private userRepository: IUserRepository;
    
    constructor(userRepository: IUserRepository) {
        this.userRepository = userRepository;
    }
    
    async getUser(id: number): Promise<User | null> {
        try {
            return await this.userRepository.findById(id);
        } catch (error) {
            console.error('Failed to get user:', error);
            return null;
        }
    }
}
"@

    $typescriptCode | Out-File -FilePath "$SampleDataPath/UserService.ts" -Encoding UTF8

    # Create sample Python code
    $pythonCode = @"
from typing import Optional
from dataclasses import dataclass

@dataclass
class User:
    id: int
    name: str
    email: str

class UserService:
    def __init__(self, user_repository):
        self.user_repository = user_repository
    
    async def get_user(self, user_id: int) -> Optional[User]:
        try:
            return await self.user_repository.find_by_id(user_id)
        except Exception as e:
            print(f"Failed to get user: {e}")
            return None
"@

    $pythonCode | Out-File -FilePath "$SampleDataPath/user_service.py" -Encoding UTF8

    # Create sample markdown documentation
    $markdownDoc = @"
# User Service Documentation

## Overview
The User Service provides functionality for managing user data in the application.

## Features
- User retrieval by ID
- Error handling with Result pattern
- Async/await support
- Cancellation token support

## Usage Example
```csharp
var userService = new UserService(userRepository);
var result = await userService.GetUserAsync(123);
if (result.IsSuccess)
{
    var user = result.Value;
    // Use user data
}
```

## Best Practices
1. Always use Result<T> pattern for error handling
2. Include CancellationToken in async methods
3. Validate input parameters
4. Use dependency injection
"@

    $markdownDoc | Out-File -FilePath "$SampleDataPath/UserService.md" -Encoding UTF8

    # Create sample text file
    $textDoc = @"
Error Handling Best Practices

1. Use Result<T> pattern instead of exceptions for control flow
2. Validate null parameters at method entry
3. Include CancellationToken in async methods
4. Use structured logging for errors
5. Return meaningful error messages

Common Patterns:
- Result<T>.Success(value) for successful operations
- Result<T>.Failure(message) for failed operations
- throw new ArgumentNullException() for null validation
- ConfigureAwait(false) for background operations
"@

    $textDoc | Out-File -FilePath "$SampleDataPath/error-handling.txt" -Encoding UTF8

    Write-Host "✅ Sample data created in $SampleDataPath" -ForegroundColor Green
}

# Ingest documents if requested
if ($IngestDocuments) {
    Write-Host "📤 Ingesting documents into knowledge base..." -ForegroundColor Yellow
    
    try {
        # Test API health
        $healthResponse = Invoke-RestMethod -Uri "$ApiBaseUrl/health" -Method Get
        Write-Host "✅ API is healthy" -ForegroundColor Green
        
        # Ingest directory
        $ingestRequest = @{
            DirectoryPath = (Resolve-Path $SampleDataPath).Path
            IngestionOptions = @{
                GenerateEmbeddings = $true
                StoreInVectorDatabase = $true
                StoreInKnowledgeGraph = $true
                SourceId = "sample_data"
                Tags = @("sample", "documentation", "code")
            }
        } | ConvertTo-Json -Depth 10
        
        $ingestResponse = Invoke-RestMethod -Uri "$ApiBaseUrl/api/document/ingest-directory" -Method Post -Body $ingestRequest -ContentType "application/json"
        
        $successCount = ($ingestResponse | Where-Object { $_.Success -eq $true }).Count
        $totalCount = $ingestResponse.Count
        
        Write-Host "✅ Ingested $successCount/$totalCount documents successfully" -ForegroundColor Green
        
        # Display results
        foreach ($result in $ingestResponse) {
            if ($result.Success) {
                Write-Host "  📄 $($result.DocumentId): $($result.ChunksCreated) chunks, $($result.ElapsedMilliseconds)ms" -ForegroundColor Cyan
            } else {
                Write-Host "  ❌ $($result.DocumentId): $($result.ErrorMessage)" -ForegroundColor Red
            }
        }
        
    } catch {
        Write-Host "❌ Error ingesting documents: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "Make sure the API is running on $ApiBaseUrl" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "🎉 Sample document population completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next steps:" -ForegroundColor Cyan
Write-Host "1. Test document processing: POST $ApiBaseUrl/api/document/process" -ForegroundColor White
Write-Host "2. Test document ingestion: POST $ApiBaseUrl/api/document/ingest" -ForegroundColor White
Write-Host "3. Test RAG queries: POST $ApiBaseUrl/api/rag/query" -ForegroundColor White
Write-Host "4. Test vector search: GET $ApiBaseUrl/api/rag/search?query=error%20handling" -ForegroundColor White
Write-Host ""
Write-Host "📁 Sample data location: $SampleDataPath" -ForegroundColor Cyan





