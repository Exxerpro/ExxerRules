# Repository Guidelines

## Project Structure & Module Organization
- Source: `code/IndFusion.*` (e.g., `IndFusion.Analyzer`, `IndFusion.Fixer`, `IndFusion.Mcp.*`, `IndFusion.Tools.Cli`).
- Tests: `test/IndFusion.*.Tests` mirroring the product projects (xUnit v3).
- Solutions: `IndFusion.sln` (runtime + tests) and `IndFusion.Fixer.Vsix.sln` (VSIX, analyzer, fixer).
- Central versions in `Directory.Packages.props`; common build in `Directory.Build.props`; SDK pinned via `global.json` (net10 preview).

## Build, Test, and Development Commands
- Restore (fallback online): `dotnet restore IndFusion.sln --configfile NuGet.online.config`
- Build (Debug): `dotnet build IndFusion.sln -c Debug --configfile NuGet.online.config`
- Test: `dotnet test IndFusion.sln -c Debug --configfile NuGet.online.config`
- Strict offline: populate with `pwsh VS/fetch-packages.ps1 -SkipDownloadIfExists`, then use `NuGet.config`.
- Run web app: `dotnet run --project code/IndFusion.Mcp.Web`
- Run CLI: `dotnet run --project code/IndFusion.Tools.Cli`

## Coding Style & Naming Conventions
- Language: C# `latest`; `Nullable` and `ImplicitUsings` enabled; warnings as errors where applicable.
- Indentation: tabs; braces on new lines (Allman).
- Naming: PascalCase for public API; camelCase for locals/parameters; private fields `_camelCase`.
- Docs: XML docs generated; add `<summary>` for public APIs.

## Testing Guidelines
- Framework: xUnit v3 (via central management) + `Microsoft.NET.Test.Sdk`; coverage with `coverlet.collector`.
- Run all: `dotnet test IndFusion.sln`
- Useful filters: `dotnet test -t` then `dotnet test --filter "FullyQualifiedName~AnalyzerTests"`.

## Commit & Pull Request Guidelines
- Commits: imperative, concise (≤72 chars), describe what/why (e.g., "Align namespaces to IndFusion.*").
- PRs: clear description, linked issues, and tests for new behavior; include screenshots for UI.

## Security & Configuration Tips
- NuGet: `NuGet.online.config` (fallback to nuget.org) or strict `NuGet.config` (offline only). See `OFFLINE-NUGET.md`.
- SourceLink is enabled; do not commit secrets or machine‑specific paths.
