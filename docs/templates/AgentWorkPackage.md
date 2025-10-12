# Agent Work Package Template

Populate each section before assigning the task to a development agent.

## Metadata
- **Work Item ID**: `<Azure Boards ID>`
- **Title**: `<Short description>`
- **Owner**: `<Supervisor name>`
- **Target Sprint**: `<Sprint label>`
- **Related Epics/Milestones**: `<E# / M#>`

## Objective
> One-sentence summary of the desired outcome (include measurable success criteria).

## Code Surface
- `src/...`
- `docs/...`
- Additional constraints: `<e.g., read-only files, generated code>`

## Guardrails
- Required analyzers/rules to monitor: `<EXXER IDs>`
- Forbidden APIs / patterns: `<e.g., no direct SQL> `
- Dependency rules: `<e.g., do not add new NuGet packages>`
- Security/offline notes: `<e.g., offline feeds only>`

## Inputs
- Knowledge base links: `<docs/...>`
- ADRs/decisions: `<docs/adr/...>`
- Existing tests/examples: `<src/test/...>`
- Telemetry dashboards: `<docs/dashboards/...>`

## Steps
1. `<Specific action>`
2. `<Specific action>`
3. `<Specific action>`

## Expected Outputs
- Updated code files: `<list>`
- New/updated tests: `<list>`
- Documentation edits: `<list>`
- Telemetry hooks/logging: `<list>`

## Verification
- Command 1: ``dotnet build IndFusion.sln -c Release`` (expected: success)
- Command 2: ``dotnet test <path>`` (expected: success)
- Additional: `<custom scripts, manual checks>`

## Telemetry & Logging
- Metrics to emit: `<SemanticRag.*>`
- Log file: `agent-trace/<workItemId>.log`
- Additional instrumentation: `<if applicable>`

## Sign-off Checklist
- [ ] All steps completed
- [ ] Verification commands executed and logged
- [ ] Code reviewed (reviewer: `<name>`)
- [ ] Documentation updated and cross-linked
- [ ] Work item updated with summary + attachments

