// <copyright file="GetRegistersListQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Queries.GetRegisterList;

/// <summary>
/// Represents the GetRegistersListQueryHandler.
/// </summary>
public class GetRegistersListQueryHandler(
    IRepository<Variable> variableRepository,
    IRepository<Register> registerRepository)
    : IMonitorRequestHandler<GetRegistersListQuery, IEnumerable<RegisterDto>>
{
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RegisterDto>>> ProcessAsync(GetRegistersListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var variablesIds = await this.GetVariablesIdFromRequest(request, cancellationToken);

            if (variablesIds is null || !variablesIds.Any())
            {
                return Result<IEnumerable<RegisterDto>>.WithFailure(["No valid variable IDs or names provided."]);
            }

            var result = await this.GetRegisterOnRequestSpec(request, cancellationToken, variablesIds);

            return result;
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<RegisterDto>>.WithFailure([ex.Message]);
        }
    }

    private async Task<IEnumerable<int>> GetVariablesIdFromRequest(GetRegistersListQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<int> variablesIds = [];

        if (request.VariablesId is not null && request.VariablesId.Any())
        {
            // Case where variable IDs are provided, no need to fetch them
            variablesIds = request.VariablesId;
        }

        if (request.RegistersName is null || !request.RegistersName.Any())
        {
            return variablesIds;
        }

        // Specification to filter registers by name and machine ID
        var registerSpec = new Specification<Register>(
            r => request.RegistersName.Contains(r.Name)
                 && request.MachineId.Contains(r.MachineId));

        // Fetch register IDs
        var registers = await registerRepository.ListAsync(registerSpec, cancellationToken);

        var registerIds = registers.Value?.Select(r => r.VariableId).ToList() ?? [];

        // Specification to filter variables by name and machine ID
        var variablesSpec = new Specification<Variable>(
            v => request.RegistersName.Contains(v.Name)
                 && request.MachineId.Contains(v.MachineId));

        // Fetch variable IDs
        var variables = await variableRepository.ListAsync(variablesSpec, cancellationToken);
        var variableIds = variables.Value?.Select(v => v.VariableId).ToList() ?? [];

        // Union of register IDs and variable IDs, eliminating duplicates
        var combinedIds = registerIds.Union(variableIds).Union(variablesIds).Distinct();

        return combinedIds;
    }

    private async Task<Result<IEnumerable<RegisterDto>>> GetRegisterOnRequestSpec(GetRegistersListQuery request, CancellationToken cancellationToken,
        IEnumerable<int> variablesIds)
    {
        Result<IEnumerable<RegisterDto>> result;

        var spec = new Specification<Register>(r =>
            variablesIds.Contains(r.VariableId) &&
            request.MachineId.Contains(r.MachineId) &&
            r.TimeStamp >= request.StartDate &&
            r.TimeStamp <= request.EndDate);

        // Retrieve the registers with the matched EntitieId, MachineId list, and CreatedOn range
        var registers = await registerRepository.ListAsync(
            spec,
            cancellationToken);

        if (registers.IsFailure)
        {
            {
                result = Result<IEnumerable<RegisterDto>>.WithFailure(["No registers found for the specified criteria."]);
                return result;
            }
        }

        // Map the result to RegisterDto
        var registerDtos = RegisterDto.ToDtoList(registers.Value ?? []);
        if (registerDtos.IsFailure || registerDtos.Value is null)
        {
            return Result<IEnumerable<RegisterDto>>.WithFailure(registerDtos.Errors);
        }

        return Result<IEnumerable<RegisterDto>>.Success(registerDtos.Value);
    }
}
