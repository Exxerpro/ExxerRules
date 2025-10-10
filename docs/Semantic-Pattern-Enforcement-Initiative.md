# Initiative: Semantic Pattern Enforcement Platform

## Vision
Deliver a semantic-first “Code Standards as a Service” capability inside the MCP server so autonomous agents can discover, enforce, and repair architectural patterns safely across IndFusion/IndTrace repositories.

## Problem Statements
- Standards drift across repositories because analyzers operate on syntax only and lack cross-project context.
- Agents cannot reuse proven IndTrace patterns during TDD loops without manual digging through documentation.
- Regex/code rewrites risk breaking builds because there is no semantic validation pipeline.
- Fixer001 repairs exist but are not exposed through MCP tooling, limiting automated remediation.

## Business Outcomes
- Reduce pattern violations and architectural regressions across projects.
- Increase autonomous agent throughput by surfacing best practices and safe repair flows.
- Improve developer trust in automated fixes via Roslyn-backed validation.

## Key Metrics (KPIs)
- ≥90 % precision on semantic pattern retrieval with <5 % false positive rate.
- <2 s P95 latency for repo/project semantic search results.
- ≥80 % of targeted EXXER diagnostics have Fixer001 repair flows invokable via MCP.
- >95 % success rate for safe regex replace validations without manual rollback.

## Stakeholders
- Product Engineering (MCP Core, Analyzer, Fixer teams)
- Autonomous Agent Maintainers
- Solution Architects overseeing IndFusion/IndTrace
- QA & DevOps for TDD and pipeline validation

## Constraints & Assumptions
- Initial scope is C#/.NET projects within IndFusion.sln and IndTrace.sln.
- Must respect offline NuGet feeds and current build pipelines.
- No new IDE UI; interactions surface through MCP protocol endpoints.
- Embedding provider must support on-prem or cached execution.

## Non-Goals
- Cross-language semantic support beyond C# in the first release.
- Replacing existing CI/CD pipelines or offline feed management.
- Building bespoke IDE extensions; agents consume MCP responses instead.

## Definition of Done
- All new MCP tools documented, versioned, and covered by automated integration tests.
- Analyzer/fixer enhancements ship with xUnit v3 + Shouldly coverage and AnalyzerTestHelper harnesses.
- Safe regex replace pipeline validates changes via Roslyn parsing and `dotnet build -c Release` (or inspection fallback) before persisting results.
- Knowledge base contains curated IndTrace best-practice exemplars with traceable provenance.

---

## Implemented Patterns and Rules

The following tables summarize the diagnostic analyzers and code fixes that have been implemented as part of this initiative. These components form the core of the semantic pattern enforcement platform.

### EXXER Diagnostics and Fixers

These rules are defined in the `ExxerAI.Rules.Analyzer` and `IndFusion.Fixer` projects.

| Category | Analyzer | Corresponding Code Fix |
| --- | --- | --- |
| **Code Style** | `RuleForRegionUsage` | `DoNotUseRegionsCodeFixProvider` |
| | `RuleForClassFileLength` | - |
| **Asynchronous Programming** | `RuleForConfigureAwaitUsage` | `ConfigureAwaitFalseCodeFixProvider` |
| **Testing** | `RuleForMigrationXUnitV3` | `XUnitV3MigrationCodeFixProvider` |
| **Error Handling** | `RuleForResultCancellationHandling` | - |

<br>

### IndFusion Standard Diagnostics and Fixers

These rules are defined in the `IndFusion.Analyzer` and `IndFusion.Fixer` projects and enforce broader coding standards.

| Category | Analyzer | Corresponding Code Fix |
| --- | --- | --- |
| **Architecture** | `DomainShouldNotReferenceInfrastructureAnalyzer` | `DomainShouldNotReferenceInfrastructureCodeFixProvider` |
| | `UseRepositoryPatternAnalyzer` | `UseRepositoryPatternCodeFixProvider` |
| **Async** | `AsyncMethodsShouldAcceptCancellationTokenAnalyzer` | `CancellationTokenCodeFixProvider` |
| | `AvoidAsyncVoidAnalyzer` | `AvoidAsyncVoidCodeFixProvider` |
| | `UseConfigureAwaitFalseAnalyzer` | `ConfigureAwaitFalseCodeFixProvider` |
| **Code Formatting**| `CodeFormattingAnalyzer` | `CodeFormattingCodeFixProvider` |
| | `ProjectFormattingAnalyzer` | `ProjectFormattingCodeFixProvider` |
| **Code Quality** | `AvoidMagicNumbersAndStringsAnalyzer` | `MagicNumbersAndStringsCodeFixProvider` |
| | `DoNotUseRegionsAnalyzer` | `DoNotUseRegionsCodeFixProvider` |
| **Documentation** | `PublicMembersShouldHaveXmlDocumentationAnalyzer` | `XmlDocumentationCodeFixProvider` |
| **Error Handling**| `AvoidThrowingExceptionsAnalyzer` | - |
| | `UseResultPatternAnalyzer` | `UseResultPatternCodeFixProvider` |
| **Functional Patterns**| `DoNotThrowExceptionsAnalyzer` | - |
| **Logging** | `DoNotUseConsoleWriteLineAnalyzer` | `ConsoleWriteLineCodeFixProvider` |
| | `UseStructuredLoggingAnalyzer` | `StructuredLoggingCodeFixProvider` |
| **Modern C#** | `UseExpressionBodiedMembersAnalyzer` | `ExpressionBodiedMembersCodeFixProvider` |
| | `UseModernPatternMatchingAnalyzer` | `ModernPatternMatchingCodeFixProvider` |
| **Null Safety** | `ValidateNullParametersAnalyzer` | `NullParameterValidationCodeFixProvider` |
| **Performance** | `UseEfficientLinqAnalyzer` | `UseEfficientLinqCodeFixProvider` |
| **Testing** | `DoNotMockDbContextAnalyzer` | `DbContextTestingCodeFixProvider` |
| | `DoNotUseFluentAssertionsAnalyzer` | `ShouldlyAssertionCodeFixProvider` |
| | `DoNotUseMoqAnalyzer` | `NSubstituteMockingCodeFixProvider` |
| | `TestNamingConventionAnalyzer` | `TestNamingConventionCodeFixProvider` |
| | `UseXUnitV3Analyzer` | `XUnitV3MigrationCodeFixProvider` |

---

## Epics & Backlog

### Epic 1 – Semantic Core & Knowledge Base
**Objective:** Enable semantic search and establish the pattern knowledge foundation.

#### Features
1. Semantic embedding integration for repo/project scopes.
2. Pattern knowledge base schema and storage.
3. Best-practice ingestion sourced from IndTrace exemplars.

#### User Stories
- *As an autonomous agent*, I want to query the repository semantically so I can retrieve contextually relevant code snippets within two seconds.
- *As an analyzer maintainer*, I want to store and retrieve pattern definitions so I can evolve diagnostics without code changes.
- *As a solution architect*, I want curated IndTrace best practices exposed via MCP so I can guide agents consistently.

#### Acceptance Criteria
- `semantic_repo_rag` MCP tool returns ranked results with embeddings-backed similarity scores and metadata.
- Knowledge base stored under `docs/patterns/` with JSON schema validation in CI.
- At least three IndTrace exemplars (Specification pattern, Strategy-based test data loader, SignalR DI abstraction) captured with rationale and layer tags.

### Epic 2 – Graph Reasoning & Pattern Guidance
**Objective:** Provide compiler-backed graph queries and actionable pattern suggestions.

#### Features
1. Roslyn-powered semantic graph service with caching.
2. Pattern suggestion engine aggregating analyzer results and knowledge base rules.
3. Pattern extraction tool for reports and targeted files.

#### User Stories
- *As an autonomous agent*, I want to inspect a project’s semantic graph so that I can detect architecture violations before committing changes.
- *As a maintainer*, I want suggested pattern alternatives with confidence scores so that I can prioritise remediation work.
- *As a developer*, I want to extract pattern references from selected reports so that best practices remain discoverable.

#### Acceptance Criteria
- `semantic_graph_query` MCP tool provides symbol nodes, edges, and layer metadata with cache hits logged.
- `pattern_suggest` responses include patternId, confidence, suggested fixes, and linked knowledge base entries.
- `pattern_extract` outputs structured JSON (code sample, description, source path) for targeted reports/files.

### Epic 3 – Safe Transformation Pipeline
**Objective:** Guarantee code modifications are validated before application and expose Fixer001 tools.

#### Features
1. Safe regex replace service using Roslyn workspaces.
2. Compilation/inspection validation flow with dry-run reporting.
3. MCP exposure for Fixer001 repair actions.

#### User Stories
- *As an autonomous agent*, I want a safe regex replace tool that previews diffs and validates builds so that I avoid breaking files.
- *As a developer*, I want Fixer001 diagnostics, such as `UseResultPatternAnalyzer` or `AvoidAsyncVoidAnalyzer`, to surface as MCP repair actions so that I can remediate EXXER issues automatically.
- *As QA*, I need telemetry on fix success and failure modes so that I can monitor platform reliability.

#### Acceptance Criteria
- `safe_regex_replace` requires successful Roslyn parse; if parse fails, returns fallback diagnostics without persisting changes.
- Validation runs `dotnet build -c Release` (or semantic inspection) before finalising replacements.
- `fixer001_apply_*` MCP tools wrap existing code fix providers, returning diff previews and status codes.

### Epic 4 – Hardening, Telemetry, and Rollout
**Objective:** Ensure production readiness, observability, and adoption.

#### Features
1. Telemetry and logging for tool usage and latency.
2. Documentation, onboarding guides, and TDD coverage.
3. Pilot rollout and feedback loop across selected repositories.

#### User Stories
- *As a product owner*, I want dashboards for semantic tool usage so that I can track adoption and ROI.
- *As an agent maintainer*, I want comprehensive docs and sample workflows so that I can integrate features quickly.
- *As a developer*, I want pilot feedback incorporated before general availability so that friction is minimised.

#### Acceptance Criteria
- All MCP responses include requestId, duration, cacheHit flags; telemetry exported to existing logging pipeline.
- Documentation published under `docs/`, including API references, knowledge base editing guide, and agent workflows.
- Pilot across at least two repositories (including IndTrace) with feedback items triaged and resolved.

---

## Release & Sprint Plan (4 two-week sprints)

| Sprint | Focus | Key Deliverables | Exit Criteria |
|--------|-------|------------------|---------------|
| Sprint 1 | Epic 1 foundations | Embedding integration, knowledge base schema, initial IndTrace exemplar ingestion | Semantic search latency <2 s on sample repo; knowledge base validation in CI |
| Sprint 2 | Epic 2 graph & suggestions | Compiler graph service, pattern suggestion/extraction MCP tools | Graph queries cached; pattern suggestions verified against known violations |
| Sprint 3 | Epic 3 safe transformations | Safe regex pipeline, Fixer001 MCP tools, validation harness | Safe replace dry-run + build validation passes on sample changes; fixer tests green |
| Sprint 4 | Epic 4 hardening | Telemetry, docs, pilot rollout, TDD coverage | Telemetry dashboards live; docs published; pilot feedback resolved; coverage goals met |

## Dependencies
- Embedding provider selection and licensing.
- Roslyn analyzer/fixer infrastructure (existing in repo).
- Build agents capable of running Roslyn workspace transformations and `dotnet build`.

## Risks & Mitigations
- **Embedding provider drift** → abstract interface, maintain offline cache.
- **Graph generation performance** → incremental compilation caching per project hash.
- **Safe replace regressions** → enforce mandatory validation and diff preview; provide rollback instructions.
- **Fixer coverage gaps** → prioritise EXXER diagnostics based on incident history; maintain backlog for remaining fixers.

## Open Questions
- Preferred long-term storage for embeddings (in-repo vs. external cache)?
- Which of the remaining diagnostics should be prioritized for new Fixer001 implementations? (see "Implemented Patterns and Rules" for current coverage)
- Do we require multi-repo semantic queries in future sprints?
- What telemetry stack (Serilog, Application Insights, etc.) will consume the new metrics?

## Next Steps
1. Confirm embedding provider and infrastructure readiness.
2. Baseline semantic search prototypes and capture latency metrics.
3. Finalise pilot repositories and stakeholders for Sprint 4 rollout planning.

