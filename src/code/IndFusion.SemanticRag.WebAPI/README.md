# IndFusion Semantic RAG Platform

A comprehensive Code Standards as a Service platform that combines semantic pattern analysis with Retrieval-Augmented Generation (RAG) capabilities.

## 🚀 Features

- **Vector Search**: Semantic similarity search using Qdrant
- **LLM Integration**: Text generation and embeddings via Ollama
- **Knowledge Graph**: Code relationships and patterns in Neo4j
- **Pattern Analysis**: Semantic pattern detection and enforcement
- **RAG Queries**: AI-powered code guidance and suggestions
- **REST API**: Complete Web API with Swagger documentation

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Web API       │    │   RAG Service    │    │  Vector Search  │
│   (Controllers) │◄──►│   (LLM + Search) │◄──►│   (Qdrant)      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│ Pattern Engine  │    │ Knowledge Graph │    │ Code Analysis   │
│ (Semantic)      │    │ (Neo4j)         │    │ (Roslyn)        │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🛠️ Technology Stack

- **.NET 8**: Core application framework
- **Qdrant**: Vector database for embeddings
- **Ollama**: Local LLM for text generation and embeddings
- **Neo4j**: Graph database for code relationships
- **Redis**: Caching layer
- **Docker**: Containerized infrastructure

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK
- Docker Desktop
- PowerShell (for initialization script)

### 1. Initialize Services

```powershell
# Run the initialization script
.\scripts\initialize-services.ps1
```

This will:
- Start Docker services (Qdrant, Ollama, Neo4j, Redis)
- Build the application
- Populate sample data

### 2. Run the Application

```bash
cd code/IndFusion.SemanticRag.WebAPI
dotnet run
```

### 3. Access the API

- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

## 📚 API Endpoints

### RAG Operations

- `POST /api/rag/query` - Perform RAG queries
- `GET /api/rag/search` - Vector similarity search
- `POST /api/rag/store` - Store documents

### Semantic Analysis

- `POST /api/semanticrag/analyze-code` - Analyze code for patterns
- `GET /api/semanticrag/analyze-project` - Analyze entire project
- `GET /api/semanticrag/guidance` - Get pattern guidance

## 🔧 Configuration

The application uses `appsettings.json` for configuration:

```json
{
  "Qdrant": {
    "Host": "localhost",
    "Port": 6333,
    "CollectionName": "semantic_patterns"
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "EmbeddingModel": "nomic-embed-text",
    "TextModel": "llama3.2"
  },
  "Neo4j": {
    "Uri": "bolt://localhost:7687",
    "Username": "neo4j",
    "Password": "password"
  }
}
```

## 🧪 Testing the Platform

### 1. Test Vector Search

```bash
curl -X GET "http://localhost:5000/api/rag/search?query=error%20handling&limit=5"
```

### 2. Test RAG Query

```bash
curl -X POST "http://localhost:5000/api/rag/query" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "How should I handle errors in C#?",
    "maxResults": 3
  }'
```

### 3. Store a Document

```bash
curl -X POST "http://localhost:5000/api/rag/store" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "doc_001",
    "content": "Use Result<T> pattern for error handling",
    "metadata": {
      "type": "pattern",
      "category": "error_handling"
    }
  }'
```

## 🏗️ Development

### Project Structure

```
IndFusion.SemanticRag.Domain/          # Domain models and entities
IndFusion.SemanticRag.Application/     # Application interfaces
IndFusion.SemanticRag.Infrastructure/ # Infrastructure implementations
IndFusion.SemanticRag.WebAPI/         # Web API controllers
```

### Building

```bash
# Restore packages
dotnet restore IndFusion.sln

# Build solution
dotnet build IndFusion.sln -c Release

# Run tests
dotnet test IndFusion.sln -c Release
```

### Docker Development

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

## 🔍 Monitoring

### Health Checks

- **Application**: http://localhost:5000/health
- **Qdrant**: http://localhost:6333/health
- **Ollama**: http://localhost:11434/api/tags
- **Neo4j**: http://localhost:7474

### Logs

Application logs are written to:
- Console (Development)
- File: `logs/indfusion-semantic-rag.log`
- Seq: http://localhost:5341 (if configured)

## 🚀 Deployment

### Docker Compose

```bash
# Production deployment
docker-compose -f docker-compose.prod.yml up -d
```

### Environment Variables

```bash
export ASPNETCORE_ENVIRONMENT=Production
export Ollama__BaseUrl=http://ollama:11434
export Qdrant__Host=qdrant
export Neo4j__Uri=bolt://neo4j:7687
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License

This project is part of the IndFusion ecosystem. See the main repository for license information.

## 🆘 Support

For issues and questions:
- Create an issue in the repository
- Check the troubleshooting guide
- Review the API documentation

## 🔮 Roadmap

- [ ] Multi-language support (TypeScript, Python)
- [ ] Pattern evolution learning
- [ ] Cross-repository pattern tracking
- [ ] VS Code extension
- [ ] CI/CD pipeline integration
- [ ] Compliance reporting dashboard





