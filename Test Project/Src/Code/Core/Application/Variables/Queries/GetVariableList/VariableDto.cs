// <copyright file="VariableDto.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Variables.Queries.GetVariableList;

/// <summary>
/// Data transfer object for Variable entity, used for transferring variable data between layers.
/// </summary>
public class VariableDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariableDto"/> class.
    /// </summary>
    public VariableDto()
    {
    }

    /// <summary>
    /// Gets or sets the unique identifier for the variable.
    /// </summary>
    public int VariableId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the variable.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC identifier associated with the variable.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Alias { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string NetType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the length of the variable.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the variable is active.
    /// </summary>
    public int IsActive { get; set; }

    /// <summary>
    /// Gets or sets the direction of the variable.
    /// </summary>
    public int Direction { get; set; }

    /// <summary>
    /// Gets or sets the group identifier to which the variable belongs.
    /// </summary>
    public int VariableGroupId { get; set; }

    /// <summary>
    /// Gets or sets the variable specification identifier, if any.
    /// </summary>
    public int? VariableSpecId { get; set; }

    /// <summary>
    /// Gets or sets the tag status of the variable.
    /// </summary>
    public int TagStatus { get; set; }

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string NativeType { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Value { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string NativeAddress { get; set; } = null!;

    /// <summary>
    /// Converts a <see cref="Variable"/> entity to a <see cref="VariableDto"/> using functional Result semantics.
    /// </summary>
    /// <param name="src">The source <see cref="Variable"/> entity.</param>
    /// <returns>A Result containing the <see cref="VariableDto"/> or failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<VariableDto> ToDto(Variable src)
    {
        // [Fix]
        // CLAUDE
        // Date: 22/08/2025
        // Reason: Pattern 11 Fix - Updated error message to match test expectation "Parameter 'src' cannot be null"
        if (src == null)
        {
            return IndQuestResults.Result<VariableDto>.WithFailure("Parameter 'src' cannot be null");
        }

        return IndQuestResults.Result<VariableDto>.Success(new VariableDto
        {
            VariableId = src.VariableId,
            MachineId = src.MachineId,
            PlcId = src.PlcId,
            Name = src.Name,
            Description = src.Description,
            Alias = src.Alias,
            Address = src.Address,
            NetType = src.NetType,
            Length = src.Length,
            IsActive = src.IsActive,
            Direction = src.Direction,
            VariableGroupId = src.VariableGroupId,
            VariableSpecId = src.VariableSpecId,
            TagStatus = src.TagStatus,
            NativeType = src.NativeType,
            Value = src.Value,
            NativeAddress = src.NativeAddress,
        });
    }

    /// <summary>
    /// Converts a collection of <see cref="Variable"/> entities to a list of <see cref="VariableDto"/> using functional Result semantics.
    /// </summary>
    /// <param name="src">The source collection of <see cref="Variable"/> entities.</param>
    /// <returns>A Result containing the list of <see cref="VariableDto"/> objects or failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<List<VariableDto>> ToDtoList(IEnumerable<Variable> src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<List<VariableDto>>.WithFailure("Variable collection cannot be null");
        }

        var list = src.Select(s => new VariableDto
        {
            VariableId = s.VariableId,
            MachineId = s.MachineId,
            PlcId = s.PlcId,
            Name = s.Name,
            Description = s.Description,
            Alias = s.Alias,
            Address = s.Address,
            NetType = s.NetType,
            Length = s.Length,
            IsActive = s.IsActive,
            Direction = s.Direction,
            VariableGroupId = s.VariableGroupId,
            VariableSpecId = s.VariableSpecId,
            TagStatus = s.TagStatus,
            NativeType = s.NativeType,
            Value = s.Value,
            NativeAddress = s.NativeAddress,
        }).ToList();
        return IndQuestResults.Result<List<VariableDto>>.Success(list);
    }

    /// <summary>
    /// Converts a <see cref="VariableDto"/> to a <see cref="Variable"/> entity using functional Result semantics.
    /// </summary>
    /// <param name="src">The source <see cref="VariableDto"/>.</param>
    /// <returns>A Result containing the <see cref="Variable"/> or failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<Variable> ToEntity(VariableDto src)
    {
        // [Fix]
        // CLAUDE
        // Date: 22/08/2025
        // Reason: Pattern 11 Fix - Updated error message to match test expectation "Parameter 'src' cannot be null"
        if (src == null)
        {
            return IndQuestResults.Result<Variable>.WithFailure("Parameter 'src' cannot be null");
        }

        return IndQuestResults.Result<Variable>.Success(new Variable
        {
            VariableId = src.VariableId,
            MachineId = src.MachineId,
            PlcId = src.PlcId,
            Name = src.Name,
            Description = src.Description,
            Alias = src.Alias,
            Address = src.Address,
            NetType = src.NetType,
            Length = src.Length,
            IsActive = src.IsActive,
            Direction = src.Direction,
            VariableGroupId = src.VariableGroupId,
            VariableSpecId = src.VariableSpecId,
            TagStatus = src.TagStatus,
            NativeType = src.NativeType,
            Value = src.Value,
            NativeAddress = src.NativeAddress,
        });
    }
}
