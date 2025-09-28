# Warning and Suppression Audit

This document tracks enforcement decisions related to warnings-as-errors and any scoped suppressions. Goal: ship with `TreatWarningsAsErrors` enabled, avoid behavioral changes, and never use global/blanket suppressions.

## Enforcement Model
- Global: `TreatWarningsAsErrors=true` via `Directory.Build.props`.

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
