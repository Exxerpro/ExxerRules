// <copyright file="DistinctRegisterService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Services;

/// <summary>
/// Represents the DistinctRegisterService.
/// </summary>
public class DistinctRegisterService(
    IRepository<DistinctRegister> distinctRegisterRepository,
    IRepository<Register> registerRepository) : IDistinctRegisterService
{
    /// <inheritdoc/>
    public async Task UpdateDistinctRegistersAsync(CancellationToken cancellationToken)
    {
        var distinctValues = await this.GetDistinctRegistersAsync(cancellationToken);

        await this.ClearTheExistingDistinctRegisters(cancellationToken);
        await this.InsertTheDistinctRegistersValues(distinctValues, cancellationToken);

        // CommitAsync changes to the database
        await distinctRegisterRepository.CommitAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DistinctRegister>> GetDistinctRegistersAsync(CancellationToken cancellationToken)
    {
        var spec = new Specification<Register>(r => true)
            .ApplyNoTracking();

        var distinctValues = await registerRepository.AsQueryableAsync(spec, cancellationToken);
        if (distinctValues.Value is null)
        {
            return [];
        }

        var distinctRegister = distinctValues.Value.Select(r => new DistinctRegister
        {
            Name = r.Name,
            VariableId = r.VariableId,
            MachineId = r.MachineId,
        })
            .Distinct();

        return distinctRegister.ToList();
    }

    private async Task InsertTheDistinctRegistersValues(
        IEnumerable<DistinctRegister> distinctValues,
        CancellationToken cancellationToken)
    {
        foreach (var distinctItem in distinctValues)
        {
            var distinctRegister = new DistinctRegister
            {
                Name = distinctItem.Name,
                VariableId = distinctItem.VariableId,
                MachineId = distinctItem.MachineId,
            };
            await distinctRegisterRepository.AddAsync(distinctRegister, cancellationToken);
        }
    }

    private async Task ClearTheExistingDistinctRegisters(CancellationToken cancellationToken)
    {
        var allDistinctRegisters = await distinctRegisterRepository.ListAsync(cancellationToken);
        if (allDistinctRegisters.IsSuccess && allDistinctRegisters.Value is not null)
        {
            foreach (var existingRegister in allDistinctRegisters.Value)
            {
                await distinctRegisterRepository.DeleteAsync(existingRegister, cancellationToken);
            }
        }
    }
}
