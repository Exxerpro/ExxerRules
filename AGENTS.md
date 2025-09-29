# Repository Guidelines

## Project Structure & Module Organization
- `src/code/` holds MCP services (Core, Server, Web) plus packaging; all compile under `IndFusion.sln`.
- `src/ExxerRules/ExxerAI.Rules.Analyzer/` contains the Roslyn analyzers and code fixes shipped as the ExxerRules package.
- `src/test/` hosts xUnit v3 suites (e.g., `IndFusion.Analyzer.Tests`) organised by scenario folders such as `TestCases/NullSafety`.
- `src/scripts/` exposes PowerShell and Python helpers for offline builds, XML docs, and data maintenance; `artifacts/nuget/offline/` caches pinned feeds for air-gapped work.
- `src/VS` and `src/VSCode` provide harnesses for IDE integrations and test runners.

## Build, Test, and Development Commands
- `dotnet restore IndFusion.sln`: restores with central package versions defined in `Directory.Packages.props`.
- `dotnet build IndFusion.sln -c Release`: builds every project with warnings promoted to errors.
- `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release --collect:"XPlat Code Coverage"`: runs analyzer regressions and emits coverage via Microsoft.Testing Platform.
- `pwsh src/scripts/build-offline.ps1 -Configuration Release`: convenience wrapper for restore plus build while priming offline caches.
- `pwsh src/scripts/Generate-XmlDocs.ps1`: regenerates API documentation because XML warnings are unsuppressed.

## Coding Style & Naming Conventions
- `.editorconfig` enforces tabs (size 4), CRLF endings, and trimmed trailing whitespace; verify formatting before committing.
- Stick to C# defaults: PascalCase for types and methods, camelCase for locals, `_camelCase` for private fields, `Async` suffix on asynchronous methods.
- All public or protected members require XML comments; `TreatWarningsAsErrors=true` means missing docs fail the build.
- Run `dotnet format --verify-no-changes` or use the EXXER900 formatting actions, and log any justified suppressions in `SUPPRESSIONS.md`.

## Testing Guidelines
- Tests use xUnit v3 with Shouldly; follow the `Should_Action_WhenCondition` naming already used in `TestCases` folders.
- Place new analyzer scenarios under `src/test/.../TestCases/` and reuse `AnalyzerTestHelper` for Roslyn driver setup.
- Keep coverage from regressing by running tests with `--collect:"XPlat Code Coverage"` (or `/p:CollectCoverage=true`) before submission.
- Document helpers even in tests; CS1591 is enabled for every assembly.

## Commit & Pull Request Guidelines
- Emulate the repository history by using `type(scope): summary` messages (e.g., `docs(analyzers): outline EXXER formatting flow`); add `[ci full]` to trigger the full pipeline when needed.
- Reference related issues and describe behavioural risks, mitigations, and test evidence in every PR.
- Summarise architecture or UX impact, attach screenshots for UI changes, and call out offline compatibility when altering scripts.
