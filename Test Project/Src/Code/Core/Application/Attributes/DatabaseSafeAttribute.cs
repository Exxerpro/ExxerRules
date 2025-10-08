// <copyright file="DatabaseSafeAttribute.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Attributes;

/// <summary>
/// Attribute used to mark entities with their appropriate database environment.
/// </summary>
/// <remarks>
/// This attribute helps prevent simulation data from contaminating production databases
/// by clearly marking which environment each entity type belongs to.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public sealed class DatabaseSafeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseSafeAttribute"/> class.
    /// </summary>
    /// <param name="environment">The environment this entity is safe to use in.</param>
    public DatabaseSafeAttribute(string environment)
    {
        this.Environment = environment ?? "Any";
        _ = this.ValidateEnvironmentResult();
    }

    /// <summary>
    /// Gets the environment this entity is designated for.
    /// </summary>
    /// <remarks>
    /// Valid values: "Production", "Simulation", "Demo", "Any".
    /// </remarks>
    public string Environment { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this entity can be used across multiple environments.
    /// </summary>
    public bool AllowCrossEnvironment { get; set; } = false;

    /// <summary>
    /// Gets or sets additional validation rules for this entity.
    /// </summary>
    public string? ValidationRules { get; set; }

    private IndQuestResults.Result ValidateEnvironmentResult()
    {
        var validEnvironments = new[] { "Production", "Simulation", "Demo", "Any" };
        if (!validEnvironments.Contains(this.Environment))
        {
            var msg = $"Invalid environment '{this.Environment}'. Valid values are: {string.Join(", ", validEnvironments)}";
            return IndQuestResults.Result.WithFailure(msg);
        }

        return IndQuestResults.Result.Success();
    }
}
