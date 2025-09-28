Purpose
 - Track and justify code analysis suppressions which are necessary to ship with TreatWarningsAsErrors enabled.

Current suppressions

- NU1903 (known vulnerability advisory on Microsoft.Build.Tasks.Core)
  Scope: solution-wide via `src/Directory.Build.props` NoWarn
  Justification: The warning originates from a transitive dependency; we do not reference the package directly. We will prioritize upgrading transitive packages (e.g., test SDK / build tooling) to versions which no longer bring in the vulnerable range. Until then, builds would be blocked despite no exploitable usage in our code paths. This suppression is temporary and will be removed once dependency upgrades eliminate the advisory.
  Owner: Build/Tooling
  Follow-up: Audit `Directory.Packages.props` regularly and bump relevant packages; remove suppression when clear.

- Test-only xUnit1051 pragmas
  Scope: localized in tests that intentionally validate cancellation behavior
  Justification: Those tests must pass a custom `CancellationToken` (e.g., a cancelled token) rather than `TestContext.Current.CancellationToken`; suppressions are narrowly scoped with `#pragma` and comments.

Notes
- We avoid global, undocumented suppressions. When suppression is necessary, we scope to the smallest surface, comment inline when possible, and record here with an owner and remediation path.
