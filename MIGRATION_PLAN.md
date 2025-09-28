## IndFusion Architecture Migration Plan

### Goals
- Align current repo to the target structure in `IndFusionPracticalArchitecture.md`.
- Preserve build green status at each step; keep history via branches and zips.
- Minimize downtime for teams; provide clear handoff points.

### Guardrails
- Work only on branch: `architecture-migration`.
- Commit small, reversible steps; push after each green build.
- Keep `main` protected; merge only when the migration test matrix is green.

### Phases
1) Preparation
- Create branch `architecture-migration`.
- Create baseline backup via `git archive` and working-tree zip.
- Ensure current solution builds cleanly (done).

2) Folder scaffolding
- Create `/src/code` and `/src/test` roots.
- Add READMEs describing purpose of each root.

3) Solution and projects layout
- Create or update solution to include new target groups:
  - code/: IndFusion.* projects per architecture list
  - test/: IndFusion.*.Tests projects mirroring code

4) Incremental moves
- Move current ExxerFactor.* projects to their equivalents:
  - ExxerFactor.Mcp.Core -> IndFusion.Mcp.Core
  - ExxerFactor.Mcp.Web -> IndFusion.Mcp.Web
  - Test projects accordingly into /src/test
- After each move:
  - Fix project references and PackageReferences
  - Run build and unit tests
  - Push

5) Add missing scaffolds from the master list
- Create empty SDK/observability/provider placeholders as needed with README and Directory.Build.props linking disabled build (Compile Remove="**/*").

6) CI updates
- Add pipeline to build all code/* and test/*
- Enable per-folder selective build where possible

7) Finalize
- Update documentation
- Merge back to main with tags

### Mapping Table (initial)
- ExxerFactor.Mcp.Core -> IndFusion.Mcp.Core
- ExxerFactor.Mcp.Web -> IndFusion.Mcp.Web
- ExxerRules.Analyzers -> IndFusion.Analyzer
- ExxerRules.CodeFixes -> IndFusion.Fixer
- VS utilities -> IndFusion.Build/tools or scripts folder

### Risks
- Namespace churn causing ambiguous references.
- Package restore mismatch after moves.
- Lost files during moves: mitigated by archived zips and branch.

### Handoff
- Keep this plan updated; check off items in the project TODOs after each step.
