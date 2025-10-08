// <copyright file="VariableCreatedEvent.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Variables.Commands.Create;

/// <summary>
/// Represents the VariableCreatedEvent.
/// </summary>
public class VariableCreatedEvent : INotification
{
    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC identifier.
    /// </summary>
    public string Plc { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variable name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variable address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variable type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Length.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets the Event.
    /// </summary>
    public int Event { get; set; }

    /// <summary>
    /// Gets or sets the Direction.
    /// </summary>
    public int Direction { get; set; }

    /// <summary>
    /// Gets or sets the VariableGroupId.
    /// </summary>
    public int VariableGroupId { get; set; }

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction.
    /// </summary>
    public string Transaction { get; set; } = string.Empty;

    /// <summary>
    /// Returns a string representation of the variable created notification.
    /// </summary>
    /// <returns>A formatted string containing the variable created details.</returns>
    public override string ToString()
    {
        // [Fix]
        // CLAUDE
        // Date: 23/08/2025
        // Reason: Added meaningful ToString() method for better debugging and logging in MessageDto factory
        return $"Variable Created - MachineId: {this.MachineId}, Name: {this.Name}, Address: {this.Address}";
    }

    /// <summary>
    /// Represents the VariableCreatedHandler.
    /// </summary>
    public class VariableCreatedHandler(INotificationService notification) : Models.Interfaces.INotificationHandler<VariableCreatedEvent>
    {
        /// <summary>
        /// Executes Process operation.
        /// </summary>
        /// <param name="notification1">The notification1.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The result of Process.</returns>
        public async Task<Result> Process(VariableCreatedEvent notification1, CancellationToken cancellationToken)
        {
            // [Fix]
            // CLAUDE
            // Date: 23/08/2025
            // Reason: Added null check for notification1 parameter to ensure Railway-Oriented Programming compliance
            if (notification1 is null)
            {
                return Result.WithFailure(["Notification cannot be null."]);
            }

            // [Fix]
            // CLAUDE
            // Date: 23/08/2025
            // Reason: INotification refactoring - use MessageDto.CreateMessage<T> factory method instead of new MessageDto()
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.WithFailure(["Operation was canceled."]);
            }

            try
            {
                return await notification
                    .SendAsync(MessageDto.CreateMessage<VariableCreatedEvent>(notification1), cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Result.WithFailure([$"Operation finished with an exception {ex.Message}"]);
            }
        }
    }
}
