// <copyright file="RegisterRepositoryExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repositories;

/// <summary>
/// Provides extension methods for <see cref="IRepository{Register}"/> to support common register queries and operations.
/// </summary>
public static class RegisterRepositoryExtensions
{
    /// <summary>
    /// Gets a list of registers grouped by machine for the specified cycle IDs and active variables.
    /// </summary>
    /// <param name="registerRepository">The register repository.</param>
    /// <param name="variableRepository">The variable repository.</param>
    /// <param name="cycleIdList">The list of cycle IDs to filter by.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of registers grouped by machine, or a failure result if not found.</returns>
    public static async Task<Result<List<Register>>> GetRegistersGroupedByMachineAsync(
        this IRepository<Register> registerRepository,
        IRepository<Variable> variableRepository,
        List<int> cycleIdList,
        CancellationToken cancellationToken)
    {
        var activeVariablesSpec = new Specification<Variable>(v => v.IsActive == 1);
        var activeVariables = await variableRepository.ListAsync(activeVariablesSpec, cancellationToken).ConfigureAwait(false);

        if (activeVariables.IsFailure || activeVariables.Value is null || !activeVariables.Value.Any())
        {
            return Result<List<Register>>.WithFailure("No active variables found");
        }

        var activeVariableIds = activeVariables.Value.Select(v => v.VariableId).ToList();

        var spec = new Specification<Register>(p => cycleIdList.Contains(p.CycleId) && activeVariableIds.Contains(p.VariableId));
        var registers = await registerRepository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        if (registers.IsFailure || registers.Value is null || !registers.Value.Any())
        {
            return Result<List<Register>>.WithFailure("No registers found for the specified Cycle IDs");
        }

        return Result<List<Register>>.WithFailure("No registers found for the specified Cycle IDs");
    }

    /// <summary>
    /// Gets a list of register views with variable information for the specified cycle IDs.
    /// </summary>
    /// <param name="registerRepository">The register repository.</param>
    /// <param name="variableRepository">The variable repository.</param>
    /// <param name="cycleIdList">The list of cycle IDs to filter by.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of register views, or a failure result if not found.</returns>
    public static async Task<Result<List<RegisterView>>> GetRegistersWithVariablesAsync(
        this IRepository<Register> registerRepository,
        IRepository<Variable> variableRepository,
        List<int> cycleIdList,
        CancellationToken cancellationToken)
    {
        var activeVariablesSpec = new Specification<Variable>(v => v.IsActive == 1);
        var activeVariablesResult = await variableRepository.ListAsync(activeVariablesSpec, cancellationToken).ConfigureAwait(false);

        if (activeVariablesResult.IsFailure || activeVariablesResult.Value is null || !activeVariablesResult.Value.Any())
        {
            return Result<List<RegisterView>>.WithFailure("No active variables found");
        }

        var spec = new Specification<Register>(r => cycleIdList.Contains(r.CycleId));
        var registersResult = await registerRepository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        if (registersResult.IsFailure || registersResult.Value is null || !registersResult.Value.Any())
        {
            return Result<List<RegisterView>>.WithFailure("No registers found for the specified Cycle IDs");
        }

        var registers = registersResult.Value;
        var activeVariables = activeVariablesResult.Value;

        var query = from register in registers
                    join variable in activeVariables on register.VariableId equals variable.VariableId into variableJoin
                    from subVariable in variableJoin.DefaultIfEmpty()
                    where subVariable?.IsActive > 0
                    select new { register, subVariable };

        var dict = new Dictionary<KeyRegister, RegisterView>();

        foreach (var g in query)
        {
            var newKey = new KeyRegister()
            {
                CycleId = g.register.CycleId,
                VariableId = g.register.VariableId,
            };

            dict.TryAdd(newKey, new RegisterView
            {
                RegisterId = g.register.RegisterId,
                Name = g.register.Name,
                MachineId = g.subVariable?.MachineId ?? 0,
                VariableId = g.register.VariableId,
                CycleId = g.register.CycleId,
                Value = !string.IsNullOrEmpty(g.register.Value) ? g.register.Value : string.Empty,
                DataType = g.register.DataType,
                StatusValueId = g.register.StatusValueId,
                Description = g.subVariable?.Description ?? string.Empty,
                TimeStamp = g.register.TimeStamp,
            });
        }

        var resultQuery = dict.Values.ToList();
        return Result<List<RegisterView>>.Success(resultQuery);
    }

    /// <summary>
    /// Gets a list of register views by cycle ID list, grouped by machine and variable.
    /// </summary>
    /// <param name="registerRepository">The register repository.</param>
    /// <param name="variableRepository">The variable repository.</param>
    /// <param name="cycleIdList">The list of cycle IDs to filter by.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of register views, or a failure result if not found.</returns>
    public static async Task<Result<List<RegisterView>>> GetRegisterByCycleIdListAsync(
        this IRepository<Register> registerRepository,
        IRepository<Variable> variableRepository,
        List<int> cycleIdList,
        CancellationToken cancellationToken)
    {
        // Step 1: Get all active variables
        var activeVariablesSpec = new Specification<Variable>(v => v.IsActive == 1);
        var activeVariablesResult = await variableRepository.ListAsync(activeVariablesSpec, cancellationToken).ConfigureAwait(false);

        if (activeVariablesResult.IsFailure || activeVariablesResult.Value is null || !activeVariablesResult.Value.Any())
        {
            return Result<List<RegisterView>>.WithFailure("No active variables found.");
        }

        var activeVariables = activeVariablesResult.Value;

        // Step 2: Get all registers related to provided cycle IDs
        var registersSpec = new Specification<Register>(r => cycleIdList.Contains(r.CycleId));
        var registersResult = await registerRepository.ListAsync(registersSpec, cancellationToken).ConfigureAwait(false);

        if (registersResult.IsFailure || registersResult.Value is null || !registersResult.Value.Any())
        {
            return Result<List<RegisterView>>.WithFailure("No registers found for the specified Cycle IDs.");
        }

        var registers = registersResult.Value;

        // Step 3: Create lookup for active variables for faster join
        var activeVariableLookup = activeVariables.ToDictionary(v => v.VariableId);

        // Step 4: Join registers with variables
        var registerViews = new Dictionary<(int MachineId, int VariableId), RegisterView>();

        foreach (var register in registers)
        {
            if (!activeVariableLookup.TryGetValue(register.VariableId, out var variable) || variable.IsActive != 1)
            {
                continue; // Skip if variable is not active
            }

            var key = (MachineId: register.MachineId, VariableId: register.VariableId);

            if (!registerViews.ContainsKey(key))
            {
                registerViews[key] = new RegisterView
                {
                    RegisterId = register.RegisterId,
                    Name = register.Name,
                    MachineId = register.MachineId, // <-- Grouping by real machine id
                    VariableId = register.VariableId,
                    CycleId = register.CycleId, // Original cycle id, but grouping is by MachineId
                    Value = register.Value ?? string.Empty,
                    DataType = register.DataType,
                    StatusValueId = register.StatusValueId,
                    Description = variable.Description ?? string.Empty,
                    TimeStamp = register.TimeStamp,
                };
            }
        }

        var resultList = registerViews.Values.ToList();

        return Result<List<RegisterView>>.Success(resultList);
    }
}
