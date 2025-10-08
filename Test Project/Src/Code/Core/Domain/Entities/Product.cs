// <copyright file="Product.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents a product entity with identification, versioning, and customer-related information.
/// </summary>
public class Product : AuditableEntity, IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the part number of the product.
    /// </summary>
    public string PartNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is active.
    /// </summary>
    public int IsActive { get; set; }

    /// <summary>
    /// Gets or sets the version of the product.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the customer part number associated with the product.
    /// </summary>
    public string CustomerPartNumber { get; set; }

    /// <summary>
    /// Gets or sets the alias part number for the product.
    /// </summary>
    public string AliasPartNumber { get; set; }

    /// <summary>
    /// Gets or sets the description of the product.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the rule identifier associated with the product.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier associated with the product.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the line identifier associated with the product.
    /// </summary>
    public int LineId { get; set; }

    /// <summary>
    /// Gets or sets the line entity associated with the product.
    /// </summary>
    public Line Line { get; set; }

    /// <summary>
    /// Gets or sets the customer entity associated with the product.
    /// </summary>
    public Customer Customer { get; set; }

    /// <summary>
    /// Gets or sets the customer name associated with the product.
    /// </summary>
    public string CustomerName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Product"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public Product()
    {
        this.PartNumber = null!;
        this.ProductName = null!;
        this.CustomerPartNumber = string.Empty;
        this.AliasPartNumber = string.Empty;
        this.Description = null!;
        this.Line = new Line();
        this.Customer = new Customer();
        this.CustomerName = string.Empty;
    }
}
