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

## 📊 Implementation Tracker

### ✅ Fully Implemented (Analyzer + Tests + Spec)
- [x] **EXXER001** - UseResultPattern Analyzer
  - Spec: `Analyzer001Specs.md` ✅
  - Implementation: `UseResultPatternAnalyzer.cs` ✅
  - Tests: `UseResultPatternAnalyzerFalsePositiveTests.cs` ✅

- [x] **EXXER002** - AvoidThrowingExceptions Analyzer
  - Spec: `Analyzer002Specs.md` ✅
  - Implementation: `AvoidThrowingExceptionsAnalyzer.cs` ✅
  - Tests: `AvoidThrowingExceptionsAnalyzerFalsePositiveTests.cs` ✅

- [x] **EXXER003** - DoNotThrowExceptions Analyzer
  - Spec: `Analyzer003Specs.md` ✅
  - Implementation: `DoNotThrowExceptionsAnalyzer.cs` ✅
  - Tests: `DoNotThrowExceptionsAnalyzerFalsePositiveTests.cs` ✅

- [x] **EXXER100** - TestNamingConvention Analyzer
  - Spec: `Analyzer100Specs.md` ✅
  - Implementation: `TestNamingConventionAnalyzer.cs` ✅
  - Tests: `TestNamingConventionAnalyzerFalsePositiveTests.cs` ✅

- [x] **EXXER101** - UseXUnitV3 Analyzer
  - Spec: `Analyzer101Specs.md` ✅
  - Implementation: `UseXUnitV3Analyzer.cs` ✅
  - Tests: `UseXUnitV3AnalyzerFalsePositiveTests.cs` ✅

- [x] **EXXER102** - UseShouldly Analyzer
  - Spec: `Analyzer102Specs.md` ✅
  - Implementation: `DoNotUseFluentAssertionsAnalyzer.cs` ✅
  - Tests: `UseShouldlyAnalyzerFalsePositiveTests.cs` ✅

### ✅ Fully Implemented (Analyzer + Tests + Spec)
- [x] **EXXER104** - DoNotMockDbContext Analyzer
  - Spec: `Analyzer104Specs.md` ✅
  - Implementation: `DoNotMockDbContextAnalyzer.cs` ✅
  - Tests: `DoNotMockDbContextAnalyzerFalsePositiveTests.cs` ✅

### ✅ Fully Implemented (Analyzer + Tests + Spec)
- [x] **EXXER200** - ValidateNullParameters Analyzer
  - Spec: `Analyzer200Specs.md` ✅
  - Implementation: `ValidateNullParametersAnalyzer.cs` ✅
  - Tests: `ValidateNullParametersAnalyzerFalsePositiveTests.cs` ✅

### 🔄 Partially Implemented (Analyzer + Spec, Missing Tests)

- [ ] **EXXER300** - AsyncMethodsShouldAcceptCancellationToken Analyzer
  - Spec: `Analyzer300Specs.md` ✅
  - Implementation: `AsyncMethodsShouldAcceptCancellationTokenAnalyzer.cs` ✅
  - Tests: ❌ Missing `AsyncMethodsShouldAcceptCancellationTokenAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER301** - UseConfigureAwaitFalse Analyzer
  - Spec: `Analyzer301Specs.md` ✅
  - Implementation: `UseConfigureAwaitFalseAnalyzer.cs` ✅
  - Tests: ❌ Missing `UseConfigureAwaitFalseAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER302** - AvoidAsyncVoid Analyzer
  - Spec: `Analyzer302Specs.md` ✅
  - Implementation: `AvoidAsyncVoidAnalyzer.cs` ✅
  - Tests: ❌ Missing `AvoidAsyncVoidAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER400** - PublicMembersShouldHaveXmlDocumentation Analyzer
  - Spec: `Analyzer400Specs.md` ✅
  - Implementation: `PublicMembersShouldHaveXmlDocumentationAnalyzer.cs` ✅
  - Tests: ❌ Missing `PublicMembersShouldHaveXmlDocumentationAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER500** - AvoidMagicNumbersAndStrings Analyzer
  - Spec: `Analyzer500Specs.md` ✅
  - Implementation: `AvoidMagicNumbersAndStringsAnalyzer.cs` ✅
  - Tests: ❌ Missing `AvoidMagicNumbersAndStringsAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER501** - UseExpressionBodiedMembers Analyzer
  - Spec: `Analyzer501Specs.md` ✅
  - Implementation: `UseExpressionBodiedMembersAnalyzer.cs` ✅
  - Tests: ❌ Missing `UseExpressionBodiedMembersAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER503** - DoNotUseRegions Analyzer
  - Spec: `Analyzer503Specs.md` ✅
  - Implementation: `DoNotUseRegionsAnalyzer.cs` ✅
  - Tests: ❌ Missing `DoNotUseRegionsAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER600** - DomainShouldNotReferenceInfrastructure Analyzer
  - Spec: `Analyzer600Specs.md` ✅
  - Implementation: `DomainShouldNotReferenceInfrastructureAnalyzer.cs` ✅
  - Tests: ❌ Missing `DomainShouldNotReferenceInfrastructureAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER601** - UseRepositoryPattern Analyzer
  - Spec: `Analyzer601Specs.md` ✅
  - Implementation: `UseRepositoryPatternAnalyzer.cs` ✅
  - Tests: ❌ Missing `UseRepositoryPatternAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER700** - UseEfficientLinq Analyzer
  - Spec: `Analyzer700Specs.md` ✅
  - Implementation: `UseEfficientLinqAnalyzer.cs` ✅
  - Tests: ❌ Missing `UseEfficientLinqAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER702** - UseModernPatternMatching Analyzer
  - Spec: `Analyzer702Specs.md` ✅
  - Implementation: `UseModernPatternMatchingAnalyzer.cs` ✅
  - Tests: ❌ Missing `UseModernPatternMatchingAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER800** - UseStructuredLogging Analyzer
  - Spec: `Analyzer800Specs.md` ✅
  - Implementation: `UseStructuredLoggingAnalyzer.cs` ✅
  - Tests: ❌ Missing `UseStructuredLoggingAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER801** - DoNotUseConsoleWriteLine Analyzer
  - Spec: `Analyzer801Specs.md` ✅
  - Implementation: `DoNotUseConsoleWriteLineAnalyzer.cs` ✅
  - Tests: ❌ Missing `DoNotUseConsoleWriteLineAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER900** - ProjectFormatting Analyzer
  - Spec: `Analyzer900Specs.md` ✅
  - Implementation: `ProjectFormattingAnalyzer.cs` ✅
  - Tests: ❌ Missing `ProjectFormattingAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER901** - CodeFormatting Analyzer
  - Spec: `Analyzer901Specs.md` ✅
  - Implementation: `CodeFormattingAnalyzer.cs` ✅
  - Tests: ❌ Missing `CodeFormattingAnalyzerFalsePositiveTests.cs`

### ❌ Not Implemented (Missing Implementation)
- [ ] **EXXER103** - UseNSubstitute Analyzer
  - Spec: ❌ Missing `Analyzer103Specs.md`
  - Implementation: `DoNotUseMoqAnalyzer.cs` ✅ (Partial - needs enhancement)
  - Tests: ❌ Missing `UseNSubstituteAnalyzerFalsePositiveTests.cs`

- [ ] **EXXER201** - UseNullSafetyPatterns Analyzer
  - Spec: ❌ Missing `Analyzer201Specs.md`
  - Implementation: ❌ Missing
  - Tests: ❌ Missing

- [ ] **EXXER502** - PrivateFieldNaming Analyzer
  - Spec: ❌ Missing `Analyzer502Specs.md`
  - Implementation: ❌ Missing
  - Tests: ❌ Missing

- [ ] **EXXER701** - DisposeResourcesProperly Analyzer
  - Spec: ❌ Missing `Analyzer701Specs.md`
  - Implementation: ❌ Missing
  - Tests: ❌ Missing

### 📈 Implementation Statistics
- **Total Specs**: 23
- **Fully Implemented**: 8 (35%)
- **Partially Implemented**: 14 (61%)
- **Not Implemented**: 4 (17%)
- **Total Analyzers**: 27 (including missing specs)

### 🎯 Next Priority Actions
1. **High Priority**: Complete test coverage for partially implemented analyzers
2. **Medium Priority**: Create missing specs for unimplemented analyzers
3. **Low Priority**: Enhance existing implementations based on spec requirements
