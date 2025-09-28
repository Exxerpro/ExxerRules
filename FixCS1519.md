## Hand-off Prompt: Finish CS1591 XML Documentation in IndFusion.sln

- Objective: Eliminate all CS1591 “Missing XML comment for publicly visible type or member” errors across the solution without changing logic. Focus is the `ExxerFactor.Mcp.Core` project; continue until CS1591 count is zero.

### Context
- Root workspace: `F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules`
- Solution: `src/IndFusion.sln`
- Offline NuGet: use `src/NuGet.config` (local-only feed). Do not modify feeds.
- Central props: `src/Directory.Build.props`, `src/Directory.Packages.props`
- Current status: CS1591 count is down to 1. Many files already documented across `Abstractions`, `Exceptions`, `Services`, `SyntaxWalkers`, `SyntaxRewriters`, and `Tools`. Recent files documented include:
  - `Extensions/ServiceCollectionExtensions.cs`
  - `Logging/LoggingConfiguration.cs`
  - `Move/MoveMethodAst.cs`
  - `Move/MoveMethodFileService.cs`
- Likely remaining: one public member/type still missing XML docs; identify via the build step below.

### Hard Requirements
- Add XML docs only; do not change code logic or signatures.
- Document all public classes/records/interfaces, and all public methods and properties inside them.
- Use `<summary>`, `<param name="...">`, and `<returns>` as appropriate.
- Do not overwrite valid XML docs; only add where missing or clearly incomplete.
- For attributes (e.g., `[McpServerTool]`, `[McpServerToolType]`), place XML comments above the attribute to avoid CS1587.
- For records with primary constructor parameters, include `<param>` entries for each parameter.
- Preserve existing indentation and formatting.
- Keep to .NET XML documentation conventions.

### Build and Error Capture (PowerShell 7)
Run from repo root. Use offline feed. Write filtered errors to `src/errors.txt`.

```powershell
pwsh -NoLogo -NoProfile -ExecutionPolicy Bypass -Command `
  "& { cd src; dotnet build IndFusion.sln --configfile NuGet.config 2>&1 `
  | Tee-Object -Variable all `
  | Where-Object { $_ -match '(: |\s)error\s' } `
  | Set-Content errors.txt; `
  (Get-Content errors.txt | Measure-Object -Line).Count }"
```

- If the count > 0, open `src/errors.txt`, locate CS1591 lines, and document those members.
- Re-run the command after each batch until the count is 0.

### Editing Guidance
- Class/record/interface:
```csharp
/// <summary>
/// Brief but meaningful purpose.
/// </summary>
public static class SomePublicType { ... }
```
- Method with parameters and return:
```csharp
/// <summary>
/// What the method does and key behavior.
/// </summary>
/// <param name="arg1">Meaning.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>The computed result.</returns>
public Task<Result> DoWorkAsync(int arg1, CancellationToken ct) { ... }
```
- With attributes (avoid CS1587):
```csharp
/// <summary>
/// Exposes the tool for XYZ.
/// </summary>
[McpServerTool]
public static string Run(string input) { ... }
```
- Record with primary ctor parameters:
```csharp
/// <summary>
/// Represents XYZ.
/// </summary>
/// <param name="name">Name.</param>
/// <param name="value">Value.</param>
public record MyRecord(string name, int value);
```

### Prioritization
1. Use `src/errors.txt` to target the remaining CS1591 precisely. Fix the exact file/member indicated.
2. If that file has other public members lacking docs, cover them while you’re there.
3. Rebuild and repeat until zero.

### Quality Bar / Acceptance Criteria
- `src/errors.txt` shows 0 lines after build.
- No CS1587 introduced (check attributes comment placement).
- No changes to runtime logic, only XML documentation additions.
- Build succeeds using offline NuGet config.
- Preserve indentation style and width.

### Nice-to-have Follow-ups (after CS1591=0)
- Rebuild full solution without filters to confirm no other errors (e.g., pre-identified CS0017 in `VS/StrykerRunner.csproj`) if in scope.

### Notes
- Key folders of interest for remaining CS1591: `src/ExxerFactor/ExxerFactor.Mcp.Core/Extensions`, `Logging`, `Move`, `SyntaxRewriters`, `Tools`.
- If you see public nested classes or public properties inside these types, document them too.

### Summary Instructions (do these in a loop)
- Run the filtered build command.
- Open `src/errors.txt`, fix all CS1591 in the referenced files.
- Rebuild and repeat until count is zero.
