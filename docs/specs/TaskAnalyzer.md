🧠 Task: Deep Review & Spec Creation for a Missing Analyzer Spec in IndFusion
📦 Context & Project Structure

The IndFusion solution includes:

Analyzer projects: IndFusion.Analyzer
Fixer projects: IndFusion.Fixer

Test projects: reference both analyzer and fixer assemblies

Analyzer source code: ExxerRules/src within the IndFusion solution

Tests solution  that exercise analyzers:
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\TestProject\Src
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\TestProject\Src\Code\Core\Application
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\TestProject\Src\Code\Core\Domain
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\TestProject\Src\Tests\Core\Domain.nitTests
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\TestProject\Src\Tests\Core\Domain.UnitTests
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\TestProject\Src\Tests\Core\Application.UnitTests


Existing analyzer specifications:
docs/specs/Analyzer{number}Specs.md

?? Search Boundaries

Limit all repository searches and file scans to the following roots only:

- docs/specs
- src/code/IndFusion.Mcp.Core/
- src/test/IndFusion.Analyzer.Tests/
- TestProject\Src\Code\Core\Application
- TestProject\Src\Code\Core\Domain
- TestProject\Src\Tests\Core\Domain.UnitTests
- TestProject\Src\Tests\Core\Domain.UnitTests
- TestProject\Src\Tests\Core\Application.UnitTests- 

-  Assignment Steps
Pre-Step: Create a Git Commit Checkpoint

Before beginning any analysis, run `git status` and commit the current workspace state so the task starts from a clean baseline.

Step 1: Select the Lowest-Numbered Analyzer Missing an Aspect Spec (Mandatory Gate)

Scan the directory docs/specs/ for files matching the pattern Analyzer{number}Specs.md and collect analyzer numbers without corresponding aspect/spec documentation.

Choose the analyzer with the lowest numeric identifier from that list, confirming there is no aspect file for it elsewhere in the repository.

Only proceed with this analyzer unless a lower-numbered analyzer gains an aspect/spec file during the task.

Step 2: Investigate the Chosen Analyzer for Faults

For the selected analyzer:

Investigate test failures, assertion mismatches, or diagnostic outputs in the test directory:
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src
for the projects on 
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Code\Core\Application
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Code\Core\Domain
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Tests\Core\Domain.UnitTestsF:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Tests\Core\Domain.UnitTests
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Tests\Core\Application.UnitTests

Focus all searches on test assets within src/test/IndFusion.Mcp.Core.Tests/ and src/test/IndFusion.Mcp.Server.Tests/.
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Code\Core\Application
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Code\Core\Domain
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Tests\Core\Domain.UnitTestsF:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Tests\Core\Domain.UnitTests
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\Test Project\Src\Tests\Core\Application.UnitTests


Document the most significant false positives affecting this analyzer.

Capture supporting evidence: e.g., test names, line numbers, assertion logs.

Before moving on, re-confirm that the analyzer still lacks an aspect/spec file after your investigation.

If the analyzer produces no reproducible false positives within these directories, log that outcome briefly and advance to the next lowest-numbered analyzer without an aspect/spec file.


🔍 Analysis and Spec Documentation
Step 3: Perform a Deep Source Review

Conduct a full, uninterrupted review of:

The analyzer’s logic and rules

Its associated test coverage and assertions

Step 4: Author docs/specs/Analyzer{number}Specs.md

Your Markdown spec file must include:

📘 Specification Section

Intent – What is the rule’s purpose? What violations is it detecting?

Scope – Where does it apply (syntax nodes, code constructs)?

Validation Plan – How is correctness confirmed? (unit tests, coverage, regression assertions)

🛠️ Enhancement Opportunities (Minimum 10)

For each of the 10 entries:

Describe a false-positive that occurs under current implementation

Propose a precise fix, with test-driven reasoning

Provide a unit test snippet (XUnit + Shouldly) that:

Triggers the false-positive now

Would pass after the fix

🔬 Test-Driven Fix Strategy

For each of the 10 enhancements:

Identify the existing or new test that fails

Explain how the analyzer logic should be changed

Include clear, scoped code examples:

Test method

Expected assertion

Input code sample (before fix)

✅ Deliverables

One file: docs/specs/Analyzer{number}Specs.md

It must contain:

 Full intent/scope/validation plan

 10 enhancement opportunities

 Concrete test fixes with matching test snippets and assertions

✔️ Acceptance Criteria

The analyzer is chosen correctly: it is the lowest-numbered analyzer without an aspect/spec file

If an analyzer yields no substantiated false positives after focused review, document the finding and proceed to the next eligible analyzer.

The spec document:

Accurately describes analyzer intent, scope, and validation

Contains ≥ 10 concrete, testable improvements

Each improvement includes working test examples

The Markdown file is syntactically correct, logically structured, and ready to be committed to docs/specs/
