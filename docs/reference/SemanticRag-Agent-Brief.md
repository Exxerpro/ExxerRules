# Semantic RAG Agent Brief

## Purpose
- Enable development agents to deliver changes that align with the Unified Semantic RAG Standards Initiative.
- Provide the minimal context required to execute work packages safely, deterministically, and offline-first.

## Operating Scope
- Primary repositories: `IndFusion.sln`, `IndTrace.sln`, satellite analyzers under `src/ExxerRules/`.
- Core components: MCP server (`src/code/IndFusion.Mcp.Server/`), knowledge fabric (`src/code/IndFusion.Mcp.Core/Knowledge/`), analyzer rule set (`src/ExxerRules/ExxerAI.Rules.Analyzer/`), MCP tooling (`src/code/IndFusion.Mcp.Tools/`).
- Tests must live under `src/test/IndFusion.Analyzer.Tests/` or `src/test/IndFusion.Mcp.Tests/` using xUnit v3 + Shouldly.

## Guardrails
- Never modify files outside defined `Code Surface` without supervisor approval.
- All code compilations must succeed with `dotnet build IndFusion.sln -c Release`.
- Formatting and analyzers are enforced via `dotnet format --verify-no-changes` and EXXER analyzers (warnings treated as errors).
- Assume offline mode: prefer packages from `artifacts/nuget/offline/`; avoid network calls unless explicitly authorised.
- Capture provenance: mention analyzer IDs (e.g., `EXXER0401`) and tool contracts in code comments or documentation updates when relevant.

## Required Checks
1. `dotnet restore IndFusion.sln`
2. `dotnet build IndFusion.sln -c Release`
3. `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release --collect:"XPlat Code Coverage"` (or targeted subset when instructed)
4. Additional commands listed in the assigned work package

Logs from every command go to `agent-trace/<workItemId>.log`.

## Knowledge Sources
- `docs/Unified-Semantic-RAG-Standards-Initiative.md`
- `docs/reference/ExxerRuleMatrix.md` (rule catalogue, owner, severity)
- `docs/playbooks/` (automation playbooks; consult relevant guide)
- ADR index under `docs/adr/` (decisions, rationale, impacted modules)
- Telemetry dashboards baseline (`docs/dashboards/README.md`)

## Communication
- Announce start/stop of each work session in Teams channel `#semantic-rag`.
- Raise blockers immediately to the Agent Supervisor; include work item ID and last successful command.
- Update `docs/reference/SemanticRag-Agent-Brief.md` only via supervisor request or change request referencing an ADR; log behavioural policy updates in `docs/operations/governance/CHANGELOG.md`.

## Escalation Triggers (Agent Perspective)
- Verification script fails twice.
- Detected schema mismatch or analyzer contract drift.
- Required offline dependency missing or corrupt.

Escalate via Teams `#semantic-rag` tagging the Tech Lead, then log the event in the work item.

## Daily Sync Requirements
- Prior to coding, run `pwsh src/scripts/Update-Agent-Brief.ps1 -CheckOnly` and record the reported checksum in the Agent Sync Log; review `docs/reference/Agent-Behavior-Guidelines.md` if the digest has changed since the last session.
- Confirm receipt of any broadcast guardrail updates before committing.

## Deliverable Checklist
- Code changes scoped to approved directories.
- Tests updated/added and passing.
- Documentation links maintained (include file + line references where applicable).
- Telemetry hooks (if required) emitting `SemanticRag.*` metrics.
- Work item comment summarising changes, commands executed, and log location.
- Behavioural compliance logged referencing `docs/reference/Agent-Behavior-Guidelines.md`.
