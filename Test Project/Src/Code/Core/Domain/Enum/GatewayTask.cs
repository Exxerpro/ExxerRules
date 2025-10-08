// <copyright file="GatewayTask.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum;

/// <summary>
/// Represents a gateway task in the system, such as creating barcodes, reading barcodes, creating cycles, updating cycles, and process-related tasks.
/// </summary>
public class GatewayTask : EnumModel
{
    /// <summary>
    /// Represents an invalid gateway task.
    /// </summary>
    public static new readonly GatewayTask Invalid
        = new(-1, "Invalid Value");

    /// <summary>
    /// Represents no gateway task.
    /// </summary>
    public static readonly GatewayTask None
        = new(0, "None");

    /// <summary>
    /// Represents a task to create a barcode asynchronously.
    /// </summary>
    public static readonly GatewayTask CreateBarCodeAsync
        = new(4, "CreateBarCodeAsync");

    /// <summary>
    /// Represents a task to read a barcode asynchronously.
    /// </summary>
    public static readonly GatewayTask ReadBarCodeAsync
        = new(8, "ReadBarCodeAsync");

    /// <summary>
    /// Represents a task to create a cycle asynchronously.
    /// </summary>
    public static readonly GatewayTask CreateCycleAsync
        = new(16, "CreateCycleAsync");

    /// <summary>
    /// Represents a task to update a cycle as OK asynchronously.
    /// </summary>
    public static readonly GatewayTask UpdateCycleOkAsync
        = new(32, "UpdateCycleOkAsync");

    /// <summary>
    /// Represents a task to update a cycle as Not OK asynchronously.
    /// </summary>
    public static readonly GatewayTask UpdateCycleNotOkAsync
        = new(64, "UpdateCycleNotOkAsync");

    /// <summary>
    /// Represents a task to end the process asynchronously.
    /// </summary>
    public static readonly GatewayTask EndOfProcessAsync
        = new(128, "EndOfProcessAsync", "RejectPartAsyncMonitor");

    /// <summary>
    /// Represents a task to reject a part asynchronously.
    /// </summary>
    public static readonly GatewayTask RejectPartAsync
        = new(256, "RejectPartAsync", "RejectPartAsyncMonitor");

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTask"/> class.
    /// </summary>
    public GatewayTask()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTask"/> class with specified values.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <param name="name">The name.</param>
    /// <param name="displayName">The display name.</param>
    private GatewayTask(int value, string name, string? displayName = null)
        : base(value, name, displayName ?? string.Empty)
    {
    }

    /// <summary>
    /// Implicitly converts a GatewayTask to its integer value.
    /// </summary>
    /// <param name="enumerator">The enumerator to convert.</param>
    public static implicit operator int(GatewayTask enumerator) => enumerator.Value;

    /// <summary>
    /// Implicitly converts a GatewayTask to a nullable integer value.
    /// </summary>
    /// <param name="enumerator">The enumerator to convert.</param>
    public static implicit operator int?(GatewayTask enumerator) => enumerator.Value;

    /// <summary>
    /// Implicitly converts a GatewayTask to its string representation.
    /// </summary>
    /// <param name="enumerator">The enumerator to convert.</param>
    public static implicit operator string(GatewayTask enumerator) => enumerator.Value.ToString();

    /// <summary>
    /// Implicitly converts an integer value to a GatewayTask.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator GatewayTask(int value) => FromValue<GatewayTask>(value);

    /// <summary>
    /// Implicitly converts a nullable integer value to a GatewayTask.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator GatewayTask(int? value) => FromValue<GatewayTask>(value ?? 0);
}
