## 🔨 Agent Task: Implement False-Positive Mitigations for Analyzer `EXXER{ID}`

### 🔧 Objective

You are assigned to implement enhancements to the analyzer `EXXER{ID}` based on the detailed mitigation blueprint provided in the corresponding specification file:

📄 `docs/specs/Analyzer{ID}Specs.md`
🔍 Analyzer source: `src/code/IndFusion.Analyzer/...{AnalyzerName}.cs`

Your goal is to reduce false positives while preserving true diagnostics by implementing the listed heuristics and safeguards, each verified by test coverage.

---

### 🥪 Implementation Steps

#### 1. Analyze the Spec

* Review all enhancement items under “Enhancement Opportunities” in the spec.
* Understand the context, rationale, and diagnostic patterns involved.

#### 2. Implement the Analyzer Enhancements

* For each mitigation (≥10 items):

  * Refactor the analyzer logic to detect the described pattern.
  * Integrate semantic analysis where required (e.g., override checks, base type detection, attribute presence).
  * Use caching and Roslyn APIs to maintain analyzer performance.

#### 3. Add Tests

* Create or update the test class:
  `src/test/IndFusion.Analyzer.Tests/{AnalyzerName}FalsePositiveTests.cs`
* For each enhancement:

  * Add at least one `[Fact]` test validating that the false positive no longer appears.
  * Use `AnalyzerTestHelper` + `Shouldly` assertions.
* Ensure control tests still fail when they should (true positives).

#### 4. Run and Validate

* Run:

  ```bash
  dotnet test src/test/IndFusion.Analyzer.Tests
  dotnet test "F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src"
  ```


---

### 📋 Final Checklist

* [ ] All enhancement cases implemented in the analyzer.
* [ ] At least 10 new test cases added and passing.
* [ ] Positive diagnostics (true violations) are still detected.
* [ ] Spec `Analyzer{ID}Specs.md` remains consistent with implementation.
* [ ] `AnalyzerReleases.Unshipped.md` updated summarizing the changes.
* [ ] `dotnet format` run to ensure style compliance.
* [ ] Solution and test projects build and pass cleanly.

---

### 🔁 Reusable Notes

This prompt applies to all analyzer on specs:, please keep here a register of the implemented Spec analyzer,

<Add a check list here>
