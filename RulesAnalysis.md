## ExxerRules Analyzer False Positives & Weak Points

This document summarizes likely false positives and weak points for each analyzer in `src/code/IndFusion.Analyzer`, with emphasis on patterns users reported (e.g., magic numbers/strings flagged even when first assigned to readonly/static fields). For each rule, we list: what it flags, why it can misfire, examples of problematic cases, and mitigation ideas.

---

### EXXER500 AvoidMagicNumbersAndStringsAnalyzer
- **Flags**: Any numeric or string literal occurrences except a few exemptions (const, static readonly fields, attribute args, switch/case, some common literals).
- **Why false positives happen**:
  - Only checks syntactic context around the literal; doesn’t verify that the literal is first assigned to a readonly/static field via a variable initializer on a different node (e.g., `private static readonly int DefaultPort = 8080;`). If the literal appears in an argument or intermediate expression of the initializer chain, the current `IsInConstantDeclaration` heuristic may miss it.
  - Very small allowlist for common numbers: only `0`, `1`, `-1`, `2` are exempt. Values like `10`, `60`, `100`, `1000`, bit flags, powers of two, time constants, and ports will trigger.
  - Strings: allowlist only checks for braces and a tiny set of common tokens. Valid short tokens (e.g., keys, names, MIME types, HTTP verbs) are flagged.
  - Array/collection initializers are exempt wholesale; but literals inside helper factory methods or records are not.
- **Example misfires**:
  - `private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);` → the `30` literal is flagged if seen outside a const/static readonly field syntax path.
  - `private static readonly int Port = 8080;` may be safe, but `Settings.DefaultPort = 8080;` in a static ctor is flagged.
  - Logging/Known IDs: `const string Json = "application/json";` is fine, but passing the same string inline to APIs in one-off spots gets flagged aggressively.
- **Mitigations**:
  - Expand exemptions: treat literals in field/property initializers where the target symbol is `const` or `static readonly` even when the literal is nested in an invocation or binary expression.
  - Consider semantic checks: if the literal is assigned to a `readonly` field on first assignment (single write), don’t flag.
  - Enlarge number/string allowlists (common time multipliers, powers of two, common ports 80/443, etc.).
  - Add project-configurable exclusions and minimum-length thresholds; allow attributes like `[SuppressMessage]` or a pragma comment.

---

### EXXER800 UseStructuredLoggingAnalyzer
- **Flags**: String concatenation or interpolation in logging calls.
- **Why false positives happen**:
  - Identifies logging by method name (e.g., `LogInformation`) without confirming the receiver is an `ILogger`. Any method named `LogInformation` on other types gets flagged.
  - Flags all interpolated strings even when they use structured templates ("User {UserId}")—this is sometimes acceptable in Microsoft ILogger APIs with interpolated strings handler.
  - Detects `+` anywhere in descendant expressions; may flag unrelated concatenations computed before the log call.
- **Mitigations**:
  - Use semantic model to verify receiver type is `Microsoft.Extensions.Logging.ILogger`.
  - Permit C# 10+ interpolated handler usage for ILogger overloads (semantic check).
  - Limit search to the message argument only, not arbitrary descendants outside the first parameter.

---

### EXXER801 DoNotUseConsoleWriteLineAnalyzer
- **Flags**: Any `Console.WriteLine/Write`.
- **Why false positives happen**:
  - Syntactic fallback treats any `Console` identifier as `System.Console` even if it’s an alias/local symbol.
  - Flags test/debug-only code in test projects or DEBUG regions.
- **Mitigations**:
  - Use semantic type checks and respect `#if DEBUG` or `[Conditional("DEBUG")]` context.
  - Allow in tests by project name/assembly attribute or namespace pattern.

---

### EXXER200 ValidateNullParametersAnalyzer
- **Flags**: Methods missing null checks for reference-type parameters.
- **Why false positives happen**:
  - Heuristics classify reference vs value types using textual name checks, then semantic fallback; may miss generics and nullable reference annotations (`string?`).
  - Only looks for a subset of validation patterns (`ThrowIfNull`, `throw new ArgumentNullException`, `Result.WithFailure`). Other guard libraries or custom guard methods are ignored.
  - Expression-bodied methods are automatically considered missing validation.
- **Mitigations**:
  - Prefer semantic `ITypeSymbol.IsReferenceType` and flow analysis to honor `?` annotations and parameter nullability context.
  - Support custom guard method attributes or configuration (e.g., `[NullChecked]`).
  - Don’t require guards for private/internal methods if all call sites pass non-null, or if parameters are annotated `NotNull`.

---

### EXXER001 UseResultPatternAnalyzer / EXXER002 AvoidThrowingExceptions / EXXER003 DoNotThrowExceptionsAnalyzer
- **Flags**: Throw statements/expressions in methods not returning `Result<T>`.
- **Why false positives happen**:
  - Skips are based on method name patterns and test attributes only; legitimate throws in boundaries (e.g., controller action translating domain errors) are flagged.
  - Does not differentiate between domain layer vs application/UI layers.
  - Local functions and lambdas flagged even when they are trivial argument validation.
- **Mitigations**:
  - Scope rules by project/namespace (e.g., enforce in Domain/Application layers, relax in API/UI).
  - Allow specific well-known exception types for guard clauses and argument validation.
  - Recognize rethrow in catch with wrapping, do not flag rethrows.

---

### EXXER300 AsyncMethodsShouldAcceptCancellationTokenAnalyzer
- **Flags**: Async methods without `CancellationToken`.
- **Why false positives happen**:
  - Heuristic to detect “application code” by class/namespace names is brittle; library code inside `WebApp` namespace or generated files may be misclassified.
  - Exempts `async void`, but other event or pipeline callbacks (ASP.NET filters, minimal APIs) may legitimately omit tokens.
- **Mitigations**:
  - Use project/assembly markers to decide library vs application (e.g., SDK type, project name, analyzer additional files config).
  - Respect ASP.NET Core action signatures and framework-provided tokens; allow when token is available at higher scope.

---

### EXXER301 UseConfigureAwaitFalseAnalyzer
- **Flags**: Await without `ConfigureAwait(false)` outside “application code”.
- **Why false positives happen**:
  - “Application code” detection relies on name substrings; libraries embedded in app repos will still be flagged.
  - Ignores cases where context capture is required (UI code, ASP.NET request context in middleware).
- **Mitigations**:
  - Honor SynchronizationContext presence analysis or known framework contexts; allow opt-out via file/project configuration.
  - Prefer advisory (Info) severity and code fix with rationale instead of strict warning.

---

### EXXER400 PublicMembersShouldHaveXmlDocumentationAnalyzer
- **Flags**: Missing XML docs on public members and interface members.
- **Why false positives happen**:
  - Considers any public field/event as requiring docs; for generated or trivial backing fields this is noisy.
  - Doesn’t check `inheritdoc` or documentation inheritance from interface/base.
- **Mitigations**:
  - Recognize `/// <inheritdoc/>`.
  - Skip for `[GeneratedCode]`/`[CompilerGenerated]` or files under generated directories.

---

### EXXER501 UseExpressionBodiedMembersAnalyzer
- **Flags**: Members with single return bodies to simplify.
- **Why false positives happen**:
  - Uses length threshold only; ignores team style or when expression-bodied reduces readability (complex expressions, multiple returns later).
- **Mitigations**:
  - Respect `.editorconfig` preferences (IDE0022/IDE0023) and severity; make this advisory.

---

### EXXER702 UseModernPatternMatchingAnalyzer
- **Flags**: `is` followed by cast in body.
- **Why false positives happen**:
  - Simple text pattern search; flags when the cast variable differs, or when pattern matching would capture a broader scope.
- **Mitigations**:
  - Semantic data flow to verify that the same identifier is cast and used; ensure safety-equivalence before suggesting.

---

### EXXER601 UseRepositoryPatternAnalyzer
- **Flags**: Non-repository classes referencing DbContext or ADO.NET directly.
- **Why false positives happen**:
  - Type detection is name-based; custom contexts may be missed or false-flagged.
  - Repository/interface detection relies on name patterns (`*Repository`), missing DDD naming variants (e.g., `CatalogStore`).
- **Mitigations**:
  - Use semantic inheritance checks for EF Core `DbContext` and `IDbConnection`.
  - Allow configuration of boundary classes/namespaces where direct access is approved (migrations, composition roots).

---

### EXXER600 DomainShouldNotReferenceInfrastructureAnalyzer
- **Flags**: `using` directives in Domain referencing Infrastructure.
- **Why false positives happen**:
  - Based on namespace string contains "Domain" / "Infrastructure"; projects with different naming will be misclassified.
  - Flags EF Core namespaces even when used in test helpers within domain folder.
- **Mitigations**:
  - Drive boundaries from project references (Domain project should not reference Infrastructure project) using compilation references.
  - Make namespace patterns configurable.

---

### EXXER700 UseEfficientLinqAnalyzer
- **Flags**: Multiple enumerations and inefficient chains.
- **Why false positives happen**:
  - Purely syntactic heuristics; cannot distinguish deferred queries, materialized collections, or side-effect-free repeated calls.
  - May flag separate variables with same base name erroneously.
- **Mitigations**:
  - Track symbols with semantic model; detect materialization (`ToList`) before subsequent operations.
  - Lower severity to Info and provide targeted examples.

---

### EXXER503 DoNotUseRegionsAnalyzer
- **Flags**: Any `#region` directive.
- **Why false positives happen**:
  - Blanket rule; teams sometimes use regions for P/Invoke, conditional code, or designer-generated code.
- **Mitigations**:
  - Skip generated files; allow specific region names; reduce severity to Info.

---

### EXXER900/901 CodeFormattingAnalyzer / ProjectFormattingAnalyzer
- **Flags**: Formatting inconsistencies; always-on hidden formatting trigger.
- **Why false positives happen**:
  - Formatting heuristics are simplistic; brace/spacing checks can be noisy and conflict with `.editorconfig`.
- **Mitigations**:
  - Defer to `dotnet format` diagnostics or Roslyn IDE analyzers; align with `.editorconfig`.

---

### EXXER101/102/103/104 Testing analyzers (UseXUnitV3, UseShouldly, UseNSubstitute, DoNotMockDbContext)
- **Flags**: Using directives/attributes indicating other frameworks or mocking DbContext.
- **Why false positives happen**:
  - Name-based detection may flag unrelated namespaces (e.g., a type named `Mock`).
  - DoNotMockDbContext uses generic argument string extraction; may mis-detect when symbol resolution fails.
- **Mitigations**:
  - Prefer semantic symbol checks for framework types and base types (`DbContext`).
  - Allow per-project overrides (legacy tests).

---

## Cross-cutting recommendations
- Add analyzer configuration via `.editorconfig` or additional files to tune namespaces, severity, and exemptions.
- Prefer semantic model and symbol-based checks over string/name heuristics.
- Respect generated code markers and test projects.
- Provide suppression mechanisms (attributes/pragma) and clear diagnostics with guidance.
- Add telemetry hooks behind a flag to learn common suppressions for future tuning.
