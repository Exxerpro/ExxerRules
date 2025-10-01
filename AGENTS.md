# Repository Guidelines

## Project Structure & Module Organization
- `src/code/` hosts the MCP Core, Server, and Web services plus packaging artifacts; every project participates in `IndFusion.sln`.
- `src/ExxerRules/ExxerAI.Rules.Analyzer/` contains the Roslyn analyzers and code fixes published as ExxerRules.
- `src/test/` mirrors runtime scenarios, with xUnit v3 suites like `IndFusion.Analyzer.Tests` and scenario folders under `TestCases/`.
- Tooling and automation live in `src/scripts/` (PowerShell/Python), while IDE harnesses sit under `src/VS` and `src/VSCode`.
- Offline assets and pinned feeds are cached beneath `artifacts/nuget/offline/`.

## Build, Test, and Development Commands
- `dotnet restore IndFusion.sln` - restores using central packages in `Directory.Packages.props`.
- `dotnet build IndFusion.sln -c Release` - compiles all services and analyzers with warnings treated as errors.
- `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release --collect:"XPlat Code Coverage"` - runs analyzer regressions and emits coverage results.
- `pwsh src/scripts/build-offline.ps1 -Configuration Release` - one-shot restore and build while priming offline caches.
- `pwsh src/scripts/Generate-XmlDocs.ps1` - regenerates XML docs because XML warnings are unsuppressed.

## Coding Style & Naming Conventions
- `.editorconfig` enforces tabs (size 4), CRLF line endings, and trimmed trailing whitespace.
- Follow C# defaults: PascalCase for types and methods, camelCase for locals, `_camelCase` for private fields, and an `Async` suffix for asynchronous APIs.
- Document every public or protected member with XML comments; builds fail on CS1591 warnings.
- Run `dotnet format --verify-no-changes` or apply EXXER900 quick fixes before submitting; log justified suppressions in `SUPPRESSIONS.md`.

## Testing Guidelines
- Tests use xUnit v3 with Shouldly assertions; reuse `AnalyzerTestHelper` for Roslyn driver setup.
- Name tests `Should_Action_WhenCondition` and place new analyzer scenarios under `src/test/.../TestCases/`.
- Capture coverage with `--collect:"XPlat Code Coverage"` or `/p:CollectCoverage=true` before final review.

## Commit & Pull Request Guidelines
- Format commits as `type(scope): summary` and append `[ci full]` when a complete pipeline run is required.
- Describe behavioural risks, mitigations, and test evidence in pull requests, and link related issues.
- Call out architecture impacts, offline compatibility, and attach screenshots for UI-facing updates.

## Security & Configuration Tips
- Prefer offline feeds via `artifacts/nuget/offline/` when working in air-gapped environments.
- Avoid resetting user-edited files; confirm unexpected diffs with the original author before proceeding.
