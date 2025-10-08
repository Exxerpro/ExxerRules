// <copyright file="Rule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents a rule for machine or product configuration, including JSON definition, version, and components.
/// </summary>
public class Rule : AuditableEntity, IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the rule.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Gets or sets the JSON definition of the rule.
    /// </summary>
    [DataType("Markdown")]
    public virtual string RuleJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the rule.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the rule.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the rule.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the rule is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the rule.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier associated with the rule.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the list of rule fragments (components) associated with the rule.
    /// </summary>
    public List<RuleFragment> Components { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of rule functions associated with the rule.
    /// </summary>
    public List<string> RuleFunction { get; set; } = [];
}
