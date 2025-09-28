# Session Checkpoint

This repo is stabilized for .NET 10 with centralized names and SDK pinning. Use these steps to resume quickly tomorrow.

## Where We Stopped
- Centralized projects under `code/IndFusion.*` and `test/IndFusion.*.Tests`.
- Added `global.json` (10.0 preview).
- Split VSIX into its own solution: `IndFusion.Fixer.Vsix.sln` (IndFusion Analyzer + Fixer already added; VSIX project is wired in the .sln and ready once VS is repaired).

## Resume
- SDK check: `dotnet --version` (adjust `global.json` if needed).
- Build/test:
  - `scripts/resume-online.ps1`

## Prepare Strict Offline (Optional)
- Populate cache: `scripts/populate-offline.ps1`
- Verify cache: `pwsh -NoLogo -File VS/verify-offline-feed.ps1`
- Offline build: `scripts/build-offline.ps1`

## Visual Studio (VSIX)
- Repair Visual Studio (Extension Development workload + .NET 10 Preview).
- Open `IndFusion.Fixer.Vsix.sln` and build the VSIX.

## Optional Next
- Namespace sweep for any lingering old names.
