# Task: Migrate Static Regex to Source-Generated `[GeneratedRegex]`

## 🎯 Objective

Modernize regex handling by replacing static `Regex` fields with .NET source-generated `[GeneratedRegex]` patterns. This enhances startup performance and provides compile-time validation.

## 📌 Context: Target Class

The class `PlcDbTagsRepository` defines five static regex fields used to validate S7 tag formats.

### Example Fields to Replace

```csharp
private static readonly Regex DbStringFormatRegex = new(@"^DB\d+\.(S|STRING)\d+\.\d+$", RegexOptions.Compiled);
private static readonly Regex DbInt16FormatRegex = new(@"^DB\d+\.(DBW\d+|W\d+)$", RegexOptions.Compiled);
private static readonly Regex DbInt32FormatRegex = new(@"^DB\d+\.(DINT\d+|DBD\d+)$", RegexOptions.Compiled);
private static readonly Regex DbBoolFormatRegex = new(@"^DB\d+\.X\d+\.[0-7]$", RegexOptions.Compiled);
private static readonly Regex DbInt32OrRealFormatRegex = new(@"^DB\d+\.(DINT\d+|DBD\d+|D\d+|REAL\d+|R\d+)$", RegexOptions.Compiled);
```

## 🛠 Refactoring Instructions

### 🔁 Replace Each Static Field

Create a new file: `Regex/S7Regexes.cs`

```csharp
using System.Text.RegularExpressions;

namespace IndTrace.DataStore.Regex;

public static partial class S7Regexes
{
    [GeneratedRegex(@"^DB\d+\.(S|STRING)\d+\.\d+$", RegexOptions.Compiled)]
    public static partial Regex DbStringFormatRegex();

    [GeneratedRegex(@"^DB\d+\.(DBW\d+|W\d+)$", RegexOptions.Compiled)]
    public static partial Regex DbInt16FormatRegex();

    [GeneratedRegex(@"^DB\d+\.(DINT\d+|DBD\d+)$", RegexOptions.Compiled)]
    public static partial Regex DbInt32FormatRegex();

    [GeneratedRegex(@"^DB\d+\.X\d+\.[0-7]$", RegexOptions.Compiled)]
    public static partial Regex DbBoolFormatRegex();

    [GeneratedRegex(@"^DB\d+\.(DINT\d+|DBD\d+|D\d+|REAL\d+|R\d+)$", RegexOptions.Compiled)]
    public static partial Regex DbInt32OrRealFormatRegex();
}
```

### 🔍 Update Call Sites

Update `IsValidFormat` in `PlcDbTagsRepository.cs` to use:

```csharp
private static bool IsValidFormat(string address, Type type) =>
    type switch
    {
        _ when type == typeof(string) => S7Regexes.DbStringFormatRegex().IsMatch(address),
        _ when type == typeof(short) => S7Regexes.DbInt16FormatRegex().IsMatch(address),
        _ when type == typeof(int) => S7Regexes.DbInt32FormatRegex().IsMatch(address),
        _ when type == typeof(bool) => S7Regexes.DbBoolFormatRegex().IsMatch(address),
        _ when type == typeof(double) => S7Regexes.DbInt32OrRealFormatRegex().IsMatch(address),
        _ => true
    };
```

Delete the old static regex declarations.

## ✅ Deliverables

- `S7Regexes.cs` in a central shared location
- Refactored `PlcDbTagsRepository.cs` using new source-generated patterns
- Confirmed no behavior regressions using existing test coverage
- (Optional) Unit test file: `S7RegexesTests.cs`

## 📄 Unit Test Sample

```csharp
public class S7RegexesTests
{
    [Theory]
    [InlineData("DB1.STRING1.0", true)]
    [InlineData("DB3.S2.5", true)]
    [InlineData("DB3.X12.3", true)]
    [InlineData("DB3.DBD4", true)]
    [InlineData("DB3.ABCD4", false)]
    public void RegexPatterns_ShouldMatchExpected(string input, bool expected)
    {
        S7Regexes.DbStringFormatRegex().IsMatch(input).ShouldBe(expected);
        S7Regexes.DbBoolFormatRegex().IsMatch(input).ShouldBe(expected);
    }
}
```

## 🧠 Final Instruction to Agent

- Confirm understanding
- Scan the class and replace eligible regex patterns
- Proceed autonomously
