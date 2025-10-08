// <copyright file="MockTestTagWriter.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Mocks;

using IndTrace.DataStore.IModelsComs;
using IndTrace.DataStore.Interfaces;

/// <summary>
/// Mock implementation of <see cref="ITestTagWriter"/> for dry-run and testing scenarios.
/// </summary>
public class MockTestTagWriter(ILogger<MockTestTagWriter> logger) : ITestTagWriter
{
    /// <summary>
    /// Simulates writing a cycle step asynchronously to the fixture context.
    /// </summary>
    /// <param name="context">The fixture context to write to.</param>
    /// <param name="step">The cycle step to write.</param>
    /// <returns>A completed task representing the asynchronous operation.</returns>
    public Task WriteCycleStepAsync(IFixtureContext context, CycleStep step)
    {
        logger.LogInformation("[DRY-RUN] WriteCycleStep -> MachineId: {MachineId}, Step: {Step}", context.PartNumber, step);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Simulates writing an invalid command asynchronously to the fixture context.
    /// </summary>
    /// <param name="context">The fixture context to write to.</param>
    /// <param name="step">The cycle step associated with the invalid command.</param>
    /// <returns>A completed task representing the asynchronous operation.</returns>
    public Task WriteInvalidCommandAsync(IFixtureContext context, CycleStep step)
    {
        logger.LogInformation("[DRY-RUN] WriteInvalidCommand -> MachineId: {MachineId}, Step: {Step}", context.PartNumber, step);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Simulates writing a faulty command and then recovering asynchronously in the fixture context.
    /// </summary>
    /// <param name="context">The fixture context to write to.</param>
    /// <returns>A completed task representing the asynchronous operation.</returns>
    public Task WriteFaultyThenRecoverAsync(IFixtureContext context)
    {
        logger.LogInformation("[DRY-RUN] WriteFaultyThenRecover -> MachineId: {MachineId}", context.PartNumber);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Simulates writing only the cycle start asynchronously to the fixture context.
    /// </summary>
    /// <param name="context">The fixture context to write to.</param>
    /// <returns>A completed task representing the asynchronous operation.</returns>
    public Task WriteCycleStartOnlyAsync(IFixtureContext context)
    {
        logger.LogInformation("[DRY-RUN] WriteCycleStartOnly -> MachineId: {MachineId}", context.PartNumber);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task WriteCycleStepAsync(IFixtureContext context, ICycleStep step)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task WriteInvalidCommandAsync(IFixtureContext context, ICycleStep step)
    {
        throw new NotImplementedException();
    }
}
