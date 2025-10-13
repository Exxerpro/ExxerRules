# Agent Behavior Guidelines

## Purpose
Define the professional standards and working norms for development agents contributing to the Semantic RAG initiative (and related IndFusion projects). These guidelines are mandatory for human and autonomous agents alike.

## Core Principles
- **Proactivity**: Anticipate follow-up tasks, surface potential risks, and propose next steps without waiting to be asked.
- **Completeness**: Deliver end-to-end results—code changes, tests, documentation, scripts, and validation—rather than partial updates.
- **Evidence-Based**: Back assertions with data (test results, telemetry snapshots, file references). No change is “done” until it’s verified.
- **Traceability**: Reference files and line numbers using the CLI’s clickable path format (e.g., `src/app.ts:42`).
- **Professional Tone**: Communicate clearly, concisely, and respectfully. Summaries must lead with concrete actions taken, followed by verification status and next steps.
- **Continuous Learning**: Record lessons learned, knowledge gaps, and recommended improvements in the relevant doc (e.g., `docs/operations/due-diligence/PLAN-0001-findings.md`).

## Behavioral Expectations
1. **Before starting work**
   - Review the latest `SemanticRag-Agent-Brief.md` and acknowledge the digest in `docs/operations/AgentSyncLog.csv`.
   - Run `Update-Agent-Brief.ps1 -CheckOnly` to ensure the context package is current.
   - Confirm guardrails and dependencies for the work package (e.g., repo manifest, external services).

2. **During execution**
   - Follow the Hexagonal (Ports & Adapters) architecture when modifying services; isolate domain logic from infrastructure.
   - Apply SOLID principles to any new code; refactor legacy sections where impractical patterns would otherwise spread.
   - Provide code samples or pseudo-code when explaining changes in documentation or summaries.
   - Run relevant tests (unit, integration, contract) and attach outputs; if tests are skipped, document why and outline mitigation.
   - Monitor telemetry or logs when changes impact runtime behavior; capture before/after metrics when possible.

3. **Upon completion**
   - Update documentation (README, runbooks, ADRs) to reflect new behavior.
   - Record due diligence results using `DueDiligence-PostFinish.ps1` and summarize outcomes in findings documents.
   - Offer future-looking next steps (e.g., performance tuning, additional automation).
   - Ensure no lint or analyzer violations remain; run `dotnet format --verify-no-changes` where applicable.

## Deliverable Checklist
- [ ] Code conforms to SOLID and hexagonal architecture guidelines.
- [ ] Tests executed and results logged (include commands).
- [ ] Documentation and runbooks updated with references to files/line numbers.
- [ ] Guardrails (linting, formatting, security checks) verified.
- [ ] Due diligence scripts executed; artifacts stored under `docs/operations/due-diligence/`.
- [ ] Summary message includes actions, verifications, and suggested next steps.

## Enforcement
- `GuardrailCheck.ps1` must confirm an agent has acknowledged the latest digest and that the behavior guidelines are referenced in the commit summary.
- Supervisors review agent logs (`agent-trace/<workItemId>.log`) daily; deviations are escalated to the Tech Lead.
- Repeated violations trigger temporary suspension from autonomous work until the agent completes remediation training documented in `docs/operations/governance/AgentRemediation.md`.

## Change Management
- Updates to this guideline require approval from the programme Tech Lead and documentation in `docs/operations/governance/ChangeLog.md`.
- All changes must be reflected in the agent brief digest so agents re-acknowledge new expectations.

Adhering to this policy ensures every agent contribution is professional, thorough, and high-value—aligned with IndFusion’s engineering standards.
