// <copyright file="NewProductForm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.Performance;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a form for creating new products with validation.
/// </summary>
public class NewProductForm(string name)
{
    /// <summary>
    /// Gets or sets the name of the new product with validation constraints.
    /// </summary>
    [Required]
    [StringLength(10, ErrorMessage = "Name length can't be more than 10.")]
    public string Name { get; set; } = name;
}
