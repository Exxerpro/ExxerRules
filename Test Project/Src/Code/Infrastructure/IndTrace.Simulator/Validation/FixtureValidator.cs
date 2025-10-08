// <copyright file="FixtureValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Validation;

using IndQuestResults;
using IndTrace.DataStore.IModelsComs;
using IndTrace.DataStore.Interfaces;
using IndTrace.Simulator.Comms;

/// <summary>
/// Represents the FixtureValidator.
/// </summary>
public class FixtureValidator(ILogger<FixtureValidator> logger,
    IFixtureDb db,
    IMachineStateEvaluator evaluator,
    ITagsRepository tagsRepository,
    IPlcClient plc) : IFixtureValidator
{
    // Mark intentionally unused dependency to satisfy CS9113 without behavior change (future use expected)
    private readonly bool tagsRepoProvided = tagsRepository is not null;

    public async Task<Result> ValidatePostExecutionStateAsync(FixtureContext context)
    {
        var validationContext = await this.BuildContextAsync(context);

        if (validationContext == null)
        {
            logger.LogError("❌ Barcode {Barcode} not found in database.", context.Barcode);
            return Result.WithFailure($"❌ Barcode {context.Barcode} not found in database.");
        }

        var plcDB = FixtureValidationResultComparer.CompareAndLogDifferences(logger, validationContext.PlcResults, validationContext.DbResults, "PLC", "DB", context);
        var plcResult = FixtureValidationResultComparer.CompareAndLogDifferences(logger, validationContext.PlcResults, validationContext.ExpectedResults, "PLC", "TASK", context);
        var dbResult = FixtureValidationResultComparer.CompareAndLogDifferences(logger, validationContext.DbResults, validationContext.ExpectedResults, "DB", "TASK", context);

        logger.LogInformation(
            plcDB.IsSuccess
                ? "PLC vs DB Results ✅ Barcode {Barcode} result from PLC vs DB fully validated."
                : "PLC vs DB Results ❌ Barcode {Barcode} result from PLC vs DB validation failed.", context.Barcode);

        logger.LogInformation(
            dbResult.IsSuccess
                ? "DB Results ✅ Barcode {Barcode} result from DB fully validated."
                : "DB Results ❌ Barcode {Barcode} result from DB validation failed.", context.Barcode);

        logger.LogInformation(
            plcResult.IsSuccess
                ? "PLC Results ✅ Barcode {Barcode} result from PLC fully validated."
                : "PLC Results ❌ Barcode {Barcode} result from PLC validation failed.", context.Barcode);

        // Combine multiple results manually since IndQuestResults doesn't have Merge
        if (plcDB.IsSuccess && plcResult.IsSuccess && dbResult.IsSuccess)
        {
            return Result.Success();
        }

        // Collect all error messages
        var allErrors = new List<string>();
        if (!plcDB.IsSuccess) allErrors.AddRange(plcDB.Errors);
        if (!plcResult.IsSuccess) allErrors.AddRange(plcResult.Errors);
        if (!dbResult.IsSuccess) allErrors.AddRange(dbResult.Errors);

        return Result.WithFailure(string.Join("; ", allErrors));
    }

    public async Task<FixtureValidationContext> BuildContextAsync(FixtureContext context)
    {
        // Use the tags from thd DBTag Mapper refactor to get on this format:
        var plcTags = await plc.ReadListIntTagsAsync<int>(context.Tags, context.MachineId);

        var plcResults = FixturePlcSnapshot.FromTagsValues(plcTags, context);

        // Step 1: Get raw DB record (DTO only)
        var barcodeRecord = await db.LoadBarcodeStateAsync(context.Barcode);

        // Step 2: Get static configuration
        var staticData = await db.LoadStaticSnapshotByPartNumberAsync(context.PartNumber, context.MachineId);

        // Step 3: Compose enriched DB snapshot
        var dbDataResults = FixtureDbSnapshot.FromBarcodeAndStatic(barcodeRecord, staticData, context);

        // Step 4: Build expected state from current state and static data
        var expectedResults = evaluator.BuildNextExpectedState(context, staticData);

        return new FixtureValidationContext
        {
            PlcResults = plcResults,
            DbResults = dbDataResults,
            ConfigDataSnapShot = staticData,
            ExpectedResults = expectedResults,
        };
    }

    public Task<Result> ValidatePostExecutionStateAsync(IFixtureContext context)
    {
        throw new NotImplementedException();
    }
}
