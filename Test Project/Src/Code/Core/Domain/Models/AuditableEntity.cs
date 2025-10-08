// <copyright file="AuditableEntity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Models;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Provides audit tracking functionality for domain entities, including creation and modification timestamps and user information.
/// </summary>
/// <remarks>
/// This base class automatically tracks when entities are created and modified, along with the user responsible for each operation.
/// The timestamp properties include validation to ensure dates are not set to values before the year 2000.
/// </remarks>
public class AuditableEntity
{
    private readonly IDateTimeMachine dateTimeMachine;
    private DateTime? createdOn;
    private DateTime? modifiedOn;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditableEntity"/> class with optional date time machine dependency.
    /// </summary>
    /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
    public AuditableEntity(IDateTimeMachine? dateTimeMachine = null)
    {
        this.dateTimeMachine = dateTimeMachine ?? new DateTimeMachine();

        // Initialize with default date 2020-01-01 when no dependency provided
        var defaultDate = new DateTime(2020, 1, 1);
        this.createdOn = this.dateTimeMachine.Now;
        this.modifiedOn = this.dateTimeMachine.Now;
    }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the entity was created.
    /// </summary>
    /// <remarks>
    /// The value is automatically set to the current local time when the entity is instantiated.
    /// Setting this property to null or a date before the year 2000 will result in the value being set to null.
    /// </remarks>
    public DateTime? CreatedOn
    {
        get => this.createdOn;
        set
        {
            if (value == null || value.Value.Year < 2000)
            {
                this.createdOn = null;
                return;
            }

            this.createdOn = value;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the entity was last modified.
    /// </summary>
    /// <remarks>
    /// The value is automatically set to the current local time when the entity is instantiated.
    /// Setting this property to null or a date before the year 2000 will result in the value being set to null.
    /// </remarks>
    public DateTime? ModifiedOn
    {
        get => this.modifiedOn;
        set
        {
            // Simulate validation logic (but no recursive call)
            if (value == null || value.Value.Year < 2000)
            {
                this.modifiedOn = null;
                return;
            }

            this.modifiedOn = value;
        }
    }
}
