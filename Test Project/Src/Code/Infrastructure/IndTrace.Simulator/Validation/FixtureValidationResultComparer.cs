// <copyright file="FixtureValidationResultComparer.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Validation;

using System.Text.Json;
using IndQuestResults;
using IndTrace.DataStore.Interfaces;

public static class FixtureValidationResultComparer
{
    public static Result CompareAndLogDifferences(
        ILogger logger,
        IFixtureValidationResult expected,
        IFixtureValidationResult actual,
        string expectedName,
        string actualName, FixtureContext context)
    {
        if (expected.Equals(actual))
        {
            logger.LogInformation("FixtureValidation ✅ {expectedName} vs {actualName} passed equality check.", expectedName, actualName);
            return Result.Success();
        }

        var errors = Compare(expected, actual, expectedName, actualName, context).ToList();

        logger.LogWarning("FixtureValidation ⚠️ {expectedName} vs {actualName} failed equality check with {Count} differences.", expectedName, actualName, errors.Count);
        foreach (var diff in errors)
        {
            logger.LogWarning("🔍 {Diff}", diff);
        }

        logger.LogDebug("FixtureValidation 📋 Expected ({expectedName}): {Expected}", expectedName, JsonSerializer.Serialize(expected));
        logger.LogDebug("FixtureValidation 📦 Actual ({actualName}): {Actual}", actualName, JsonSerializer.Serialize(actual));

        var result = Result.WithFailure($"FixtureValidation Validation failed for {expectedName} vs {actualName}");

        return result;
    }

    public static IEnumerable<string> Compare(
        IFixtureValidationResult expected,
        IFixtureValidationResult actual,
        string expectedName,
        string actualName, FixtureContext context)
    {
        if (expected == null)
        {
            yield return $"{expectedName}: expected is null.";
            yield break;
        }

        if (actual == null)
        {
            yield return $"{expectedName}: actual is null.";
            yield break;
        }

        foreach (var result in CompareField(expectedName, actualName, nameof(expected.Barcode), expected.Barcode, actual.Barcode, true, context))
        {
            yield return result;
        }

        foreach (var result in CompareField(expectedName, actualName, nameof(expected.CycleStatus), expected.CycleStatus, actual.CycleStatus, true, context))
        {
            yield return result;
        }

        foreach (var result in CompareField(expectedName, actualName, nameof(expected.PartStatus), expected.PartStatus, actual.PartStatus, true, context))
        {
            yield return result;
        }

        foreach (var result in CompareField(expectedName, actualName, nameof(expected.FlowStatus), expected.FlowStatus, actual.FlowStatus, true, context))
        {
            yield return result;
        }

        foreach (var result in CompareField(expectedName, actualName, nameof(expected.NextMachineId), expected.NextMachineId, actual.NextMachineId, true, context))
        {
            yield return result;
        }

        foreach (var result in CompareField(expectedName, actualName, nameof(expected.ActualStation), expected.ActualStation, actual.ActualStation, true, context))
        {
            yield return result;
        }
    }

    private static IEnumerable<string> CompareField<T>(string expectedName, string actualName, string fieldName,
        T expected, T actual, bool ignoreCase, FixtureContext context)
    {
        if (expected == null && actual == null)
        {
            yield break;
        }

        bool isEqual = ignoreCase && expected is string s1 && actual is string s2
            ? string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase)
            : Equals(expected, actual);

        if (!isEqual)
        {
            yield return $"{expectedName}: {fieldName} mismatch → Expected {expectedName}: {expected}, Actual {actualName}: {actual} context {context.ToString()}";
        }
    }
}
