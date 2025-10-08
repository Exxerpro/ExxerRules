---
description: C#.NET Expert Programmer and XML refactor
globs:
alwaysApply: false
---

This is your task and you must no stop until completee.

Please scan the current folder and ensure that all **public classes**, **public methods**, and **public properties** have appropriate XML documentation.  

Follow these constraints:
- DO NOT modify any existing code logic or structure.
- DO NOT overwrite valid existing XML comments.
- If a public member lacks XML comments, add `<summary>`, `<param>`, and `<returns>` tags as appropriate.
- Use meaningful descriptions that accurately reflect the member’s purpose.
- If no changes are needed, you may proceed silently without further confirmation.

**Production code must not change, no properties, types, names, methods, any kind of behavior, only add coments**

- **Do not modify any code structure or behavior** under any circumstance.
- **Update XML comments only when they are outdated or incomplet**
- If documentation is missing, **insert XML comments only**.
- If no changes were required, **proceed silently without prompting for confirmation**.
- Add `<summary>` XML comments to all **public classes**.
- Add `<summary>` XML comments to all **public methods and properties** within public classes.
- Use `<param name="...">` for each parameter in methods.
- Use `<returns>` for methods that return values.
- Use meaningful and concise descriptions to summarize intent and behavior.
- Follow standard .NET XML documentation conventions.
- Apply this rule uniformly across all `.cs` files in the solution.
- Don't Modify any code
- Please proceed without further notification.

**All the comments must be meaninfull and add value think of the consumer of the code**

- Examples

/// <summary>
/// Represents the result of an operation, including success status and error messages.
/// </summary>
public class Result
{

/// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/> instance.</returns>
    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }


/// <summary>
/// Gets a value indicating whether the result is a failure.
/// </summary>
public bool IsFailure => !IsSuccess;

/// <summary>
    /// Creates a failed result with the specified errors.
    /// </summary>
    /// <param name="errors">The collection of error messages.</param>
    /// <returns>A failed <see cref="Result"/> instance.</returns>
    public static Result WithFailure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }

}
