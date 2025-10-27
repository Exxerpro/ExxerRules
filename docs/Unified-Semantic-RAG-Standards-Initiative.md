# Unified Semantic RAG Code-Standards Initiative

---

## Vision
Transform the MCP server into a **Semantic RAG Fabric** that:

- Indexes and understands code, patterns, diagnostics, and fixes across every IndFusion-managed repository.
- Exposes linting and semantic enforcement flows as MCP tools with deterministic validation (Roslyn analyzers, Fixer001, safe regex pipeline).
- Supplies agents with contextual “how + why” guidance sourced from a curated knowledge base, vector search, and a pattern graph—complete with provenance and confidence scores.
- Provides closed-loop remediation by combining analyzers/fixers, semantic matching, RAG knowledge, and validation pipelines.

---

## Programme Pillars

1. **RAG Knowledge Fabric**
   - Multi-repo ingestion (code, diagnostics, docs) with chunking + embedding.
   - Vector search + hybrid keyword filtering; reranking via cross-encoder.
   - Graph RAG overlay (symbol graph, dependency edges, pattern relationships, diagnostics-to-fix edges).
   - Knowledge base curation with verified exemplars, anti-patterns, playbooks, and Fixer walkthroughs.

2. **Semantic Pattern Enforcement Services**
   - Semantic search for violations, best-practice retrieval, and cross-repo comparisons.
   - Pattern graph queries (semantic_graph_query, pattern_suggest, pattern_extract).
   - Safe transformation pipeline (Roslyn workspace validation, Fixer001 invocation, `safe_regex_replace` dry-run on temp workspaces with analyzer/build validation and optional failure heuristics).

3. **Agentic Linting & Governance**
   - Real-time linting MCP tools that surface EXXER analyzers/fixers, policy decisions, and auto-fixes (EXXER001–EXXER901).
   - Agent-aware rules (“watcher” mode) with streaming notifications, dashboards, and historical audits.
   - Quality gates and metrics integrated into CI/CD, dashboards, and telemetry pipelines.

4. **Cross-Repository Coverage & Insights**
   - Normalised ingestion pipeline for IndFusion.sln, IndTrace.sln, and satellite repos.
   - Multi-repo pattern consistency, drift detection, and remediation prioritisation.
   - Telemetry-driven ROI tracking (adoption, violation reduction, fix success rate, latency).

---

## Target Outcomes & KPIs

| Category | Goal |
| --- | --- |
| **Retrieval Quality** | ≥93 % semantic retrieval precision with <5 % false positives (text + graph). |
| **Latency** | <2 s P95 response for MCP RAG queries (semantic_search, graph_query, linting). |
| **Linting Compliance** | ≥95 % pass rate on EXXER “error” rules across repos; 80 % on warnings. |
| **Fix Automation** | ≥80 % of Fixer001 scenarios executable through MCP tools with successful validation. |
| **Agent Adoption** | ≥70 % of autonomous agent flows invoke MCP RAG services for decisions/remediation. |

---

## Delivery Framework

### Epics & Stories

| Epic | Key Outcomes | Code Anchors | Representative Stories |
| --- | --- | --- | --- |
| **E1: Semantic RAG Fabric Foundations** | MCP server indexes and retrieves standards knowledge with provenance. | `src/code/IndFusion.Mcp.Server/`, `src/code/IndFusion.Mcp.Core/Knowledge` | - As a platform engineer, I can schedule repository ingestion so that `IngestionPipelineJob` produces vector + graph payloads.<br>- As an autonomous agent, I can call `knowledge_rag` and receive snippets with `sourcePath` + commit hash metadata. |
| **E2: Pattern Enforcement Surface** | Deterministic analyzers/fixers exposed as MCP tools with safe apply flows. | `src/ExxerRules/ExxerAI.Rules.Analyzer/`, `src/code/IndFusion.Mcp.Tools/Fixer001` | - As an analyzer maintainer, I can register a new EXXER rule and have it appear in the MCP `lint_run` manifest within one CI cycle.<br>- As an agent, I can dry-run `fixer001_apply_cs` and receive build + analyzer validation output. |
| **E3: Agentic Governance & Telemetry** | Policy-aware linting, dashboards, and audit trails across repos. | `src/code/IndFusion.Mcp.AgentGovernance/`, `src/code/IndFusion.Telemetry/` | - As a DevEx lead, I can subscribe to EXXER error regressions through the governance feed.<br>- As an agent, I can query `policy_decision_trace` and view the rule, context, and remediation guidance. |
| **E4: Cross-Repository Insights & Drift Control** | Consistency checks, drift detection, and prioritised remediation backlog. | `src/code/IndFusion.Mcp.Analytics/`, `src/test/IndFusion.Analyzer.Tests/TestCases/` | - As a standards steward, I get a ranked backlog of drifted repositories with supporting metrics.<br>- As a team, we can generate remediation tasks with links to offending files and EXXER diagnostics. |

### Milestones & Timeline

| Milestone | Target Sprint | Exit Criteria | Dependencies |
| --- | --- | --- | --- |
| **M1: Charter Ratification & Kickoff** | Sprint 0 | Charter approved, backlog seeded in Azure Boards, kickoff deck signed off. | Cross-team buy-in (Analyzer, MCP, DevEx). |
| **M2: Ingestion MVP (E1)** | Sprint 2 | `IngestionPipelineJob` ingests IndFusion.sln nightly, vector + graph stores deployed with sample queries. | Build pipeline access, embedding model availability. |
| **M3: MCP Tooling Surface (E2)** | Sprint 4 | `lint_run`, `pattern_suggest`, `fixer001_apply_*` exposed through MCP with integration tests. | M2 complete, analyzer metadata contract versioned. |
| **M4: Governance Telemetry (E3)** | Sprint 6 | Telemetry dashboards showing EXXER compliance, policy gates active in CI (`dotnet build` + analyzer enforcement). | M2 + M3, telemetry pipeline capacity. |
| **M5: Drift & Remediation Insights (E4)** | Sprint 8 | Drift detection jobs producing backlog items; remediation SLA tracking live. | Knowledge graph enriched with repo metadata, governance APIs stable. |
| **M6: End-to-End Pilot Release** | Sprint 9 | Pilot repos (IndFusion + IndTrace) passing Definition of Done, due diligence sign-offs complete. | All epics feature-complete, pilot feedback loop staffed. |

### Guardrails & Governance

- **Code Traceability**: Every automated remediation must link to analyzer ID, fixer version, and source commit (`docs/automation/changelog.md`).
- **Safety Tiers**: Classify automation scripts (informational, dry-run, apply) before exposing through `script_bridge`.
- **Schema Compatibility**: Knowledge graph schema changes gated behind feature flags and contract tests (`GraphSchemaContractTests`).
- **Offline First**: All RAG components must operate against `artifacts/nuget/offline/` and cached embedding models; network calls flagged in CI.
- **Change Control**: Pattern or rule changes require architecture guild approval recorded in `docs/standards/ChangeLog.md`.

### Deliverables & Artifacts

| Deliverable | Description | Owner | Storage |
| --- | --- | --- | --- |
| **Architecture Decision Records** | ADRs covering ingestion, vector store, graph schema, MCP tool contracts. | Platform Architecture | `docs/adr/` |
| **Analyzer & Fixer Catalog** | Living index of EXXER rules, severities, MCP tool bindings. | Analyzer Guild | `docs/reference/ExxerRuleMatrix.md` |
| **Automation Playbooks** | Step-by-step remediation guides with code samples. | Agent Ops | `docs/playbooks/` |
| **Telemetry Dashboards** | PowerBI dashboards tracking KPIs (latency, compliance, adoption). | DevEx Insights | `docs/dashboards/README.md` + PowerBI workspace |
| **Test Case Repository** | Curated IITDD scenarios and golden files. | QA Chapter | `src/test/IndFusion.Analyzer.Tests/TestCases/UnifiedSemanticRag/` |

### Tracking & Reporting

- **Backlog**: Markdown-based project management in `docs/project-management/` with epic and story tracking files.
- **Standups**: Twice-weekly cross-team sync; blockers tracked in shared Teams channel `#semantic-rag`.
- **Burndown & Velocity**: Sprint progress tracked in markdown files with automated validation scripts.
- **Compliance Dashboards**: CI pipeline posts EXXER pass/fail metrics and RAG latency to `GovernanceTelemetry` stream.
- **Retrospectives**: End-of-sprint retro must review guardrail breaches, due-diligence outcomes, and test coverage deltas.

### Agent Development Operating Model

- **Roles**
  - *Tech Lead*: curates backlog, owns Definition of Done interpretation, reviews agent output daily.
  - *Agent Supervisor*: assigns work packages, monitors telemetry, runs validation scripts before human review.
  - *Dev Agent*: executes work package scripts, updates trace log, proposes patches via MCP tooling only.
  - *QA Partner*: owns regression validation and IITDD upkeep.
- **Supervision Cadence**
  - Daily 15-minute standup focused on status, blockers, telemetry anomalies.
  - Mid-sprint alignment review ensuring agents still operate within updated guardrails.
  - End-of-day supervisor checkpoint validating each agent submission against acceptance checklist.
- **Context Refresh**
  - Maintain `docs/reference/SemanticRag-Agent-Brief.md` as the authoritative context packet (digest generated by `src/scripts/Update-Agent-Brief.ps1`).
  - Agents must sync the brief before each work session; supervisors confirm via `docs/operations/AgentSyncLog.csv`.
  - Any analyzer/fixer contract change triggers immediate briefing update and broadcast in `#semantic-rag`.

### Agent Work Package Template

Every delegated unit of work must include (use `docs/project-management/templates/AgentWorkPackage.md`):

1. **Objective** – single sentence referencing markdown work item.
2. **Code Surface** – explicit paths/projects (e.g., `src/ExxerRules/...`).
3. **Guardrails** – required analyzers, forbidden APIs, dependency rules.
4. **Inputs** – links to knowledge base entries, ADRs, existing tests.
5. **Expected Outputs** – code artifacts, test updates, documentation edits.
6. **Verification Steps** – commands/tests the agent must run with expected outcomes.
7. **Telemetry Hooks** – metrics or logs to emit (e.g., `SemanticRag.Metrics.WorkItemId`).

Template stored at `docs/project-management/templates/AgentWorkPackage.md` and referenced in markdown tracking files.

### Agent Execution Loop

1. Supervisor assigns work package and logs assignment in `docs/operations/AgentAssignmentRegister.csv`.
2. Agent refreshes context packet, acknowledges guardrails, and records checksum in `docs/operations/AgentSyncLog.csv`.
3. Agent executes work package, capturing each command in `agent-trace/<workItemId>.log`.
4. Agent runs verification steps verbatim; updates markdown tracking files with evidence.
5. Supervisor reviews logs, reruns spot-check commands, and initiates human code review.
6. QA partner triggers regression suite or targeted test scenarios before merge.

### Validation & Review Gates

- **Pre-Commit**: `dotnet format --verify-no-changes`, analyzer suite, performance smoke (`Measure-RagLatency.ps1` sample run).
- **Pre-PR**: Supervisor validates telemetry fields, ensures documentation updates committed, confirms no guardrail violations via `src/scripts/GuardrailCheck.ps1`.
- **PR Review**: Tech Lead confirms alignment with epics/stories, verifies traceability links, checks markdown tracking file updates.
- **Post-Merge**: QA partner ensures pipeline green, archives agent artifacts, updates knowledge base with learnings.

### Context Refresh Mechanisms

- Nightly job `src/scripts/Update-Agent-Brief.ps1` aggregates latest ADRs, rule changes, telemetry trends.
- Supervisors receive digest summarising deltas; agents must sign off within 12 hours.
- Context packet includes quick links to the EXXER rule matrix, open due diligence findings, guardrail overrides, and the false-positive catalog.

### Reporting & Escalation

- **Daily Report**: Supervisor posts summary in `#semantic-rag` with completed work items, pending reviews, guardrail exceptions (attach digest from `docs/operations/due-diligence/` when relevant).
- **Escalation Triggers**:
  - Agent output failing verification twice in a sprint.
  - Telemetry anomaly >10% deviation from latency/compliance targets.
  - Knowledge graph schema change without corresponding contract tests.
  - Due diligence gate validation failures.
- **Escalation Path**: Supervisor > Tech Lead > Programme Steering (weekly meeting).

### Quality Engineering Strategy

- **Unit Tests**: All new analyzers/fixers require xUnit v3 tests in `IndFusion.Analyzer.Tests`; use `AnalyzerTestHelper` and Shouldly assertions.
- **Integration Tests**: Add MCP end-to-end tests under `src/test/IndFusion.Mcp.Tests/` covering `lint_run`, `pattern_suggest`, and `fixer001_apply_*` flows with stub repositories.
- **Regression Suites**: Nightly pipeline executes `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release --collect:"XPlat Code Coverage"` and publishes coverage trend.
- **Contract Tests**: Schema/version compatibility validated via `GraphSchemaContractTests` and `ToolManifestContractTests`.
- **Performance Harness**: Maintain scripted load tests for RAG queries (P95 latency target) using `src/scripts/Measure-RagLatency.ps1`.
- **Build Gates**: `dotnet build IndFusion.sln -c Release` with warnings as errors + `dotnet format --verify-no-changes` enforced in CI.

### Code Style & Standards Alignment

- Adhere to `.editorconfig` (tabs, CRLF, no trailing whitespace); include analyzer IDs in suppression justifications (`SUPPRESSIONS.md`).
- Document all public/protected APIs with XML comments; failing CS1591 blocks merges.
- Ensure new MCP tools expose deterministic JSON contracts documented under `docs/reference/mcp/`.
- Prefer deterministic identifiers (`RuleIds.SemanticRag###`) and maintain central registration in `SemanticRagRuleRegistry.cs`.

### Definition of Done

- Story acceptance criteria met with automated verification (unit + integration tests covering the change).
- No analyzer regressions; EXXER rule suite passes locally and in CI.
- Documentation updated (playbooks, reference pages, changelog) with links to commits and tooling outputs.
- Telemetry/metrics hooks implemented and validated in lower environments.
- Security/offline review complete; dependencies resolved from offline feeds.
- All code reviewed, approvals recorded, and work item moved to `Done` with evidence (test logs, dashboard screenshots).

### Kickoff Preparation Checklist

- Charter and scope validated with stakeholders; agenda distributed 3 days prior.
- Programme backlog initialised with epics E1–E4 and seed stories.
- Environment baselining: confirm access to IndFusion/IndTrace repos, offline feeds, embedding models.
- Tooling readiness: ensure build/test pipelines green on current main branch; create pilot branch protection rules.
- Communication plan drafted (cadence, channels, decision log templates).

### Due Diligence Gates

#### Initiation Gate (Pre-Development)

- Verify problem statement, constraints, and KPIs signed off.
- Confirm legal/compliance approval for data ingestion scope.
- Run `src/scripts/Validate-PreDevelopmentGate.ps1` to validate project management setup and tracking artifacts (outputs stored in `docs/operations/due-diligence/`).
- Assess resource allocation (SME coverage for analyzers, MCP, DevEx).
- Capture baseline metrics (current EXXER compliance, RAG latency, agent adoption).

#### Closeout Gate (Post-Delivery)

- Execute `src/scripts/Validate-PostDeliveryGate.ps1` to validate completion of all epics, stories, and tracking artifacts (outputs stored in `docs/operations/due-diligence/`).
- Validate Definition of Done artifacts: test reports, documentation updates, ADR decisions.
- Confirm operational ownership: runbooks handed to DevEx, alerting configured, training completed.
- Conduct final risk review, document residual debt, and schedule follow-up audits.
- Publish programme retrospective with lessons learned and improvement backlog items.

---

## Unified Architecture

```
┌─────────────────────────────┐
│   Repositories (IndFusion,  │
│   IndTrace, satellite repos)│
└──────────────┬──────────────┘
               │ Git hooks / sched jobs
┌──────────────▼──────────────┐
│ Ingestion Pipeline           │
│  • AST/Roslyn extraction     │
│  • Documentation/ADR parser  │
│  • Analyzer diagnostics      │
│  • Fixer debug traces        │
└──────────────┬──────────────┘
               │ Normalised chunks + metadata
┌──────────────▼──────────────────────────────────────────────────────┐
│ Semantic RAG Fabric                                                 │
│  • Vector store (embeddings, metadata)                              │
│  • Knowledge graph (symbols, patterns, diagnostics, fixes, repos)   │
│  • Hybrid retrieval (BM25 + vector + reranker)                      │
│  • Provenance store (source file, commit, confidence)               │
└──────────────┬──────────────────────────────┬───────────────────────┘
               │                              │
┌──────────────▼────────────┐      ┌──────────▼───────────┐
│ MCP Semantic Tools        │      │ Linting & Fixer APIs │
│  • semantic_search        │      │  • lint_run          │
│  • pattern_graph_query    │      │  • fixer001_apply_*  │
│  • pattern_suggest        │      │  • safe_regex_replace│
│  • knowledge_rag          │      │  • lint_watch        │
└──────────────┬────────────┘      └──────────┬───────────┘
               │                               │
┌──────────────▼────────────────────────────────▼────────────────────┐
│ Consumers                                                           │
│  • Autonomous agents (Cursor, MCP CLI, dashboards)                  │
│  • Engineers (RAG dashboards, MCP Web, IDE integrations)            │
│  • CI/CD & pipelines (policy gating, auto-remediation)              │
└─────────────────────────────────────────────────────────────────────┘
```

### RAG Enhancements
- **Text RAG**: chunk-level embeddings with repository/project metadata, commit hash, analyzer/fixer references.
- **Graph RAG**: node/edge embeddings (projects, namespaces, classes, EXXER diagnostics, MCP tools); query-time fusion via graph traversal + vector search.
- **Hybrid Retrieval**: BM25 keyword search + vector similarity + cross-encoder reranking for high-precision hits.
- **Query Planning**: Select best retrieval mode (vector, graph, or hybrid) based on question type and agent hints.
- **Grounded Responses**: MCP tools return contextual snippets, diff previews, diagnostics, and fix instructions referencing actual files and commits.

---

## Capability Matrix

| Capability | Description | Stats/Policy Inputs | RAG Utilisation |
| --- | --- | --- | --- |
| **semantic_pattern_analysis** | Analyse code/project for alignment with pattern families | Project path, pattern type | Vector+graph retrieval of relevant patterns, known violations, curated fixes |
| **pattern_graph_query** | Query pattern knowledge graph (e.g., dependencies, violation clusters) | Query filters, repo scope | Graph embeddings + vector expansion with adjacency context |
| **pattern_suggest** | Suggest remediation strategies with confidence and citations | Violation metadata, context | Retrieve best-practice exemplars, analyzer docs, fix code, commit history |
| **knowledge_rag** | General RAG endpoint for docs, ADRs, linting rules | Natural language query | Vector search + reranker, with follow-up graph expansion |
| **lint_run / lint_watch** | Run EXXER analyzers, stream violations, apply policy | File/project scope, severity config | Connect analyzer results with knowledge base for each rule |
| **fixer001_apply_* / safe_regex_replace** | Deterministic code repair with validation | Diagnostic ID, target files, regex pattern | Stage changes on temp workspace, run analyzers/build, surface diffs (with optional failure heuristics) before committing |
| **semantic_change_review** | Compare revisions, highlight semantic drift, suggest fixes | Original vs. modified code | Semantic diff (embeddings), lint diff, pattern matches |
| **cross_repo_consistency** | Report drift or pattern adoption variance across repos | Pattern ID, time window | Multi-repo graph traversal + aggregated stats via RAG |
| **dependency_api_lookup** | Return API surfaces, usage examples, and syntactic changes for referenced packages | Package id, version, target framework | Embed package docs/release notes, build graph linking repos → package versions → API symbols; RAG returns version-correct syntax and breaking-change notes |
| **script_bridge / script_catalog** | Discover and execute vetted Python/PowerShell automation with personalised parameters | Script identifier, arguments, safety policy | Script metadata + usage guides embedded; MCP wrapper orchestrates dry-run/validation and returns contextual guidance |
| **document_processing** | Process documents using Tesseract OCR and Ollama LLM services | Document input, processing options | OCR, entity extraction, relationship mapping through hexagonal architecture adapters |
| **vector_search** | Search vector embeddings using Qdrant with semantic similarity | Query text, search options | Vector similarity search with metadata filtering and confidence scoring |
| **knowledge_graph_query** | Query Neo4j knowledge graph for relationships and patterns | Graph query, node/edge filters | Graph traversal with relationship analysis and pattern detection |

---

## Implementation Status

### ✅ **COMPLETED: Foundation Architecture (Sprint 0)**
**Status**: Architecture foundation implemented with critical issues identified  
**Completion Date**: January 2025

| Component | Status | Implementation Quality | Key Achievements |
| --- | --- | --- | --- |
| **Domain Layer** | ✅ Complete | High | Pure domain with custom `Result<T>`, no external dependencies |
| **Application Layer** | ✅ Complete | High | CQRS with MediatR, FluentValidation, structured logging |
| **Infrastructure Layer** | ⚠️ Partial | Medium | Repository pattern with hexagonal architecture, placeholder implementations |
| **Web API Layer** | ⚠️ Partial | Medium | ASP.NET Core with clean controller design, build issues |
| **Unit Tests** | ❌ Blocked | Low | Tests exist but cannot execute due to build failures |
| **Integration Tests** | ❌ Blocked | Low | Tests skipped due to infrastructure issues |

**Architecture Highlights**:
- ✅ Hexagonal Architecture (Ports & Adapters) - Well implemented
- ✅ Functional Programming with `Result<T>` pattern - Excellent implementation
- ✅ Modern C# patterns (Records, Expression-bodied members) - Properly applied
- ✅ Clean Architecture & SOLID principles - Correctly structured
- ❌ TDD/ITDD methodology - Blocked by build issues
- ✅ Comprehensive error handling without exceptions - Well implemented

### ❌ **CRITICAL ISSUES IDENTIFIED**

#### Build System Failures
- **Central Package Management**: Missing package versions causing restore failures
- **Dependency Resolution**: Multiple projects cannot build due to package conflicts
- **Test Execution**: Cannot run tests due to build infrastructure problems

#### RAG Implementation Status
- **All RAG Services**: Contain placeholder implementations with `TODO` comments
- **Vector Search**: Qdrant integration not implemented (placeholder only)
- **Knowledge Graph**: Neo4j integration not implemented (placeholder only)
- **LLM Integration**: Ollama integration not implemented (placeholder only)
- **Core Functionality**: No actual RAG capabilities functional

#### Test Infrastructure Issues
- **Test Coverage Claims**: Cannot be verified due to build failures
- **Skipped Tests**: Multiple tests marked as `[Fact(Skip = "Hanging")]`
- **Integration Tests**: Cannot execute due to infrastructure problems

### 📋 **CURRENT PHASE: Critical Infrastructure Fixes Required**
**Target**: Fix build system, implement actual RAG functionality, enable test execution

---

## Implementation Plan (Updated - Blocked Sprint Timeline)

| Sprint | Focus | Key Deliverables | Exit Criteria |
| --- | --- | --- | --- |
| 1 | **Fabric Foundations** | Repo ingestion pipeline, vector store, knowledge base schema, MCP text RAG endpoint (`knowledge_rag`) | `knowledge_rag` <2 s P95; baseline embeddings validated on sample queries |
| 2 | **Linting Convergence** | Central policy config, unified lint MCP tools, real-time watcher integration, metrics capture | `lint_run` + `lint_watch` produce actionable output across IndFusion.sln; dashboard MVP |
| 3 | **Graph RAG Layer** | Symbol/pattern graph builder, caching, `pattern_graph_query`, `pattern_suggest` | Graph queries cached per project hash; suggestions include confidence + provenance |
| 4 | **Safe Transformation Pipeline** | `safe_regex_replace`, Fixer001 MCP wrappers, build validation harness; ExxerAI interface/IITDD adoption blueprint | Dry-run validation + `dotnet build -c Release` or semantic fallback executed before apply; documentation of borrowed contracts approved |
| 5 | **Multi-Repo Expansion** | Integrate IndTrace + satellite repos, cross-repo analytics, drift detection tooling | Multi-repo reports operational; drift alerts with recommended actions |
| 6 | **Hardening & Autonomy** | Telemetry dashboards, agent cookbook, TDD coverage, resilience testing | Telemetry exported, docs published, agent pilot adoption ≥70 %, tests green |

  ---

## Hexagonal Architecture & IITDD Integration Strategy

**See detailed architecture document**: [`docs/architecture/Hexagonal-Architecture-IITDD-Strategy.md`](docs/architecture/Hexagonal-Architecture-IITDD-Strategy.md)

This section provides a comprehensive overview of the hexagonal architecture approach with IITDD (Integration Interface Test-Driven Development) that will be used for the IndFusion Semantic RAG platform.

## Borrowed Interface & IITDD Integration Strategy

To align with the ExxerAI initiative without blocking their delivery milestones, we will **document first, implement when approved**. No code is copied at this stage—only reference material and plans.

### Immediate Documentation Tasks
1. **Interface Snapshot (`docs/reference/ExxerAI-Interface-Snapshots.md`)**
   - Capture namespaces, method signatures, and behavioural contracts for:
     - `IDocumentIngestionService`, `IDocumentMetadataRepository`, `IDocumentTruthSystem`
     - `IDocumentProcessingPipeline`, `IOCRService`, `ILLMExtractionService`
     - `IStorageProvider`, `ISecretsVault`, `IQueueDispatcher`
2. **IITDD Harness Plan (`docs/reference/IITDD-Harness-Plan.md`)**
   - Describe fixture layout, data builders, dependency expectations, and how IITDD suites will be staged here.
3. **Adoption Guardrails**
   - Document approval gates, version sync expectations, rollback procedure, and compatibility testing strategy.

### Planned Repository Layout (documentation only)
```
/src
  /Shared
    /ExxerAI.Contracts       # placeholder for future interface imports
    /ExxerAI.IITDD           # placeholder for IITDD harness port
  /IndFusion.Mcp.Core
  /IndFusion.Mcp.Server
  /IndFusion.Mcp.Web
  /IndFusion.Automation
  /IndFusion.Rag

/docs
  /reference
    ExxerAI-Interface-Snapshots.md
    IITDD-Harness-Plan.md
```

### Sequenced Adoption Plan
1. **Documentation Capture (Sprint 1)** – complete snapshots/plan; review with ExxerAI maintainers.
2. **Abstraction Stubs (Sprint 2)** – define interface placeholders behind feature flags if early alignment is required (no concrete implementations).
3. **Harness Dry-Run (Sprint 3)** – design IITDD-compatible test scaffolding referencing stubs only.
4. **Code Import Gate (Post‑Epic 7)** – once ExxerAI signs off, replace stubs with real interfaces/tests and run shared IITDD suite.
5. **Ongoing Sync** – add version tracking in documentation and a CI guard to detect divergence.

  ---

## Data & Knowledge Management

- **Embeddings**: Support on-prem models (e.g., text-embedding-3-large cached offline) with periodic refresh (full weekly, incremental nightly).
- **Vector Store**: Use Qdrant for vector storage with partitioning by repo; include metadata (path, project, analyzer ID, fixer ID, commit, rule severity, package id/version, target framework).
- **Graph Storage**: Use Neo4j for knowledge graph with incremental updates keyed by project hash; add nodes/edges for NuGet packages, versions, API symbols, and consuming repos/files.
- **Document Processing**: Use Tesseract OCR (FOSS) for document text extraction and analysis.
- **LLM Integration**: Use Ollama with open-weight models (e.g., Llama 2, Mistral, CodeLlama) for entity extraction, relationship mapping, and semantic analysis.
- **Automation Catalog**: Index approved Python/PowerShell scripts with purpose, parameters, required tools, safety tier, and sample invocations; embed guides so agents can query usage and execution instructions.
- **Provenance**: Every retrieval includes source file, line, commit, analyzer/fixer references for traceability.
- **Curation Workflow**: Knowledge base updates require human approval; incorporate reviewer metadata for trust scores.
- **Architecture**: All external services accessed through hexagonal architecture ports and adapters with IITDD testing.

---

## Agent Integration Patterns

1. **Agent Lint Loop**
   1. Call `lint_run` with diff scope.
   2. For each violation, call `pattern_suggest` to retrieve best-practice snippet + instructions.
   3. Resolve dependency-specific syntax via `dependency_api_lookup` when fixes depend on package version semantics.
   4. Retrieve recommended automation from `script_catalog` and, where appropriate, invoke it through `script_bridge` with user-specific parameters and dry-run validation.
   5. Decide to apply fix via `fixer001_apply_*` or manual patch; validate with `safe_regex_replace` if needed.

2. **Semantic Guidance Loop**
   1. Call `semantic_pattern_analysis` or `knowledge_rag` with natural-language query.
   2. Use surfaced exemplars and graph metadata to plan changes.
   3. When API usage changes involve external packages, invoke `dependency_api_lookup` to confirm correct syntax for the current version.
   4. Consult `script_catalog` for relevant automation (formatters, housekeeping, migration scripts) and invoke via `script_bridge` when safe.
   5. After edits, call `semantic_change_review` + `lint_run` to ensure compliance.

3. **Cross-Repo Auditing**
   1. Schedule `cross_repo_consistency` to detect drift.
   2. Trigger notifications/dashboards and open follow-up tasks automatically.

---

## Governance & Compliance

- **Policy Management**: Central .editorconfig + MCP policy endpoints support repo overrides but track deviations.
- **Observability**: Telemetry fields include requestId, repo, transport, latency, cacheHit, retrievalConfidence.
- **Security & Offline**: Respect offline NuGet feeds, on-prem model hosting, no external network dependencies by default.
- **Testing**: xUnit v3 + Shouldly for analyzers/fixers, integration harness for MCP tools, regression snapshots for RAG retrieval.
- **Change Management**: RAG knowledge updates versioned; pattern changes require signoff from architecture guild.

---

## Risks & Mitigations

| Risk | Mitigation |
| --- | --- |
| Embedding or graph drift | Maintain hash-based invalidation, scheduled re-embeddings, test queries. |
| Latency spikes | Pre-compute caches, use async pipeline, degrade gracefully to keyword fallback. |
| False positives in retrieval | Use rerankers, human-in-loop feedback, telemetry-driven tuning. |
| Fixer regressions | Mandatory Roslyn parse/build validation and diff previews before apply. |
| Adoption friction | Provide agent cookbooks, dashboards, interactive tutorials, pilot feedback loops. |

---

## Next Actions

1. **CRITICAL: Fix Build System** - Resolve Central Package Management issues and missing package versions
2. **CRITICAL: Implement Actual RAG Functionality** - Replace all placeholder implementations with real Qdrant, Neo4j, and Ollama integration
3. **CRITICAL: Fix Test Infrastructure** - Resolve test hanging issues and enable test execution
4. **Update Documentation** - Correct misleading claims about implementation status and test coverage
5. **Reassess Sprint 2 Readiness** - Project cannot proceed without completing critical infrastructure fixes
6. **Provide Realistic Timeline** - Estimate 5-8 weeks additional work before Sprint 2 can begin

**Note**: The current implementation status does not support the documented claims. Significant additional work is required before the project can proceed to Sprint 2.

---

## Execution Backlog & Ordered Histories

The programme progresses through the following ordered histories. Each history follows the **Agentic Execution Framework** to prevent developers from going off-rails by ensuring:

1. **Code-First Validation**: Every step verifies against existing code
2. **Pattern Recognition**: Identifies existing patterns before creating new ones
3. **Incremental Verification**: Stops and validates at each checkpoint
4. **Real-World Constraints**: Grounds all decisions in actual codebase capabilities

**See detailed framework**: [`docs/architecture/Agentic-Execution-Framework.md`](docs/architecture/Agentic-Execution-Framework.md)

Each history is self-contained, references the required code surface, and is written so that an agent or human teammate can execute it with minimal supervision. Move to the next history only when the exit criteria are satisfied.

### History 1: Charter, Access, and Operating Foundations
- **Context**: Establish governance, access, and due-diligence instrumentation so delivery can proceed safely.
- **Key Tasks**
  - Finalise charter sign-offs and capture stakeholder approvals in `docs/operations/governance/CharterApprovals.md`.
  - Populate `docs/operations/AgentAssignmentRegister.csv` and `docs/operations/AgentSyncLog.csv` with real programme owners; verify `src/scripts/Update-Agent-Brief.ps1 -CheckOnly`.
  - Run `src/scripts/Validate-PreDevelopmentGate.ps1` to validate project management setup and record baseline environment state in `docs/operations/due-diligence/`.
  - Confirm build health: `dotnet restore IndFusion.sln`, `dotnet build IndFusion.sln -c Release`, `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release`.
- **Exit Criteria**
  - Pre-development gate validation passed with all tracking artifacts created.
  - Agent brief digest current; guardrail scripts pass with clean working tree.
  - Communication cadence confirmed in Teams `#semantic-rag`.

### ⚠️ History 2: Foundation Architecture Implementation (PARTIAL)
- **Context**: Implement clean architecture foundation with hexagonal patterns, CQRS, and comprehensive testing
- **Status**: ⚠️ **PARTIAL** - Architecture foundation implemented but critical issues identified
- **Achievements**: 
  - ✅ Domain layer with pure C# and custom `Result<T>` type
  - ✅ Application layer with CQRS, MediatR, and FluentValidation
  - ⚠️ Infrastructure layer with repository pattern and hexagonal architecture (placeholder implementations)
  - ⚠️ Web API layer with ASP.NET Core and clean controllers (build issues)
  - ❌ Unit tests exist but cannot execute due to build failures
  - ❌ Integration tests skipped due to infrastructure problems
  - ❌ Build system has critical package management issues

### History 3: Critical Infrastructure Fixes (Epic E1 - BLOCKED)
- **Context**: Fix build system failures and implement actual RAG functionality
- **Current State**: Foundation architecture implemented but non-functional due to critical issues
- **Target State**: Working build system with actual RAG capabilities

#### **CRITICAL BLOCKERS IDENTIFIED**

**Build System Issues**:
- Central Package Management configuration errors
- Missing package versions in `Directory.Packages.props`
- Multiple projects cannot restore dependencies
- Solution fails to build completely

**RAG Implementation Status**:
- All services contain placeholder implementations with `TODO` comments
- No actual Qdrant, Neo4j, or Ollama integration
- Core RAG functionality is not implemented
- Services return empty results or mock data

**Test Infrastructure Problems**:
- Tests cannot execute due to build failures
- Multiple tests skipped due to "hanging" issues
- No actual test coverage measurement possible
- Integration tests blocked by infrastructure problems

#### **REQUIRED ACTIONS BEFORE PROCEEDING**

1. **Fix Build System** (Estimated: 1-2 weeks)
   - Resolve Central Package Management issues
   - Add missing package versions to `Directory.Packages.props`
   - Ensure all projects can build successfully
   - Fix dependency resolution conflicts

2. **Implement Actual RAG Functionality** (Estimated: 3-4 weeks)
   - Replace all placeholder implementations with real functionality
   - Implement actual Qdrant vector search integration
   - Implement actual Neo4j knowledge graph operations
   - Implement actual Ollama LLM integration
   - Remove all `TODO` comments and placeholder code

3. **Fix Test Infrastructure** (Estimated: 1-2 weeks)
   - Resolve test hanging issues
   - Enable skipped tests
   - Implement proper test isolation
   - Measure actual test coverage

**Total Estimated Additional Work**: 5-8 weeks before Sprint 2 can begin

#### **EXPLORE Phase - Code Discovery**
```bash
# Mandatory exploration commands
find src/ -name "*.cs" -exec grep -l "Service" {} \; | head -20
grep -r "interface.*Service" src/ | head -20
grep -r "class.*Adapter" src/ | head -20
codebase_search "How are services registered in the current codebase?"
codebase_search "What are the existing configuration patterns?"
```

**Deliverables**:
- [ ] Code exploration report in `docs/execution/History2-Exploration.md`
- [ ] Existing service pattern analysis
- [ ] Configuration pattern analysis
- [ ] Adapter pattern analysis

#### **ANALYZE Phase - Pattern Analysis**
```bash
# Analyze existing patterns
grep -r "AddScoped.*Service" src/
grep -r "IOptions" src/ | head -10
grep -r "Configuration" src/ | head -10
codebase_search "What are the existing service registration patterns?"
```

**Deliverables**:
- [ ] Pattern analysis report
- [ ] Convention documentation
- [ ] Implementation strategy

#### **IMPLEMENT Phase - Pattern-Following Implementation**
**Checkpoints**:
- [ ] **Checkpoint 1**: Verify service registration follows existing patterns
- [ ] **Checkpoint 2**: Verify configuration follows existing patterns
- [ ] **Checkpoint 3**: Verify adapters follow existing patterns
- [ ] **Checkpoint 4**: Verify tests follow existing patterns

**Plan of Action**
1. Layout & configuration: repo manifest (`docs/operations/ingestion/RepoManifest.json`), Qdrant/Neo4j/Ollama settings in `appsettings.SemanticRag.json`.
2. Domain ports: ingestion/vector/graph interfaces and entities under `src/code/IndFusion.Mcp.Core/Knowledge`.
3. Adapters: Qdrant, Neo4j, Ollama clients in `src/code/IndFusion.Mcp.Infrastructure/`.
4. Application services: Roslyn ingestion orchestrators coordinating embeddings, graph updates.
5. API/DI wiring: expose `knowledge_rag`, register adapters in server composition root.
6. Testing: contract/integration suites (`GraphSchemaContractTests`, `VectorStoreSmokeTests`, `IngestionLedgerTests`).
7. Automation: `Sync-KnowledgeFabric.ps1` runbook.

#### **VERIFY Phase - Real-World Validation**
```bash
# Mandatory verification commands
dotnet build IndFusion.sln -c Release
dotnet test src/test/IndFusion.Analyzer.Tests/ -c Release
./src/scripts/Validate-Implementation.ps1 -History 2
```

- **Exit Criteria**
  - `knowledge_rag` endpoint returns indexed snippets with provenance metadata sourced via Qdrant + Neo4j.
  - Graph schema contract tests pass in CI against local Neo4j instance; vector integration smoke tests validate Qdrant connectivity.
  - Operational dashboards note ingestion latency and success metrics.
  - All verification checkpoints passed.
  - Implementation follows existing patterns.

### History 4: MCP Tooling Surface & Analyzer Integration (Epic E2)
- **Context**: Expose analyzers/fixers through MCP tools with deterministic validation workflows.
- **Plan of Action**
  1. Extend MCP tools (`src/code/IndFusion.Mcp.Tools`) with `lint_run`, `pattern_suggest`, `fixer001_apply_*`.
  2. Implement validation pipeline (`src/code/IndFusion.Mcp.Core/Validation/`) leveraging Roslyn + safe regex.
  3. Update analyzer manifests (`src/ExxerRules/ExxerAI.Rules.Analyzer/RuleRegistry.cs`) for MCP exposure.
  4. Add integration tests in `src/test/IndFusion.Mcp.Tests/` covering success/failure paths.
  5. Document request/response shapes in `docs/reference/mcp/`.
- **Exit Criteria**
  - MCP manifest lists EXXER analyzers/fixers with correct rule IDs.
  - Integration tests validate both successful and failure scenarios.
  - Documentation refreshed in `docs/reference/mcp/` with request/response samples.

### History 5: Agent Governance, Telemetry, and Guardrails (Epic E3)
- **Context**: Stand up supervision, guardrail enforcement, and telemetry required for agent-led changes.
- **Plan of Action**
  1. Enhance guardrail tooling (`GuardrailCheck.ps1`, CI hooks, JSON output).
  2. Emit telemetry via `SemanticRagTelemetry` (latency, adoption, guardrail breaches).
  3. Stand up dashboards per `docs/dashboards/README.md`; publish baseline KPIs.
  4. Schedule nightly context refresh (`Update-Agent-Brief.ps1`) and integrate with supervisor processes.
  5. Update governance docs/PR templates to enforce guardrail checks.
- **Exit Criteria**
  - Guardrail check required in PR template; failure blocks merges on scope breaches.
  - Telemetry dashboards show live data for at least one sprint.
  - Agent supervisors acknowledge daily digest in `docs/operations/AgentSyncLog.csv`.

### History 6: Cross-Repository Insights & Drift Remediation (Epic E4)
- **Context**: Detect standards drift across repos and generate actionable remediation backlogs.
- **Plan of Action**
  1. Build drift detection services (`src/code/IndFusion.Mcp.Analytics/Drift/`) feeding Neo4j/Qdrant.
  2. Automate backlog creation via `src/scripts/automation/New-DriftBacklog.ps1` and Azure Boards API.
  3. Expand analyzer drift tests (`src/test/IndFusion.Analyzer.Tests/TestCases/UnifiedSemanticRag/`).
  4. Publish quarterly drift reports (`docs/insights/UnifiedRagDriftReports.md`).
  5. Configure SLA dashboards tracking remediation velocity.
- **Exit Criteria**
  - Nightly drift job produces backlog entries tagged `SemanticRAG`.
  - Analytics dashboards highlight drift trends; remediation SLA tracking live.
  - Documentation references current drift algorithm and remediation playbooks.

### History 7: Pilot Enablement & Programme Close-Out
- **Context**: Validate end-to-end flow with pilot repositories, capture learnings, and transition to steady state.
- **Plan of Action**
  1. Enable pilot repos (IndFusion, IndTrace) with branch policies, guardrail checks, telemetry.
  2. Execute `src/scripts/Validate-PostDeliveryGate.ps1` to validate completion, archive artefacts in `docs/operations/due-diligence/`.
  3. Run retrospectives, document outcomes (`docs/operations/retros/PLAN-0001.md`), and share lessons.
  4. Transition ownership to DevEx/analyzer guild with runbooks and support docs.
  5. Refresh roadmap with post-pilot improvements.
- **Exit Criteria**
  - Post-delivery gate validation passed with all epics and stories marked complete.
  - Operational ownership transferred to DevEx/Analyzer guild with runbooks.
  - Close-out findings communicated to stakeholders; backlog groomed for steady-state improvements.

---

## Open Questions & Future Considerations

This section captures open questions and future considerations that don't need immediate decisions but should be tracked for future evaluation and decision-making.

### Repository Indexing Strategy

**Question**: How should we handle repository indexing to avoid server overload?

**Context**: Need to determine the optimal approach for indexing repositories in the IndFusion Semantic RAG system without overwhelming the server resources.

**Options Under Consideration**:
1. **On-Demand Indexing**: Index repositories only when requested by the LLM
2. **Selective Pre-Indexing**: Index only critical/active repositories  
3. **Tiered Indexing**: Always indexed (Tier 1) vs On-demand (Tier 2) vs Never indexed (Tier 3)

**Repository Selection Criteria** (To Be Determined):
- LLM request-based indexing
- Project importance ranking
- Recent activity levels
- Manual curation approach
- Performance impact considerations

**Related Considerations**:
- Memory management for repository switching
- Context persistence strategies
- Cleanup mechanisms for inactive repositories
- Performance monitoring and optimization

### Serena MCP Server Integration

**Question**: How should we integrate Serena MCP Server capabilities?

**Context**: Serena MCP Server by oraios offers valuable code analysis tools (semantic search, regex search/replace, memory management, project activation) but has shown performance degradation.

**Current Status**:
- ✅ **Initial Performance**: Worked like a charm initially
- ⚠️ **Performance Issues**: Noticed degradation over time
- ❌ **Stats/Logging**: Stats functionality never worked well
- 🔍 **Investigation Needed**: Need to analyze root cause of degradation

**Integration Options**:
1. **Adapt Serena's tools** into our MCP server
2. **Use Serena as separate service** with IndFusion integration
3. **Integrate Serena capabilities** into ExxerAI foundation
4. **Selective tool adoption** - pick only working/stable tools

**Investigation Required**:
- Root cause analysis of performance degradation
- Evaluation of stats/logging system issues
- Assessment of current stability and reliability
- Performance benchmarking against alternatives

**Serena Tools of Interest**:
- `FindSymbolTool` - Symbol-level code understanding
- `FindReferencingSymbolsTool` - Reference tracking
- `ReplaceSymbolBodyTool` - Code manipulation
- Memory management (read/write memories)
- Semantic search capabilities
- Regex search and replace
- Project activation and context management

### Package Indexing Strategy

**Question**: How should we handle package indexing to provide current, relevant information to LLMs?

**Context**: LLMs suffer from outdated package information. Need to determine what packages and patterns to index to provide the most value.

**Indexing Priorities**:
1. **Breaking Changes**: API changes, method signatures, parameter changes, deprecations
2. **New Patterns**: Emerging design patterns, framework updates, best practices
3. **Atypical Patterns**: Creative solutions, performance optimizations, security patterns
4. **Contextual Indexing**: Based on project type, framework, existing patterns

**Key Considerations**:
- What constitutes "atypical" vs "brand new" patterns
- How to detect breaking API changes automatically
- When to index new packages vs just breaking changes
- How to prioritize indexing based on project context

### Technology Stack Decisions

**Question**: Final technology stack configuration and deployment strategy

**Context**: While we have a strong foundation with ExxerAI adaptation, some technology choices remain open for future optimization.

**Open Decisions**:
- **Vector Database**: Qdrant vs PostgreSQL with pgvector (both supported)
- **Graph Database**: Neo4j implementation details and optimization
- **LLM Models**: Specific Ollama model selection for different use cases
- **OCR Engine**: Tesseract configuration and language support
- **Caching Strategy**: Redis vs in-memory vs hybrid approach
- **Deployment**: Docker vs Kubernetes vs hybrid deployment

### Performance and Scalability

**Question**: Performance optimization and scalability strategies

**Context**: Need to define performance targets and scalability approaches as the system grows.

**Open Considerations**:
- **Performance Targets**: Response time SLAs, throughput requirements
- **Scalability Strategy**: Horizontal vs vertical scaling approaches
- **Resource Management**: Memory usage optimization, CPU utilization
- **Load Balancing**: Multi-instance deployment strategies
- **Caching Strategy**: What to cache, cache invalidation policies
- **Database Optimization**: Query optimization, indexing strategies

### Security and Compliance

**Question**: Security model and compliance requirements

**Context**: Need to define security requirements and compliance standards for the IndFusion platform.

**Open Considerations**:
- **Authentication Strategy**: JWT vs OAuth vs other approaches
- **Authorization Model**: Role-based access control implementation
- **Data Privacy**: GDPR compliance, data retention policies
- **Audit Logging**: What to log, retention periods, compliance requirements
- **Encryption**: Data at rest vs in transit encryption strategies
- **API Security**: Rate limiting, input validation, security headers

### Monitoring and Observability

**Question**: Comprehensive monitoring and observability strategy

**Context**: Need to define monitoring, logging, and observability approaches for production deployment.

**Open Considerations**:
- **Metrics Collection**: What metrics to track, collection frequency
- **Logging Strategy**: Log levels, structured logging, log aggregation
- **Alerting**: Alert thresholds, notification channels, escalation procedures
- **Health Checks**: Service health monitoring, dependency health
- **Performance Monitoring**: APM tools, performance profiling
- **Business Metrics**: User adoption, feature usage, success rates

### Decision Framework

**When to Revisit These Questions**:
1. **Performance Issues**: When current approach shows limitations
2. **Scalability Needs**: When system reaches capacity constraints
3. **User Feedback**: When users request specific capabilities
4. **Technology Changes**: When new technologies become available
5. **Business Requirements**: When business needs evolve

**Decision Criteria**:
- **Performance Impact**: How does the decision affect system performance?
- **Development Effort**: What's the implementation complexity?
- **Maintenance Overhead**: What's the ongoing maintenance cost?
- **User Value**: How does it improve user experience?
- **Technical Risk**: What are the implementation risks?

---

**Note**: These open questions should be revisited as the project progresses and more information becomes available. Decisions should be made based on actual performance data, user feedback, and business requirements rather than theoretical considerations.
