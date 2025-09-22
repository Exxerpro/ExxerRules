# Warning and Suppression Audit

This document tracks enforcement decisions related to warnings-as-errors and any scoped suppressions. Goal: ship with `TreatWarningsAsErrors` enabled, avoid behavioral changes, and never use global/blanket suppressions.

## Enforcement Model
- Global: `TreatWarningsAsErrors=true` via `Directory.Build.props`.
- XML docs:
  - Tests: `GenerateDocumentationFile=false` (Condition: `IsTestProject==true`). Rationale: test assemblies do not ship public APIs; avoids CS1591 noise without suppressing the rule.
  - Executables: `GenerateDocumentationFile=false` (Condition: `OutputType==Exe`). Rationale: console apps do not expose reusable APIs.

## Addressed Warnings
- CS1591 (Missing XML comment):
  - Scope: many in tests and the CLI executable.
  - Action: disabled XML doc generation for tests and executables (see above). Libraries keep XML docs and CS1591 enforcement.
  - Justification: No runtime behavior change; aligns enforcement with shipping surfaces.

- CS8602/CS8604 (Nullable dereference/argument):
  - Scope: IndFusion.Fixer (netstandard2.0) — flow analysis flagged chained member access in helper predicates and potential null return expressions when converting switch statements.
  - Action: refactored null checks to use local variables and explicit guards; added a non-null check before constructing `SwitchExpressionArm` and used `!` where proven safe.
  - Files: `Architecture/DomainShouldNotReferenceInfrastructureCodeFixProvider.cs`, `ModernCSharp/ExpressionBodiedMembersCodeFixProvider.cs`, `ModernCSharp/ModernPatternMatchingCodeFixProvider.cs`.
  - Justification: Improves null-safety without changing behavior; eliminates warnings instead of suppressing them.

- CS0219 (Variable assigned but never used):
  - Scope: `IndFusion.Mcp.Tests` (Move*ToolTests).
  - Action: removed unused `expected*` constants; assertions already verify outcomes.
  - Justification: dead code removal in tests; no change to behavior or coverage intent.

## Temporary Global NoWarn (to be scoped or removed)
Removed. No global NoWarn remains.

Scoped outcomes instead:
- NU1510 (package prune suggestions)
  - Action: removed redundant PackageReference entries from projects where the APIs are provided by the shared framework:
    - IndFusion.Mcp.Web: removed Microsoft.AspNetCore.Components.Web, Microsoft.Extensions.Hosting, System.Text.Json.
    - IndFusion.Mcp.Server: removed System.Text.Json.
    - IndFusion.Mcp.Web.Tests: removed Microsoft.Extensions.DependencyInjection.
  - Justification: packages were unused or provided by the target framework; no behavioral change.

- NU1603 (version resolution mismatch)
  - Action: aligned central versions with resolvable versions:
    - AngleSharp.Css -> 1.0.0-beta.157 (to match bunit).
  - Justification: resolves restore mismatch without affecting runtime behavior.

- NU1903 (Microsoft.Build.Tasks.Core advisory)
  - Action: Attempt central upgrade first (tracked via Directory.Packages.props). A subset of projects still surface the advisory via SDK/transitive restore. Added scoped `NoWarn=NU1903` in production projects only: `IndFusion.Mcp.Core`, `IndFusion.Mcp.Server`, `IndFusion.Mcp.Web`, `IndFusion.Tools.Cli`.
  - Justification: Restore advisory does not affect runtime behavior; suppression is temporary and project-scoped, never global. We will remove once upstream packages resolve the advisory.

## Scoped Suppression Index
- NU1903: transitive advisory
  - Projects: IndFusion.Mcp.Core, IndFusion.Mcp.Server, IndFusion.Mcp.Web, IndFusion.Tools.Cli
  - Rationale: transient advisory via SDK tasks; unblock WAE restore/build; revisit after dependency updates.

## How to Audit Locally
- Inventory warnings without failing the build:
  - `dotnet build IndFusion.sln -c Debug -p:TreatWarningsAsErrors=false -v n`
- After remediation, enforce:
  - `dotnet build IndFusion.sln -c Release`
