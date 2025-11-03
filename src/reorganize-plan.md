# Solution Reorganization Plan

## Target Structure (matching Visual Studio logical folders)

**code/**
- **Analyzer/**
  - IndFusion.Analyzer/
  - IndFusion.Fixer/
- **Cli/**
  - IndFusion.Tools.Cli/
  - IndFusion.Tools.Cli.Core/
  - IndFusion.Tools.Cli.Console/
- **Mcp/**
  - IndFusion.Mcp.Core/
  - IndFusion.Mcp.Server/
  - IndFusion.Mcp.Web/
- **SemanticRag/**
  - IndFusion.SemanticRag.Domain/
  - IndFusion.SemanticRag.Application/
  - IndFusion.SemanticRag.Infrastructure/
  - IndFusion.SemanticRag.WebAPI/
- IndFusion.Packaging/ (stays at code level)

**test/**
- **AnalyzerTests/**
  - IndFusion.Analyzer.Tests/
- **CliTests/**
  - IndFusion.Tools.Cli.Tests/
- **McpTests/**
  - IndFusion.Mcp.Core.Tests/
  - IndFusion.Mcp.Server.Tests/
  - IndFusion.Mcp.Tests/
  - IndFusion.Mcp.Tests.Integration/
  - IndFusion.Mcp.Web.Tests/
- **SemanticRagTests/**
  - IndFusion.SemanticRag.Tests/
  - IndFusion.SemanticRag.Tests.Unit/
  - IndFusion.SemanticRag.Tests.Integration/
  - IndFusion.SemanticRag.Tests.Architecture/
  - IndFusion.SemanticRag.Tests.System/
- IndFusion.SemanticRag.Tests.Standalone/ (stays at test level)
- IndFusion.Architecture.Tests/ (stays at test level)

## Project Reference Path Updates Needed

After moving, all ProjectReference paths in .csproj files need to be updated to reflect new relative paths.

