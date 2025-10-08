// <copyright file="RegisterCleaner.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services;

/// <summary>
/// Cleans and prepares registers for persistence.
/// </summary>
public class RegisterCleaner : IRegisterCleaner
{
    private readonly ILogger<RegisterCleaner> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterCleaner"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public RegisterCleaner(ILogger<RegisterCleaner> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Result<IEnumerable<Register>> CleanRegisters(
        IDictionary<string, Register> registers,
        int cycleId,
        int machineId,
        DateTime timestamp)
    {
        try
        {
            return ResultExtensions.CreateIfValid(
                    factory: () => new { Registers = registers, CycleId = cycleId, MachineId = machineId, Timestamp = timestamp },
                    validations: (registers, nameof(registers)))
                .Ensure(ctx => ctx.CycleId > 0, $"Invalid cycleId: {cycleId}")
                .Ensure(ctx => ctx.MachineId > 0, $"Invalid machineId: {machineId}")
                .Tap(ctx => _logger.LogInformation(
                    "Cleaning {Count} registers for CycleId={CycleId}, MachineId={MachineId}",
                    ctx.Registers.Count, ctx.CycleId, ctx.MachineId))
                .Map(ctx => ProcessRegisters(ctx.Registers, ctx.CycleId, ctx.MachineId, ctx.Timestamp))
                .Tap(cleanedRegisters => _logger.LogInformation("Successfully cleaned {Count} registers", cleanedRegisters.Count()))
                .OnFailure(errors => _logger.LogError("Register cleaning validation failed: {Errors}", string.Join(", ", errors)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception cleaning registers");
            return Result<IEnumerable<Register>>.WithFailure($"Exception cleaning registers: {ex.Message}");
        }
    }

    private IEnumerable<Register> ProcessRegisters(
        IDictionary<string, Register> registers,
        int cycleId,
        int machineId,
        DateTime timestamp)
    {
        return registers
            .Where(kvp => kvp.Value is not null)
            .Select(kvp => CleanRegister(kvp.Value, kvp.Key, cycleId, machineId, timestamp))
            .Where(register => register is not null)
            .Cast<Register>();
    }

    private Register? CleanRegister(Register register, string key, int cycleId, int machineId, DateTime timestamp)
    {
        if (register is null)
        {
            _logger.LogWarning("Null register found for key: {Key}", key);
            return null;
        }

        return Result<Register>
            .Success(register)
            .Map(r => new Register
            {
                Name = CleanString(r.Name),
                Description = CleanString(r.Description),
                Value = CleanString(r.Value),
                CycleId = cycleId,
                MachineId = machineId,
                RegisterId = 0, // Reset for new insert
                TimeStamp = timestamp
            })
            .Value;
    }

    private static string CleanString(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        // Remove invisible characters and trim
        return value.Trim()
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\t", string.Empty);
    }
}
