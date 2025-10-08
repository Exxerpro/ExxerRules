// <copyright file="TestPathRunner.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Simulation
{
    using IndQuestResults;
    using IndTrace.DataStore.IModelsComs;
    using IndTrace.DataStore.Interfaces;
    using IndTrace.DataStore.Models;
    using IndTrace.DataStore.ModelsComs;
    using IndTrace.Simulator.Comms;
    using IndTrace.Simulator.Export;
    using IndTrace.Simulator.Models.Constants;
    using IndTrace.Simulator.Validation;

    /// <summary>
    /// Executes test paths for fixture simulation, including full success, failure, retry, and incomplete cycle scenarios.
    /// </summary>
    /// <remarks>
    /// Provides methods to run and validate different test path scenarios for a fixture, supporting barcode generation, retry logic, and result export.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TestPathRunner"/> class.
    /// </remarks>
    /// <param name="store">The fixture store for saving results.</param>
    /// <param name="plc">The PLC client for command simulation.</param>
    /// <param name="tagWriter">The tag writer for writing test tags.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="validator">The fixture validator for post-execution validation.</param>
    /// <param name="exporter">The fixture exporter for exporting results.</param>
    /// <param name="machineResolver">The machine resolver for resolving machine sequences.</param>
    /// <param name="tagMapper">The tags repository for retrieving tags.</param>
    public class TestPathRunner(
        IFixtureStore store,
        IPlcClient plc,
        ITestTagWriter tagWriter,
        ILogger<TestPathRunner> logger,
        IFixtureValidator validator,
        IFixtureExporter exporter,
        IMachineResolver machineResolver,
        ITagsRepository tagMapper) : ITestPathRunner
    {
        private string lastCode = string.Empty;

        /// <summary>
        /// Generates a barcode for the given part number and index, or returns the last code if available.
        /// </summary>
        /// <param name="partNumber">The part number.</param>
        /// <param name="index">The sequence index.</param>
        /// <returns>The generated barcode string.</returns>
        private string GenerateBarcode(string partNumber, int index)
        {
            return string.IsNullOrEmpty(this.lastCode) ? $"{partNumber}-{index:0000}" : this.lastCode;
        }

        /// <inheritdoc/>
        public async Task RunAsync(FixtureContext context, TestPathType pathType)
        {
            switch (pathType)
            {
                case TestPathType.FullSuccess:
                    await this.RunFullSuccessAsync(context);
                    break;

                case TestPathType.FullFailure:
                    await this.RunFullFailureAsync(context);
                    break;

                case TestPathType.MidwayFailure:
                    await this.RunMidwayFailureAsync(context);
                    break;

                case TestPathType.RetryLoop:
                    await this.RunRetryLoopAsync(context);
                    break;

                case TestPathType.IncompleteCycle:
                    await this.RunIncompleteCycleAsync(context);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }

            await validator.ValidatePostExecutionStateAsync(context);
            await exporter.ExportAsync(context);
        }

        /// <summary>
        /// Simulates a midway failure in the test path by injecting an invalid command at the halfway point.
        /// </summary>
        /// <param name="context">The fixture context to simulate.</param>
        private async Task RunMidwayFailureAsync(FixtureContext context)
        {
            var halfway = context.Tasks.Count / 2;
            foreach (var i in Enumerable.Range(0, halfway))
            {
                await tagWriter.WriteCycleStepAsync(context, context.Tasks[i]);
            }

            await tagWriter.WriteInvalidCommandAsync(context, context.Tasks[halfway]);
            logger.LogWarning("Midway failure injected on task {TaskName}", context.Tasks[halfway].Name);
            await store.SaveAsync(context, "MidwayFailure");
        }

        /// <summary>
        /// Simulates a retry loop scenario, retrying until success or maximum retries are reached.
        /// </summary>
        /// <param name="context">The fixture context to simulate.</param>
        private async Task RunRetryLoopAsync(FixtureContext context)
        {
            var profile = FixtureProfile.Load();
            var maxRetries = profile.MaxRetries;
            var succeeded = false;
            var retryCount = 0;
            while (!succeeded && retryCount < maxRetries)
            {
                try
                {
                    await tagWriter.WriteFaultyThenRecoverAsync(context);
                    succeeded = true;
                    logger.LogInformation("Retry succeeded on attempt {RetryCount}", retryCount);
                }
                catch (Exception ex)
                {
                    retryCount++;
                    logger.LogError(ex, "Retry {RetryCount} failed.", retryCount);
                    await Task.Delay(profile.DelayMs);
                }
            }

            await store.SaveAsync(context, "RetryLoop", retryCount);
        }

        /// <summary>
        /// Simulates an incomplete cycle scenario by only writing the cycle start.
        /// </summary>
        /// <param name="context">The fixture context to simulate.</param>
        private async Task RunIncompleteCycleAsync(FixtureContext context)
        {
            await tagWriter.WriteCycleStartOnlyAsync(context);
            logger.LogWarning("Incomplete cycle left active for barcode {Barcode}", context.Barcode);
            await store.SaveAsync(context, "IncompleteCycle");
        }

        /// <summary>
        /// Simulates a full success scenario by executing all steps successfully.
        /// </summary>
        /// <param name="context">The fixture context to simulate.</param>
        private async Task RunFullSuccessAsync(FixtureContext context)
        {
            foreach (var step in context.Tasks)
            {
                await tagWriter.WriteCycleStepAsync(context, step);
            }

            logger.LogInformation("Full success path completed for barcode {Barcode}", context.Barcode);
            await store.SaveAsync(context, "FullSuccess");
        }

        /// <summary>
        /// Simulates a full failure scenario by executing all steps as invalid commands.
        /// </summary>
        /// <param name="context">The fixture context to simulate.</param>
        private async Task RunFullFailureAsync(FixtureContext context)
        {
            foreach (var step in context.Tasks)
            {
                await tagWriter.WriteInvalidCommandAsync(context, step);
            }

            logger.LogWarning("Full failure path executed for barcode {Barcode}", context.Barcode);
            await store.SaveAsync(context, "FullFailure");
        }

        /// <summary>
        /// Executes a single test run for a product and task, handling PLC simulation and error logging.
        /// </summary>
        /// <param name="product">The product to simulate.</param>
        /// <param name="options">The dry run options.</param>
        /// <param name="task">The execution step task.</param>
        /// <param name="attempt">The current attempt number.</param>
        /// <param name="context">The fixture context.</param>
        /// <returns>The resulting barcode string, or an empty string on failure.</returns>
        private async Task<string> ExecuteTestRun(Product product, DryRunOptions options, ExecutionStepTask task, int attempt, FixtureContext context)
        {
            try
            {
                logger.LogInformation("Executing step {Step} Command {Command} ({CommandName}) on machine {MachineId} for PartNumber {PartNumber} barcode {Barcode}", task.Task, task.Command,
                    PlcCommandMap.GetCommandName(task.Command), context.MachineId, product.PartNumber, context.Barcode);
                var barCodeResult = await plc.ExecuteCommandSimulation(product, context.Tags, context.Barcode, task, context.MachineId, options);
                if (barCodeResult != "Failure")
                {
                    logger.LogInformation("PLC command executed successfully for barcode {Barcode} on machine {MachineId}", context.Barcode, context.MachineId);
                    context.Barcode = barCodeResult;
                    return barCodeResult;
                }

                await store.LogResultAsync(new FixtureLogEntry(
                    product.PartNumber, product.ProductId, context.MachineId,
                    task.Task, barCodeResult, task.State, DateTime.UtcNow,
                    "Failed", attempt, "Executing Simulation failed"));
            }
            catch (Exception ex)
            {
                await plc.ClearTagsAsync(context.Tags);
                logger.LogInformation("Waiting {DelayTimeBetweenRetries} ms before next attempt", options.DelayTimeBetweenRetries);
                await Task.Delay(options.DelayTimeBetweenRetries);
                await store.LogResultAsync(new FixtureLogEntry(
                    product.PartNumber, product.ProductId, context.MachineId,
                    task.Task, context.Barcode, task.State, DateTime.UtcNow,
                    "Exception", attempt, ex.Message));
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates the result of a test run and logs the outcome.
        /// </summary>
        /// <param name="product">The product to validate.</param>
        /// <param name="options">The dry run options.</param>
        /// <param name="task">The execution step task.</param>
        /// <param name="machineId">The machine identifier.</param>
        /// <param name="tags">The tags used in the test run.</param>
        /// <param name="attempt">The current attempt number.</param>
        /// <param name="barcode">The barcode for the test run.</param>
        /// <param name="context">The fixture context.</param>
        /// <returns>A <see cref="Result"/> indicating the outcome of the validation.</returns>
        private async Task<Result> ValidateTestRun(Product product, DryRunOptions options, ExecutionStepTask task, int machineId,
            Dictionary<string, VariableS7> tags, int attempt, string barcode, FixtureContext context)
        {
            try
            {
                logger.LogInformation("Validating Results: step {Step} Command {Command} ({CommandName}) on machine {MachineId} for PartNumber {PartNumber} barcode {Barcode}", task.Task, task.Command,
                    PlcCommandMap.GetCommandName(task.Command), machineId, product.PartNumber, barcode);
                var result = await validator.ValidatePostExecutionStateAsync(context);
                if (result.IsSuccess)
                {
                    this.lastCode = context.Barcode;
                    await store.LogResultAsync(new FixtureLogEntry(
                        product.PartNumber, product.ProductId, machineId,
                        task.Task, barcode, task.State, DateTime.UtcNow,
                        "Success", attempt,
                        $"Validating Results: step {task.Task} Command {task.Command}" +
                        $" ({PlcCommandMap.GetCommandName(task.Command)}) " +
                        $"on machine {machineId} for PartNumber {product.PartNumber} barcode {barcode}"));
                }
                else
                {
                    var errorMessages = string.Join("\n", result.Errors.Select(e => $"PostValidationError: {e}"));
                    await store.LogResultAsync(new FixtureLogEntry(
                        product.PartNumber, product.ProductId, machineId,
                        task.Task, barcode, task.State, DateTime.UtcNow,
                        "Failure", attempt, errorMessages));
                }

                return result;
            }
            catch (Exception ex)
            {
                await plc.ClearTagsAsync(tags);
                logger.LogInformation("Waiting {DelayTimeBetweenRetries} ms before next attempt", options.DelayTimeBetweenRetries);
                await Task.Delay(options.DelayTimeBetweenRetries);
                await store.LogResultAsync(new FixtureLogEntry(
                    product.PartNumber, product.ProductId, machineId,
                    task.Task, barcode, task.State, DateTime.UtcNow,
                    "Exception", attempt, ex.Message));
            }

            return Result.WithFailure("This is not the normal path, so something goes wrong!!");
        }

        /// <inheritdoc/>
        public async Task ExecutePathAsync(Product product, TestPathType pathType, int sequenceIndex,
            ExecutionFlavor flavor, int maxNumberMachines, DryRunOptions options, CancellationToken cancellationToken)
        {
            try
            {
                var machinePath = await machineResolver.GetMachineSequenceAsync(product.ProductId);
                var barcode = this.GenerateBarcode(product.PartNumber, sequenceIndex);
                var context = new FixtureContext
                {
                    Barcode = barcode,
                };
                ArgumentNullException.ThrowIfNull(context);
                var ctx = context;
                var initialMachine = true;
                IEnumerable<ExecutionStepTask> taskList = Array.Empty<ExecutionStepTask>();
                foreach (var machineNumberPath in Enumerable.Range(0, Math.Min(maxNumberMachines, machinePath.Count)))
                {
                    var machineId = machinePath[machineNumberPath];
                    var tags = await tagMapper.GetTagsAsync(machineId, cancellationToken);
                    tags ??= new Dictionary<string, VariableS7>();
                    ctx.Tags = tags;
                    taskList = ExecutionStepTask.ResolveTaskList(initialMachine, flavor, machineNumberPath, maxNumberMachines);
                    foreach (var task in taskList)
                    {
                        foreach (var attempt in Enumerable.Range(0, options.MaxNumberOfRetry))
                        {
                            barcode = ctx.Barcode;
                            if (ctx.MachineId != machineId)
                            {
                                ctx.MachineId = machineId;
                                tags = await tagMapper.GetTagsAsync(machineId, cancellationToken);
                                tags ??= new Dictionary<string, VariableS7>();
                                ctx.Tags = tags;
                                logger.LogInformation("MachineID changed to {MachineId} on barcode {barcode} on Task {Task}", barcode, machineId, task);
                            }

                            ctx.PartNumber = product.PartNumber;
                            ctx.MachineId = machineId;
                            ctx.ProductId = product.ProductId;
                            ctx.TaskName = task.Task;
                            await this.ExecuteTestRun(product, options, task, attempt, ctx);
                            if (ctx.Barcode != barcode)
                            {
                                barcode = ctx.Barcode;
                                logger.LogInformation("Barcode changed to {Barcode} on machine {MachineId} on Task {Task}", barcode, machineId, task);
                            }

                            var validationResult = await this.ValidateTestRun(product, options, task, ctx.MachineId, ctx.Tags, attempt, ctx.Barcode, ctx);
                            if (validationResult.IsSuccess)
                            {
                                logger.LogInformation("Execution succeeded for task {TaskName} on attempt {Attempt}", task.Task, attempt);
                                break;
                            }

                            if (context != null)
                            {
                                break;
                            }

                            logger.LogWarning("Execution failed for task {TaskName} on attempt {Attempt}", task.Task, attempt);
                            await store.LogResultAsync(new FixtureLogEntry(
                                product.PartNumber, product.ProductId, machineId,
                                task.Task, barcode, task.State, DateTime.UtcNow,
                                "Failed", attempt, "Validation failed"));
                            logger.LogInformation("Waiting {DelayTimeBetweenRetries} ms before next task", options.DelayTimeBetweenRetries);
                            await Task.Delay(task.DelayTimeBetweenTasks);
                        }

                        logger.LogInformation("Waiting {DelayTimeBetweenTasks} ms before passing to next machine", task.DelayTimeBetweenTasks);
                        await Task.Delay(task.DelayTimeBetweenTasks);
                    }

                    logger.LogInformation("Waiting {DelayTimeBetweenMachines} ms before passing to next machine", options.DelayTimeBetweenMachines);
                    await Task.Delay(options.DelayTimeBetweenMachines);
                    initialMachine = false;
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error executing path: {Message}", e.Message);
                await store.LogResultAsync(new FixtureLogEntry(
                    product.PartNumber, product.ProductId, -1,
                    "Error", string.Empty, string.Empty, DateTime.UtcNow,
                    "Exception", 0, e.Message));
            }
        }
    }
}
