### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
EXXER200 | IndFusion.Analyzers.NullSafety | Warning | Validate null parameters at method entry
EXXER300 | IndFusion.Analyzers.Async | Info | Async methods should accept CancellationToken
EXXER400 | IndFusion.Analyzers.Documentation | Info | Public members should have XML documentation
EXXER500 | IndFusion.Analyzers.CodeQuality | Warning | Avoid magic numbers and strings
EXXER503 | IndFusion.Analyzers.CodeQuality | Warning | Do not use regions for code organization
EXXER800 | IndFusion.Analyzers.Logging | Warning | Use structured logging instead of string concatenation
EXXER801 | IndFusion.Analyzers.Logging | Warning | Do not use Console.WriteLine in production code
EXXER003 | IndFusion.Analyzers.FunctionalPatterns | Error | Do not throw exceptions - use Result&lt;T&gt; pattern instead
EXXER301 | IndFusion.Analyzers.Async | Warning | Use ConfigureAwait(false) in library code
EXXER302 | IndFusion.Analyzers.Async | Warning | Avoid async void methods except event handlers
EXXER501 | IndFusion.Analyzers.CodeQuality | Info | Use expression-bodied members where appropriate
EXXER600 | IndFusion.Analyzers.Architecture | Error | Domain layer should not reference Infrastructure layer
EXXER601 | IndFusion.Analyzers.Architecture | Warning | Use Repository pattern with focused interfaces
EXXER700 | IndFusion.Analyzers.Performance | Warning | Use efficient LINQ operations
EXXER702 | IndFusion.Analyzers.CodeQuality | Info | Use modern pattern matching with declaration patterns
EXXER900 | IndFusion.Analyzers.CodeQuality | Hidden | Format project using dotnet format command
EXXER901 | IndFusion.Analyzers.CodeQuality | Info | Code formatting inconsistency detected
EXXER003 | IndFusion.Analyzers.FunctionalPatterns | Error | DoNotThrowExceptionsAnalyzer updated to recognise guard patterns (null/range), switch default-throws, domain parser exceptions, constructor invariants, factory validation, expression-bodied guards, defensive NotSupported/NotImplemented, and catch-wrapping; boundary/test contexts ignored; reduced false positives.

### Changes

Rule ID | Notes
--------|------
EXXER100 | Relaxed test naming: allow prefixes before Should, alternate connectors (When/For/With/If/On), behavior-only names, compound conditions, async suffixes, nested context classes, DisplayName/Description overrides, and opt-out attribute.
