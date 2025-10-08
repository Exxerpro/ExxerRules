---
description:
globs:
alwaysApply: false
---
description: Test Data Loading
# Functional Refactor Plan: Test Data Loading and Repository Pattern Modernization

## 1. **Background & Motivation**

The current testbed for the IndTrace industrial traceability application leverages a repository pattern with a factory "cache" and context pooling. Test data is loaded from JSON files (some very large) for unit-of-work and repository tests. The architecture is highly performant in production, using no-tracking contexts and optimized indexes, but the testbed faces IO bottlenecks and maintainability challenges as test data grows.

## 2. **Current State**

- **Repository Pattern:**  
  - Each repository requests a context from a factory/pool.
  - Two repository types: read-only and read-write.
  - Contexts are registered as no-tracking by default for performance.
  - Tests use in-memory EF Core contexts.

- **Test Data Loading:**  
  - Test data is loaded from JSON files in the `SeedDataFiles` directory.
  - `TestDataLoader` uses raw string literals for small files and memory-mapped files for large files.
  - Data is loaded on each repository call, leading to repeated IO for large files.
  - `DataFileLocator` robustly resolves file paths across environments.

- **Test Coverage:**  
  - Tests are not full integration tests, but unit-of-work/request tests.
  - Some tests require only a subset of data, but all data is loaded.
  - Helpers exist for deduplication and data management.

## 3. **Identified Issues**

- **Performance:**  
  - Repeated loading of large JSON files causes significant IO overhead.
  - Test runs are slower than necessary, especially as data grows.

- **Maintainability:**  
  - Adding new test cases requires updating or creating large JSON files.
  - Difficult to track which data is actually used in tests.

- **Extensibility:**  
  - No easy way to switch between static (hardcoded) and dynamic (JSON) data sources.
  - Edge cases and new scenarios require manual data management.

## 4. **Refactor Goals**

- **Reduce IO and speed up tests** by minimizing repeated large file loads.
- **Track actual test data usage** to generate static lists for common/edge cases.
- **Enable a strategy pattern** to switch between static, JSON, or hybrid data sources.
- **Deduplicate data** when mixing static and file-based sources.
- **Maintain flexibility** for adding new test cases and edge scenarios.

## 5. **Recommendations & Enhancements**

- **Usage Tracking:**  
  - Extend or implement a `TestDataUsageTracker` to log which objects/IDs are used in tests.
  - Output usage logs for source generation.

- **Source Generator:**  
  - Automate creation of static C# lists for most-used or edge-case objects.
  - Place these in a partial class or static helper for fast, allocation-free access.

- **Strategy Pattern for Data Loading:**  
  - Refactor `TestDataLoader` to accept a strategy (static, JSON, hybrid).
  - Default to static for speed, fall back to JSON for new/edge cases.

- **Deduplication:**  
  - Ensure IDs/keys are unique across static and file-based sources in hybrid mode.

- **Performance Monitoring:**  
  - Use cache statistics and custom logging to monitor test data load times and cache hits.

- **Extensibility:**  
  - Allow new test cases by adding JSON files or updating static lists.

## 6. **Next Steps**

1. **Track Test Data Usage**
   - Instrument repositories or loaders to log which data is accessed during tests.
   - Store logs for analysis and source generation.

2. **Implement Source Generator**
   - Create a tool to convert usage logs into static C# lists.
   - Integrate generated lists into the testbed.

3. **Refactor TestDataLoader**
   - Introduce a strategy pattern to select between static, JSON, or hybrid data sources.
   - Implement deduplication logic for hybrid mode.

4. **Update Test Infrastructure**
   - Refactor tests to use the new loader and strategies.
   - Document how to add new test cases and update static data.

5. **Monitor and Optimize**
   - Use performance metrics to validate improvements.
   - Iterate on the approach as new requirements or edge cases arise.

## 7. **How I Can Help**

- **Design and implement the strategy pattern for `TestDataLoader`.**
- **Draft and integrate a source generator for static test data.**
- **Refactor and document the new test data management workflow.**
- **Provide code samples, templates, and best practices for each step.**
- **Assist in incremental migration and validation of the new approach.**

---


**This plan ensures your testbed remains fast, maintainable, and extensible as your domain and data grow, while supporting both legacy and future test scenarios.**
