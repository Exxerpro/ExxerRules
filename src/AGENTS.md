# Repository Guidelines

## Project Structure & Module Organization
- Source: `ExxerRules/` (Roslyn analyzers), `ExxerFactor/` (Console/Web/Core), `ExxerFactor.Rules/` (rule implementations & tests).
- Tests: `ExxerRules.Tests/`, `ExxerFactor.Rules/*Tests/` (xUnit v3).
- Solution: `IndFusion.sln`. Central package versions in `Directory.Packages.props`; shared build settings in `Directory.Build.props`.
- NuGet: `NuGet.config` (supports offline flow; see `OFFLINE-NUGET.md`).

## Build, Test, and Development Commands
- Restore: `dotnet restore --configfile NuGet.config`
- Build (Debug): `dotnet build IndFusion.sln -c Debug`
- Build (Release): `dotnet build IndFusion.sln -c Release`
- Test all: `dotnet test IndFusion.sln -c Debug`
- Run console app: `dotnet run --project ExxerFactor/ExxerFactor.Mcp.ConsoleApp`
- Run web app: `dotnet run --project ExxerFactor/ExxerFactor.Mcp.Web`
- Format (optional): `dotnet format` (keep diffs small and consistent).

## Coding Style & Naming Conventions
- Language: C# `latest`; `Nullable` enabled; `ImplicitUsings` enabled; warnings treated as errors.
- Indentation: tabs; braces on new lines (Allman style).
- Naming: PascalCase for public types/members; camelCase for locals/parameters; private fields `_camelCase`.
- Documentation: XML docs are generated; add `<summary>` where public API is exposed.

## Testing Guidelines
- Framework: xUnit v3 with `Microsoft.Testing.Platform`; coverage via `coverlet.collector`.
- Run project tests: `dotnet test ExxerRules.Tests/ExxerRules.Tests.csproj`
- Filter by trait/name example: `dotnet test -t` then `dotnet test --filter "FullyQualifiedName~AnalyzerTests"`.
- Keep tests fast, deterministic, and isolated; prefer data builders in existing test helpers.

## Commit & Pull Request Guidelines
- Commits: concise, imperative subject (≤72 chars) describing what/why (e.g., "Fix analyzer null checks").
- History shows short, action‑oriented messages; keep that style and group related changes.
- PRs: clear description, scope of change, linked issues, and before/after notes or screenshots for UI. Include test coverage for new behavior and analyzers.

## Security & Configuration Tips
- Use the provided `NuGet.config`; for offline development follow `OFFLINE-NUGET.md`.
- Embed SourceLink is enabled; avoid committing secrets or local paths. Environment settings should live in user‑secrets or dev profiles, not source.

