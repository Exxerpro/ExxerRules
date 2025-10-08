All set—here’s the fully reviewed Task Specification with a polished closing. Let me know if you’d like any further tweaks:

---

## Task: Expand and Fortify Unit-Test Coverage

### 1. **Context & Objective**

You already have a working production-grade test bed for our .NET system, but several classes lack any unit tests and edge-case behavior is not yet verified. Although existing tests pass against current behavior, we want broader, meaningful assertions to guard against regressions—**without** refactoring production code or rewriting tests in TDD style.

### 2. **Scope of Work**

* **New Tests**
  * Identify classes and public methods with **zero** or **insufficient** coverage.
  * Cover critical edge cases (null inputs, boundary values, error paths, external-dependency failures).
* **Stabilize Broken Tests**
  * Locate tests that currently fail.
  * Modify *only the test code* so that they accurately assert the *intended* behavior (as seen in production logs/behavior).
* **Do Not**
  * Touch or refactor any code under `src/`.
  * Modify any existing tests that already **pass** .
  * Write full TDD-style “red → green → refactor” cycles—focus on black-box, behavior-driven test additions.

### 3. **Tools & Frameworks**

* **Test Runner:** xUnit v3
* **Assertion Library:** Shouldly
* **Mocking:** NSubstitute
* **Logging (in-test capture):** Meziantou.Extensions.Logging.Xunit.v3
* **Coverage Report:** (Coverlet + ReportGenerator) – aim for ≥ 80 % on newly tested classes.

### 4. **Technical Requirements**

1. **Test Structure**
   * Mirror production namespaces under `ProjectName.Tests/`.
   * Name test classes `FooServiceTests` for `FooService`, and methods `MethodName_StateUnderTest_ExpectedBehavior()`.
2. **Mocks & Stubs**
   * Use NSubstitute for all external-dependency interfaces.
   * **Application.UnitTest** : Mock repositories with sample data from `/SeedDataFiles`.
   * **Aggregation.BoundedTest** : Use an in-memory database seeded with real data.
   * Verify side-effects or exception flows via substitute call counts (`Received()`, `DidNotReceive()`).
3. **Assertions**
   * Use Shouldly’s fluent syntax (`result.ShouldBe(...)`, `action.ShouldThrow<...>()`).
4. **Logging**
   * Capture and assert on `ILogger` events via Meziantou.Extensions.Logging.Xunit.
5. **Edge Cases & Error Paths**
   * Null/empty inputs → `ArgumentNullException` or documented behavior.
   * Boundary values (min/max, zero, negatives).
   * Injected-dependency failures → verify exception wrapping or fallback logic.

### 5. **Deliverables**

1. New test files under `ProjectName.Tests/`.
2. All existing tests still green under `dotnet test`.
3. Coverage report summary showing improved coverage.
4. “Test Journal” (Markdown) listing:
   * Classes tested
   * Edge cases covered
   * Tests “fixed” with explanations of intended behavior

### 6. **Constraints & Best Practices**

* **No Production Code Changes:** All work lives in test projects.
* **No Passing-Test Changes:** Preserve existing green tests verbatim.
* **Meaningful Assertions:** Each new/fixed test must verify a concrete behavior.
* **Readability & Maintainability:**
  * One behavior per test
  * SOLID principles in test code
  * Helper methods only to remove duplication without hiding intent

---

**Next Steps for the Agent (“Teller”):**

1. Scan for untested/under-tested classes.
2. Draft tests per sections 2–4.
3. Run `dotnet test` to ensure zero regressions.
4. Generate coverage report and “Test Journal.”

**Please confirm your understanding of these instructions, scan the project and existing test bed, outline your plan of action, and acknowledge that you’ll proceed autonomously.**
