### Doc Quality Analyzer – Story & Plan

**Context**

- We keep finding XML docs that technically satisfy CS1591 but provide no real information (e.g., summaries that just repeat the member name or `<param>` descriptions that echo the identifier).
- We also noticed missing `<returns>` blocks on methods that return meaningful data, plus inconsistent remarks around cancellation behavior.
- Goal: add a Roslyn analyzer that enforces richer doc comments across the codebase and prevents regressions during CI.  
  This analyzer should live alongside the existing analyzers (outside the indfusion.mcp tests tree) and ship with unit tests.

---

### Milestones

1. **Scouting (complete)**  
   - Catalogued examples of “empty” docs:  
     `InitialPatterns`, `GetHighestConfidencePatternAsync`, and others with summaries like “Gets the …”.  
     - `<param name="fieldName">Field name</param>`  
     - `<param name="documentType">EAIDocument type</param>`  
     - Missing `<returns>` commentary.  
   - Confirmed we need semantic awareness to avoid false positives (e.g., allow “Gets …” when returning simple DTO wrappers from generated code).

2. **Rule Design (to implement)**
   - *Rule DQ0001 – SummaryMustBeInformative*  
     Detect summary lines that simply restate the member name, begin with “Gets the”, “Returns the”, or match the PascalCase identifier.  
     Require ≥5 words with at least one verb not present in the identifier.  
   - *Rule DQ0002 – ParamDescriptionMustAddValue*  
     `<param>` text must not be identical (case-insensitive) to the parameter name, and must exceed a minimal token length (e.g., ≥3 words).  
   - *Rule DQ0003 – ReturnDescriptionRequired*  
     If a method returns a non-void/non-task type (or a `Task<T>`), a `<returns>` element must exist and include more than boilerplate (no “Returns the result”).  
   - *Rule DQ0004 – CancellationRemarkRequired*  
     If the member accepts a `CancellationToken`, require either a `<param>` explanation mentioning cancellation or a `<remarks>` section describing cancellation behavior.  
   - Each rule emits a diagnostic with precise location: summaries point to the `<summary>` node, params to the offending `<param>` tag, etc.

3. **Analyzer Implementation**
   - Create a new project `IndFusion.Analyzers.DocQuality` (Roslyn analyzer template).
   - Author the analyzer using syntax/semantic analysis of `DocumentationCommentTriviaSyntax`.
   - Register each rule with `DiagnosticDescriptor` (category “Documentation”, severity `Warning`).

4. **Unit Tests**
   - Add a companion test project `IndFusion.Analyzers.DocQuality.Tests` using the Roslyn SDK test harness.
   - Cover positive and negative scenarios:
     - Valid summaries with actionable verbiage.
     - Invalid summaries (“Gets the value”) flagged with DQ0001.
     - Parameters documented with meaningful sentences.
     - Cancellation tokens documented in either `<param>` or `<remarks>`.
   - Include regression tests for auto-generated code (should suppress diagnostics).

5. **Repository Integration**
   - Wire the analyzer into `Directory.Build.props` so it runs on all projects, but allow opt-out via `#pragma warning disable DQxxxx` for generated files.
   - Update CI to treat DQ000x warnings as errors.
   - Provide fixer suggestions (optional but nice-to-have) for DQ0002 and DQ0003 (inserting TODO comments).

6. **Documentation**
   - Write a README under the analyzer project describing each rule, suppression guidance, and sample fixes.
   - Update contributor docs to highlight the new analyzer and command to run analyzer tests:  
     `dotnet test src/Analyzers/IndFusion.Analyzers.DocQuality.Tests`.

7. **Rollout**
   - Run the analyzer across the repository, fix existing hits, and commit in a dedicated change.  
     Track remaining TODOs as tech debt items.

---

### Next Steps

- [ ] Scaffold `IndFusion.Analyzers.DocQuality` and `…Tests` projects.
- [ ] Implement rules DQ0001–DQ0004.
- [ ] Create unit tests mirroring the bad patterns we’ve already spotted.
- [ ] Integrate into CI and repository build.
- [ ] Sweep the codebase and resolve new diagnostics.

Once those boxes are checked, we’ll have automated guardrails that prevent the “Field name” style docs from sneaking back in, and we can build additional rules as needed.

---

### Documentation Examples

Showcase the patterns we want to eliminate and what “good” looks like to help reviewers, contributors, and eventual fixer logic.

**Bad documentation**

```csharp
/// <summary>
/// Gets the pattern.
/// </summary>
/// <param name="documentType">Document type</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>The pattern</returns>
public async Task<PatternResult> GetHighestConfidencePatternAsync(
	EAIDocumentType documentType,
	CancellationToken cancellationToken)
{
	// Implementation
}
```

**Improved documentation**

```csharp
/// <summary>
/// Computes the most confident pattern for the supplied document type by
/// evaluating all registered extractors in priority order.
/// </summary>
/// <param name="documentType">
/// Identifies the document category whose applicable patterns must be scored.
/// </param>
/// <param name="cancellationToken">
/// Propagates caller intent to stop pattern evaluation before completion.
/// </param>
/// <returns>
/// A <see cref="PatternResult"/> containing the pattern and confidence details,
/// or <c>null</c> when no eligible pattern exists.
/// </returns>
public async Task<PatternResult?> GetHighestConfidencePatternAsync(
	EAIDocumentType documentType,
	CancellationToken cancellationToken)
{
	// Implementation
}
```

Similar pairs can be prepared for properties, constructors, and DTO parameters so the analyzer rules have concrete reference cases.

---

### Test-First Plan (TDD)

- Start each rule with failing analyzer tests that exercise the “bad documentation” snippets above. Example: `SummaryMustBeInformative_FlagsRepeatingIdentifier`.
- Follow with positive tests demonstrating allowed phrasing (e.g., summary with five descriptive words and verb diversity).
- Include boundary tests: three-word param descriptions (fail), four-word with added context (pass), cancellation remarks in `<remarks>` instead of `<param>`.
- No dedicated interface appears necessary; instead, helper classes (e.g., `DocumentationTriviaInspector`) can be introduced as `internal` types. If reuse across analyzers emerges, promote to a shared helper later.
- Tests should also cover generated-code suppression scenarios to prevent noise.

Proposed test structure for `IndFusion.Analyzers.DocQuality.Tests`:

- `SummaryMustBeInformativeTests`
- `ParamDescriptionMustAddValueTests`
- `ReturnDescriptionRequiredTests`
- `CancellationRemarkRequiredTests`
- `GeneratedCodeSuppressionTests`

---

### Implementation Blueprint

- Build reusable parsing helpers for `DocumentationCommentTriviaSyntax`, including:
	- `TryGetSummaryText(SyntaxNode node, out string text)`
	- `GetParameterBlocks(SyntaxNode node)` returning parameter name + text pairs.
- Leverage semantic model to normalize type names (e.g., distinguish `Task` vs `Task<T>`).
- String analysis utilities:
	- Tokenize on whitespace + punctuation, lower-case comparison.
	- Detect verbs via small curated list (`get`, `set`, `return`, `provide`, etc.) to guard against repetition in summaries.
- Consider static analysis caching per document to avoid reprocessing identical trivia.
- Introduce `DocQualityAnalyzerResources.resx` for diagnostic messages; ensures localization readiness.
- Optional: provide fixer scaffolding that inserts TODO text with placeholders for human edits, keeping implementation simple for first iteration.

These refinements should help us move quickly: write failing tests, implement the helper(s), flesh out analyzers, and iterate until the suite passes before integrating into the repo.
