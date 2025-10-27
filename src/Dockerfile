# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["code/IndFusion.SemanticRag.WebAPI/IndFusion.SemanticRag.WebAPI.csproj", "code/IndFusion.SemanticRag.WebAPI/"]
COPY ["code/IndFusion.SemanticRag.Application/IndFusion.SemanticRag.Application.csproj", "code/IndFusion.SemanticRag.Application/"]
COPY ["code/IndFusion.SemanticRag.Infrastructure/IndFusion.SemanticRag.Infrastructure.csproj", "code/IndFusion.SemanticRag.Infrastructure/"]
COPY ["code/IndFusion.SemanticRag.Domain/IndFusion.SemanticRag.Domain.csproj", "code/IndFusion.SemanticRag.Domain/"]
COPY ["code/IndFusion.Mcp.Core/IndFusion.Mcp.Core.csproj", "code/IndFusion.Mcp.Core/"]
COPY ["code/IndFusion.Mcp.Server/IndFusion.Mcp.Server.csproj", "code/IndFusion.Mcp.Server/"]

# Copy solution and package files
COPY ["IndFusion.sln", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Build.targets", "."]
COPY ["Directory.Packages.props", "."]
COPY ["NuGet.config", "."]

# Restore dependencies
RUN dotnet restore "IndFusion.sln"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/code/IndFusion.SemanticRag.WebAPI"
RUN dotnet build "IndFusion.SemanticRag.WebAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "IndFusion.SemanticRag.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Install Tesseract OCR
RUN apt-get update && \
    apt-get install -y tesseract-ocr tesseract-ocr-eng && \
    rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "IndFusion.SemanticRag.WebAPI.dll"]
