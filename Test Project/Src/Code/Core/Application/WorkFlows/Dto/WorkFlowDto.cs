// <copyright file="WorkFlowDto.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.WorkFlows.Dto;

/// <summary>
/// Data Transfer Object (DTO) for representing a workflow, including machines and edges.
/// </summary>
public class WorkFlowDto
{
    /// <summary>
    /// Gets or sets the workflow identifier.
    /// </summary>
    public int WorkFlowId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier associated with the workflow.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the next machine in the workflow.
    /// </summary>
    public int NextMachineId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the last machine in the workflow.
    /// </summary>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the rule identifier associated with the workflow.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Gets or sets the list of machines in the workflow.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: [CLUSTER F FIX] Constructor null safety - Tests expect collections to be null by default, not empty collections
    public List<Machine> Machine { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of edges in the workflow.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: [CLUSTER F FIX] Constructor null safety - Tests expect collections to be null by default, not empty collections
    public List<Edge> Edges { get; set; } = [];

    /// <summary>
    /// Converts a <see cref="WorkFlow"/> entity to a <see cref="WorkFlowDto"/>.
    /// </summary>
    /// <param name="src">The source <see cref="WorkFlow"/> entity.</param>
    /// <returns>A <see cref="WorkFlowDto"/> representing the entity.</returns>
    public static IndQuestResults.Result<WorkFlowDto> ToDto(WorkFlow src)
    {
        // [Fix]
        // CLAUDE
        // Date: 22/08/2025
        // Reason: Pattern 11 Fix - Updated error message to match test expectation "Parameter 'src' cannot be null"
        if (src == null)
        {
            return IndQuestResults.Result<WorkFlowDto>.WithFailure("Parameter 'src' cannot be null");
        }

        return IndQuestResults.Result<WorkFlowDto>.Success(new WorkFlowDto
        {
            WorkFlowId = src.WorkFlowId,
            ProductId = src.ProductId,
            NextMachineId = src.NextMachineId,
            LastMachineId = src.LastMachineId,
            RuleId = src.RuleId,
            Machine = src.Machine,
            Edges = src.Edges,
        });
    }

    /// <summary>
    /// Converts a <see cref="WorkFlowDto"/> to a <see cref="WorkFlow"/> entity.
    /// </summary>
    /// <param name="src">The source <see cref="WorkFlowDto"/>.</param>
    /// <returns>A <see cref="WorkFlow"/> entity representing the DTO.</returns>
    public static IndQuestResults.Result<WorkFlow> ToEntity(WorkFlowDto src)
    {
        // [Fix]
        // CLAUDE
        // Date: 22/08/2025
        // Reason: Pattern 11 Fix - Updated error message to match test expectation "Parameter 'src' cannot be null"
        if (src == null)
        {
            return IndQuestResults.Result<WorkFlow>.WithFailure("Parameter 'src' cannot be null");
        }

        return IndQuestResults.Result<WorkFlow>.Success(new WorkFlow
        {
            WorkFlowId = src.WorkFlowId,
            ProductId = src.ProductId,
            NextMachineId = src.NextMachineId,
            LastMachineId = src.LastMachineId,
            RuleId = src.RuleId,
            Machine = src.Machine,
            Edges = src.Edges,
        });
    }
}
