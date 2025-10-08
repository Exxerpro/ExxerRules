Refactor Logging to Structured Logging Format
🎯 Objective
Refactor all occurrences of interpolated or concatenated string-based logging calls (e.g., $"User {userId} not found" or "User " + userId + " not found") to structured logging using parameterized syntax supported by Serilog and Microsoft.Extensions.Logging.

Also look for bad exception logging practices in the codebase. for example


### 1. Redundant Exception.Message Logging

### 2. Missing Exception Parameter So you lose stack trace, inner exceptions, and all context.

### 3. Console.WriteLine(ex) Anti-PatternConsole.WriteLine(ex) Anti-Pattern

**4 Console.WriteLine(ex) Anti-Pattern

** 6 Throw New Exception, instread of simple Throw

Issue:

exceptions are being destructured manually or logged at inappropriate levels instead of using proper structured exception logging.

## CRITICAL The Exception must be pass ex as first parameter, the logging framework automatically capture

🧩 Scope
Project Logging Backend: Microsoft.Extensions.Logging + Serilog

Logger Variable: _logger or logger

Logging Levels in Use: LogInformation, LogDebug, LogError, LogWarning, LogCritical, etc.

🔍 Examples of Mistakes to Refactor
❌ _logger.LogInformation($"Processing file {fileName} at {DateTime.Now}")
❌  logger.LogInformation($"Processing file {fileName} at {DateTime.Now}")

❌ _logger.LogError("Error occurred at " + DateTime.UtcNow + ": " + ex.Message)
❌  logger.LogError("Error occurred at " + DateTime.UtcNow + ": " + ex.Message)

✅ Convert to Structured Logging
✅ _logger.LogInformation("Processing file {FileName} at {Timestamp}", fileName, DateTime.Now)
✅  logger.LogInformation("Processing file {FileName} at {Timestamp}", fileName, DateTime.Now)

✅ _logger.LogError(ex, "Error occurred at {Timestamp}", DateTime.UtcNow)
✅  logger.LogError(ex, "Error occurred at {Timestamp}", DateTime.UtcNow)

📋 Requirements
Only refactor logging calls using _logger or logger.

Do not modify business logic or method behavior.

Maintain exception logging structure, e.g., _logger.LogError(ex, "Descriptive message with {Data}", data).

All string interpolation and concatenation within logging calls must be removed.

Preserve semantic naming for all structured parameters.

Avoid creating structured logging for invariant strings. Only extract values that change per invocation.

Maintain code readability and clarity.

🧪 Validation
Verify all refactored logs compile.

Ensure application runs without logging-related runtime exceptions.
