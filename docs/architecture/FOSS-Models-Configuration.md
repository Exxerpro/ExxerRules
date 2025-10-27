# FOSS Models Configuration Guide

## Overview

This document provides a comprehensive guide for configuring the IndFusion Semantic RAG platform using only Free and Open Source Software (FOSS) models and services. This approach ensures cost-effectiveness, data privacy, and full control over the AI infrastructure.

## FOSS Technology Stack

### 🧠 **LLM Models (Ollama)**

#### Recommended Models

| Model | Size | Use Case | Performance | Memory Requirements |
|-------|------|----------|-------------|-------------------|
| **Llama 2 7B** | 7B | General purpose, code analysis | High | 8GB RAM |
| **Llama 2 13B** | 13B | Advanced reasoning, complex tasks | Very High | 16GB RAM |
| **Mistral 7B** | 7B | Fast inference, good for real-time | High | 8GB RAM |
| **CodeLlama 7B** | 7B | Code-specific tasks, syntax analysis | High | 8GB RAM |
| **CodeLlama 13B** | 13B | Complex code understanding | Very High | 16GB RAM |
| **Phi-3 Mini** | 3.8B | Lightweight, fast inference | Medium | 4GB RAM |
| **Gemma 7B** | 7B | Balanced performance/speed | High | 8GB RAM |

#### Embedding Models

| Model | Dimensions | Use Case | Performance |
|-------|------------|----------|-------------|
| **sentence-transformers/all-MiniLM-L6-v2** | 384 | Fast embeddings | High |
| **sentence-transformers/all-mpnet-base-v2** | 768 | High quality embeddings | Very High |
| **BAAI/bge-large-en-v1.5** | 1024 | Best quality embeddings | Excellent |

### 🔍 **OCR Engine (Tesseract)**

Tesseract is the industry-standard open-source OCR engine with excellent accuracy for most document types.

#### Supported Languages
- English (default)
- Spanish, French, German, Italian
- Chinese, Japanese, Korean
- Arabic, Hebrew, Russian
- And 100+ other languages

#### Performance Characteristics
- **Accuracy**: 95-99% for clean documents
- **Speed**: 1-5 seconds per page
- **Memory**: Low memory footprint
- **Formats**: PDF, PNG, JPEG, TIFF, BMP

### 🗄️ **Vector Database (Qdrant)**

Qdrant is a high-performance vector database designed for machine learning applications.

#### Features
- **Scalability**: Horizontal scaling support
- **Performance**: Sub-millisecond search times
- **Filtering**: Rich metadata filtering capabilities
- **Persistence**: On-disk storage with memory caching
- **API**: REST and gRPC APIs

### 🕸️ **Graph Database (Neo4j)**

Neo4j is the leading graph database for relationship modeling and complex queries.

#### Features
- **ACID Compliance**: Full transaction support
- **Cypher Query Language**: Powerful graph query language
- **Scalability**: Enterprise clustering support
- **APIs**: REST, Bolt, and Java APIs
- **Visualization**: Built-in graph visualization tools

## Configuration Examples

### Ollama Configuration

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama2:7b",
    "EmbeddingModel": "nomic-embed-text",
    "Timeout": 300,
    "MaxRetries": 3,
    "Temperature": 0.7,
    "TopP": 0.9,
    "MaxTokens": 2048
  }
}
```

### Tesseract Configuration

```json
{
  "Tesseract": {
    "DataPath": "/usr/share/tesseract-ocr/4.00/tessdata",
    "Language": "eng",
    "PageSegmentationMode": "PSM_AUTO",
    "OCRMode": "OEM_LSTM_ONLY",
    "Whitelist": "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,!?;:()[]{}\"'",
    "Blacklist": "",
    "ConfidenceThreshold": 60
  }
}
```

### Qdrant Configuration

```json
{
  "Qdrant": {
    "Host": "localhost",
    "Port": 6333,
    "ApiKey": "",
    "Timeout": 30,
    "CollectionName": "semantic_rag",
    "VectorSize": 384,
    "Distance": "Cosine",
    "ReplicationFactor": 1,
    "WriteConsistencyFactor": 1
  }
}
```

### Neo4j Configuration

```json
{
  "Neo4j": {
    "Uri": "bolt://localhost:7687",
    "Username": "neo4j",
    "Password": "password",
    "Database": "neo4j",
    "MaxConnectionPoolSize": 50,
    "ConnectionTimeout": 30,
    "MaxTransactionRetryTime": 30
  }
}
```

## Implementation Details

### Ollama Service Integration

```csharp
// Infrastructure/Options/OllamaOptions.cs
public class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama2:7b";
    public string EmbeddingModel { get; set; } = "nomic-embed-text";
    public int Timeout { get; set; } = 300;
    public int MaxRetries { get; set; } = 3;
    public double Temperature { get; set; } = 0.7;
    public double TopP { get; set; } = 0.9;
    public int MaxTokens { get; set; } = 2048;
}

// Infrastructure/Adapters/Processing/OllamaLLMExtractionAdapter.cs
public class OllamaLLMExtractionAdapter : ILLMExtractionService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;
    private readonly ILogger<OllamaLLMExtractionAdapter> _logger;

    public OllamaLLMExtractionAdapter(HttpClient httpClient, IOptions<OllamaOptions> options, ILogger<OllamaLLMExtractionAdapter> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ExtractionResult> ExtractEntitiesAsync(string text, ExtractionOptions options, CancellationToken cancellationToken = default)
    {
        var prompt = CreateEntityExtractionPrompt(text, options);
        var request = new OllamaRequest
        {
            Model = _options.Model,
            Prompt = prompt,
            Stream = false,
            Options = new OllamaOptions
            {
                Temperature = _options.Temperature,
                TopP = _options.TopP,
                NumPredict = _options.MaxTokens
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_options.BaseUrl}/api/generate", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken: cancellationToken);
            return ParseExtractionResult(result.Response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract entities using Ollama");
            throw new LLMExtractionException("Entity extraction failed", ex);
        }
    }

    public async Task<EmbeddingResult> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var request = new OllamaEmbeddingRequest
        {
            Model = _options.EmbeddingModel,
            Prompt = text
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_options.BaseUrl}/api/embeddings", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: cancellationToken);
            return new EmbeddingResult
            {
                Embedding = result.Embedding,
                Model = _options.EmbeddingModel,
                Dimensions = result.Embedding.Length
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate embedding using Ollama");
            throw new LLMExtractionException("Embedding generation failed", ex);
        }
    }

    private string CreateEntityExtractionPrompt(string text, ExtractionOptions options)
    {
        return $@"
Extract entities from the following text and return as JSON. Focus on: {string.Join(", ", options.EntityTypes)}.

Text: {text}

Return format:
{{
  ""entities"": [
    {{""name"": ""entity_name"", ""type"": ""entity_type"", ""confidence"": 0.95, ""start"": 0, ""end"": 10}}
  ],
  ""relationships"": [
    {{""source"": ""entity1"", ""target"": ""entity2"", ""type"": ""relationship_type"", ""confidence"": 0.90}}
  ]
}}";
    }
}
```

### Tesseract OCR Integration

```csharp
// Infrastructure/Options/TesseractOptions.cs
public class TesseractOptions
{
    public string DataPath { get; set; } = "/usr/share/tesseract-ocr/4.00/tessdata";
    public string Language { get; set; } = "eng";
    public PageSegmentationMode PageSegmentationMode { get; set; } = PageSegmentationMode.Auto;
    public OcrEngineMode OcrEngineMode { get; set; } = OcrEngineMode.LstmOnly;
    public string Whitelist { get; set; } = "";
    public string Blacklist { get; set; } = "";
    public int ConfidenceThreshold { get; set; } = 60;
}

// Infrastructure/Adapters/Processing/TesseractOCRAdapter.cs
public class TesseractOCRAdapter : IOCRService
{
    private readonly TesseractEngine _tesseractEngine;
    private readonly TesseractOptions _options;
    private readonly ILogger<TesseractOCRAdapter> _logger;

    public TesseractOCRAdapter(IOptions<TesseractOptions> options, ILogger<TesseractOCRAdapter> logger)
    {
        _options = options.Value;
        _logger = logger;
        _tesseractEngine = new TesseractEngine(_options.DataPath, _options.Language, _options.OcrEngineMode);
        
        ConfigureTesseract();
    }

    public async Task<OCRResult> ExtractTextAsync(ImageInput image, CancellationToken cancellationToken = default)
    {
        try
        {
            using var img = Pix.LoadFromMemory(image.Data);
            using var page = _tesseractEngine.Process(img);
            
            var text = page.GetText();
            var confidence = page.GetMeanConfidence();
            var words = ExtractWords(page);
            
            return new OCRResult
            {
                Text = text,
                Confidence = confidence,
                Words = words,
                Status = confidence >= _options.ConfidenceThreshold ? OCRStatus.Success : OCRStatus.LowConfidence
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract text using Tesseract");
            throw new OCRException("Text extraction failed", ex);
        }
    }

    public async Task<OCRResult> ExtractTextFromPdfAsync(PdfInput pdf, CancellationToken cancellationToken = default)
    {
        try
        {
            using var pdfDoc = PdfDocument.Open(pdf.Data);
            var allText = new StringBuilder();
            var allWords = new List<OCRWord>();
            var totalConfidence = 0.0;
            var pageCount = 0;

            foreach (var page in pdfDoc.GetPages())
            {
                using var pix = page.ToPix();
                using var ocrPage = _tesseractEngine.Process(pix);
                
                allText.AppendLine(ocrPage.GetText());
                allWords.AddRange(ExtractWords(ocrPage));
                totalConfidence += ocrPage.GetMeanConfidence();
                pageCount++;
            }

            return new OCRResult
            {
                Text = allText.ToString(),
                Confidence = pageCount > 0 ? totalConfidence / pageCount : 0,
                Words = allWords,
                Status = OCRStatus.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract text from PDF using Tesseract");
            throw new OCRException("PDF text extraction failed", ex);
        }
    }

    private void ConfigureTesseract()
    {
        _tesseractEngine.SetVariable("tessedit_pageseg_mode", ((int)_options.PageSegmentationMode).ToString());
        
        if (!string.IsNullOrEmpty(_options.Whitelist))
            _tesseractEngine.SetVariable("tessedit_char_whitelist", _options.Whitelist);
            
        if (!string.IsNullOrEmpty(_options.Blacklist))
            _tesseractEngine.SetVariable("tessedit_char_blacklist", _options.Blacklist);
    }

    private List<OCRWord> ExtractWords(Page page)
    {
        var words = new List<OCRWord>();
        using var iter = page.GetIterator();
        
        iter.Begin();
        do
        {
            if (iter.IsAtBeginningOf(PageIteratorLevel.Word))
            {
                var word = new OCRWord
                {
                    Text = iter.GetText(PageIteratorLevel.Word),
                    Confidence = iter.GetConfidence(PageIteratorLevel.Word),
                    BoundingBox = iter.GetBoundingBox(PageIteratorLevel.Word)
                };
                words.Add(word);
            }
        } while (iter.Next(PageIteratorLevel.Word));

        return words;
    }

    public void Dispose()
    {
        _tesseractEngine?.Dispose();
    }
}
```

## Deployment and Setup

### Docker Compose Configuration

```yaml
version: '3.8'

services:
  ollama:
    image: ollama/ollama:latest
    ports:
      - "11434:11434"
    volumes:
      - ollama_data:/root/.ollama
    environment:
      - OLLAMA_HOST=0.0.0.0
    deploy:
      resources:
        reservations:
          devices:
            - driver: nvidia
              count: 1
              capabilities: [gpu]

  qdrant:
    image: qdrant/qdrant:latest
    ports:
      - "6333:6333"
      - "6334:6334"
    volumes:
      - qdrant_data:/qdrant/storage
    environment:
      - QDRANT__SERVICE__HTTP_PORT=6333
      - QDRANT__SERVICE__GRPC_PORT=6334

  neo4j:
    image: neo4j:5.15-community
    ports:
      - "7474:7474"
      - "7687:7687"
    volumes:
      - neo4j_data:/data
      - neo4j_logs:/logs
    environment:
      - NEO4J_AUTH=neo4j/password
      - NEO4J_PLUGINS=["apoc"]

  semantic-rag:
    build: .
    ports:
      - "5000:5000"
    environment:
      - Ollama__BaseUrl=http://ollama:11434
      - Qdrant__Host=qdrant
      - Neo4j__Uri=bolt://neo4j:7687
    depends_on:
      - ollama
      - qdrant
      - neo4j

volumes:
  ollama_data:
  qdrant_data:
  neo4j_data:
  neo4j_logs:
```

### Model Installation Script

```bash
#!/bin/bash
# install-models.sh

echo "Installing Ollama models..."

# Install LLM models
ollama pull llama2:7b
ollama pull mistral:7b
ollama pull codellama:7b
ollama pull phi3:mini

# Install embedding models
ollama pull nomic-embed-text
ollama pull all-minilm:l6-v2

echo "Models installed successfully!"
echo "Available models:"
ollama list
```

## Performance Optimization

### Model Selection Guidelines

1. **Development/Testing**: Use smaller models (Phi-3 Mini, Llama 2 7B)
2. **Production**: Use larger models (Llama 2 13B, CodeLlama 13B) for better accuracy
3. **Real-time Applications**: Use Mistral 7B for faster inference
4. **Code-specific Tasks**: Use CodeLlama models

### Hardware Requirements

| Component | Minimum | Recommended | High Performance |
|-----------|---------|-------------|------------------|
| **CPU** | 4 cores | 8 cores | 16+ cores |
| **RAM** | 16GB | 32GB | 64GB+ |
| **GPU** | None | RTX 3060 (8GB) | RTX 4090 (24GB) |
| **Storage** | 100GB SSD | 500GB NVMe | 1TB+ NVMe |

### Performance Tuning

```csharp
// Optimize Ollama for production
public class OptimizedOllamaOptions : OllamaOptions
{
    public OptimizedOllamaOptions()
    {
        Temperature = 0.3; // Lower for more deterministic results
        TopP = 0.8; // Focus on most likely tokens
        MaxTokens = 1024; // Limit response length
        NumCtx = 4096; // Increase context window
        NumGpu = 1; // Use GPU if available
    }
}
```

## Benefits of FOSS Approach

### 💰 **Cost Benefits**
- **No API Costs**: No per-token or per-request charges
- **No Vendor Lock-in**: Full control over infrastructure
- **Predictable Costs**: Only hardware and hosting costs

### 🔒 **Privacy & Security**
- **Data Privacy**: All data stays on-premises
- **No Data Sharing**: No data sent to external services
- **Compliance**: Easier to meet regulatory requirements
- **Audit Trail**: Full control over data processing

### 🛠️ **Control & Customization**
- **Model Fine-tuning**: Can fine-tune models for specific use cases
- **Custom Models**: Can integrate custom or specialized models
- **Offline Operation**: Works without internet connectivity
- **Version Control**: Full control over model versions

### 📈 **Performance**
- **Latency**: Lower latency for local processing
- **Throughput**: No rate limiting from external APIs
- **Reliability**: No dependency on external service availability
- **Scalability**: Can scale horizontally as needed

## Migration from Cloud Services

### From OpenAI to Ollama

```csharp
// Before (OpenAI)
public class OpenAIAdapter : ILLMExtractionService
{
    private readonly OpenAIClient _client;
    
    public async Task<ExtractionResult> ExtractEntitiesAsync(string text, ExtractionOptions options)
    {
        var response = await _client.ChatCompletions.CreateChatCompletionAsync(
            new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage> { new ChatMessage("user", prompt) },
                Model = "gpt-3.5-turbo"
            });
        // Process response...
    }
}

// After (Ollama)
public class OllamaAdapter : ILLMExtractionService
{
    private readonly HttpClient _httpClient;
    
    public async Task<ExtractionResult> ExtractEntitiesAsync(string text, ExtractionOptions options)
    {
        var request = new OllamaRequest
        {
            Model = "llama2:7b",
            Prompt = prompt,
            Stream = false
        };
        
        var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/generate", request);
        // Process response...
    }
}
```

This FOSS approach provides a robust, cost-effective, and privacy-focused solution for the IndFusion Semantic RAG platform while maintaining high performance and flexibility.

---

**Last Updated**: 2024-01-XX  
**Updated By**: PM Agent

