// <copyright file="MachineDto.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Queries.GetMachinesList;

/// <summary>
/// Data transfer object representing a manufacturing machine with its configuration and connectivity information.
/// </summary>
/// <remarks>
/// This DTO contains all machine details including identification, location, traceability settings,
/// network configuration, and workflow relationships. It provides mapping methods for conversion between entity and DTO.
/// </remarks>
public class MachineDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MachineDto"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public MachineDto()
    {
        this.MachineType = Domain.Enum.MachineType.None;
        this.WorkFlowType = Domain.Enum.WorkFlowType.None;
        this.ImageName = string.Empty;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the machine.
    /// </summary>
    /// <value>The machine ID as an integer.</value>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the descriptive name of the machine.
    /// </summary>
    /// <value>The machine name as a string. Defaults to empty string if not specified.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the physical location of the machine within the facility.
    /// </summary>
    /// <value>The machine location as a string. Defaults to empty string if not specified.</value>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a detailed description of the machine and its capabilities.
    /// </summary>
    /// <value>The machine description as a string. Defaults to empty string if not specified.</value>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type/category of the machine (e.g., Printer, Scanner, Assembly).
    /// </summary>
    /// <value>A MachineType enumeration value indicating the machine's functional category.</value>
    public MachineType MachineType { get; set; }

    /// <summary>
    /// Gets or sets the workflow type that defines how this machine participates in the production process.
    /// </summary>
    /// <value>A WorkFlowType enumeration value indicating the machine's workflow participation.</value>
    public WorkFlowType WorkFlowType { get; set; }

    /// <summary>
    /// Gets or sets the IP address for network communication with the machine.
    /// </summary>
    /// <value>The IP address as a string. Defaults to empty string if not specified.</value>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether application-level traceability is enabled for this machine.
    /// </summary>
    /// <value>1 if application traceability is enabled, 0 if disabled.</value>
    public int EnableAppTraceability { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether traceability bypass is enabled for this machine.
    /// </summary>
    /// <value>1 if bypass traceability is enabled, 0 if disabled.</value>
    /// <remarks>
    /// When enabled, this allows the machine to bypass normal traceability requirements.
    /// </remarks>
    public int EnableBypassTraceability { get; set; }

    /// <summary>
    /// Gets a value indicating whether the machine is enabled for operation.
    /// </summary>
    /// <value>True if the machine is enabled (either app traceability is on OR bypass traceability is off), false otherwise.</value>
    /// <remarks>
    /// A machine is considered enabled if application traceability is enabled OR if bypass traceability is not enabled.
    /// This logic ensures machines are operational unless explicitly bypassed without app traceability.
    /// </remarks>
    public bool IsEnabled => this.EnableAppTraceability == 1
                             || this.EnableBypassTraceability != 1;

    /// <summary>
    /// Gets or sets the number of retry attempts for operations on this machine.
    /// </summary>
    /// <value>The retry count as an integer. Defaults to 1 if not specified.</value>
    public int Retry { get; set; } = 1;

    /// <summary>
    /// Gets or sets the file name or path of the image associated with this machine.
    /// </summary>
    /// <value>The image file name as a string.</value>
    public string ImageName { get; set; }

    /// <summary>
    /// Gets or sets the collection of workflow edges where this machine is the source.
    /// </summary>
    /// <value>A collection of Edge objects representing outgoing workflow connections. Defaults to an empty collection.</value>
    /// <remarks>
    /// These edges define which machines receive work or products FROM this machine.
    /// </remarks>
    public ICollection<Edge> FromEdges { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of workflow edges where this machine is the destination.
    /// </summary>
    /// <value>A collection of Edge objects representing incoming workflow connections. Defaults to an empty collection.</value>
    /// <remarks>
    /// These edges define which machines send work or products TO this machine.
    /// </remarks>
    public ICollection<Edge> ToEdges { get; set; } = [];

    /// <summary>
    /// Gets or sets the identifier of the business rule associated with this machine.
    /// </summary>
    /// <value>The rule ID as an integer.</value>
    public int RuleId { get; set; }

    /// <summary>
    /// Converts a Machine entity to a MachineDto.
    /// </summary>
    /// <param name="src">The Machine entity to convert.</param>
    /// <returns>A MachineDto containing the converted machine data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when src is null.</exception>
    /// <remarks>
    /// This method handles conversion of enumeration values using EnumModel.FromValue for type safety.
    /// Edge collections are directly assigned as they are reference types.
    /// </remarks>
    /// <summary>
    /// Converts a Machine entity to a MachineDto.
    /// </summary>
    /// <param name="src">The Machine entity to convert.</param>
    /// <returns>A Result containing the MachineDto or failure information.</returns>
    public static Result<MachineDto> ToDto(Machine src)
    {
        if (src == null)
        {
            return Result<MachineDto>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        return Result<MachineDto>.Success(new MachineDto
        {
            MachineId = src.MachineId,
            Name = src.Name,
            Location = src.Location,
            Description = src.Description,
            MachineType = EnumModel.FromValue<MachineType>(src.MachineType),
            WorkFlowType = EnumModel.FromValue<WorkFlowType>(src.WorkFlowType),
            EnableAppTraceability = src.EnableAppTraceability,
            EnableBypassTraceability = src.EnableBypassTraceability,
            Retry = src.Retry,
            FromEdges = src.FromEdges,
            ToEdges = src.ToEdges,
            RuleId = src.RuleId,
        });
    }

    /// <summary>
    /// Converts a MachineDto to a Machine entity.
    /// </summary>
    /// <param name="src">The MachineDto to convert.</param>
    /// <returns>A Machine entity containing the converted data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when src is null.</exception>
    /// <remarks>
    /// This method extracts the enumeration values using implicit conversion for database storage.
    /// Edge collections are directly assigned as they are reference types.
    /// </remarks>
    /// <summary>
    /// Converts a MachineDto to a Machine entity.
    /// </summary>
    /// <param name="src">The MachineDto to convert.</param>
    /// <returns>A Result containing the Machine entity or failure information.</returns>
    public static Result<Machine> ToEntity(MachineDto src)
    {
        if (src == null)
        {
            return Result<Machine>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        return Result<Machine>.Success(new Machine
        {
            MachineId = src.MachineId,
            Name = src.Name,
            Location = src.Location,
            Description = src.Description,
            MachineType = (int)src.MachineType,
            WorkFlowType = (int)src.WorkFlowType,
            EnableAppTraceability = src.EnableAppTraceability,
            EnableBypassTraceability = src.EnableBypassTraceability,
            Retry = src.Retry,
            FromEdges = src.FromEdges,
            ToEdges = src.ToEdges,
            RuleId = src.RuleId,
        });
    }
}
