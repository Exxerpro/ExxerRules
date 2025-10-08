// <copyright file="IWorkflowCompatibilityRepository.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Configuration.Services;

public interface IWorkflowCompatibilityRepository
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate workflow compatibility repository logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    IReadOnlyList<MachineProductMap> GetAll();

    IEnumerable<MachineProductMap> GetByCustomer(int customerId);

    IEnumerable<MachineProductMap> GetByProduct(int productId);

    IEnumerable<MachineProductMap> GetByMachine(int machineId);

    bool IsCompatible(int machineId, int productId);

    Task RefreshAsync();
}
