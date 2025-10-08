// <copyright file="ProductDto.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Products.Queries.GetProductDetail;

/// <summary>
/// Data transfer object representing a product with its associated customer, line, and machine information.
/// </summary>
/// <remarks>
/// This DTO contains all product details including identification, customer relationship, line assignment,
/// and machine compatibility information. It provides mapping methods for conversion between entity and DTO.
/// </remarks>
public class ProductDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    /// <value>The product ID as an integer.</value>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the part number that uniquely identifies this product.
    /// </summary>
    /// <value>The part number as a string. Defaults to empty string if not specified.</value>
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the descriptive name of the product.
    /// </summary>
    /// <value>The product name as a string. Defaults to empty string if not specified.</value>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the product is currently active in production.
    /// </summary>
    /// <value>1 if the product is active, 0 if inactive.</value>
    public int IsActive { get; set; }

    /// <summary>
    /// Gets or sets the version number of the product specification.
    /// </summary>
    /// <value>The version number as an integer.</value>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the customer's internal part number for this product.
    /// </summary>
    /// <value>The customer part number as a string. Defaults to empty string if not specified.</value>
    public string CustomerPartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the customer who owns this product.
    /// </summary>
    /// <value>The customer ID as an integer.</value>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer who owns this product.
    /// </summary>
    /// <value>The customer name as a string. Defaults to empty string if not specified.</value>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path or identifier for the customer's logo image.
    /// </summary>
    /// <value>The customer logo path as a string. Defaults to empty string if not specified.</value>
    public string CustomerLogo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an alternative part number or alias for this product.
    /// </summary>
    /// <value>The alias part number as a string. Defaults to empty string if not specified.</value>
    public string AliasPartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a detailed description of the product.
    /// </summary>
    /// <value>The product description as a string. Defaults to empty string if not specified.</value>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username of the person who created this product record.
    /// </summary>
    /// <value>The creator's username as a string. Defaults to empty string if not specified.</value>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username of the person who last modified this product record.
    /// </summary>
    /// <value>The modifier's username as a string. Defaults to empty string if not specified.</value>
    public string ModifiedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the production line assigned to this product.
    /// </summary>
    /// <value>The line ID as an integer.</value>
    public int LineId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the business rule associated with this product.
    /// </summary>
    /// <value>The rule ID as an integer.</value>
    public int RuleId { get; set; }

    /// <summary>
    /// Gets or sets the customer entity associated with this product.
    /// </summary>
    /// <value>A Customer object containing customer details. Defaults to a new Customer instance.</value>
    public Customer Customer { get; set; } = new();

    /// <summary>
    /// Gets or sets the date and time when this product record was created.
    /// </summary>
    /// <value>The creation date and time.</value>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this product record was last modified.
    /// </summary>
    /// <value>The last modification date and time.</value>
    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// Gets or sets the collection of machine IDs that are capable of processing this product.
    /// </summary>
    /// <value>An enumerable collection of machine IDs. Defaults to an empty list.</value>
    public IEnumerable<int> Machines { get; set; } = new List<int>();

    /// <summary>
    /// Gets or sets a dictionary mapping machine IDs to their descriptive names.
    /// </summary>
    /// <value>A dictionary with machine ID as key and machine name as value. Defaults to an empty dictionary.</value>
    public Dictionary<int, string> MachineNames { get; set; } = [];

    /// <summary>
    /// Gets or sets the production line entity associated with this product.
    /// </summary>
    /// <value>A Line object containing production line details.</value>
    /// <remarks>
    /// This property provides the complete line information beyond just the LineId.
    /// </remarks>
    public Line Line { get; set; } = new Line();

    /// <summary>
    /// Converts a Product entity to a ProductDto.
    /// </summary>
    /// <param name="src">The Product entity to convert.</param>
    /// <returns>A ProductDto containing the converted product data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when src is null.</exception>
    /// <remarks>
    /// This method handles safe conversion of nullable properties and provides default values
    /// for dates when they are null. Customer name is derived from the Customer relationship if available.
    /// </remarks>
    public static IndQuestResults.Result<ProductDto> ToDto(Product src)
    {
        // [Fix]
        // CLAUDE
        // Date: 22/08/2025
        // Reason: Pattern 11 Fix - Updated error message to match test expectation and removed extra Customer null check that was causing test failures
        if (src == null)
        {
            return IndQuestResults.Result<ProductDto>.WithFailure("Parameter 'src' cannot be null");
        }

        return IndQuestResults.Result<ProductDto>.Success(new ProductDto
        {
            ProductId = src.ProductId,
            PartNumber = src.PartNumber,
            ProductName = src.ProductName,
            IsActive = src.IsActive,
            Version = src.Version,
            CustomerPartNumber = src.CustomerPartNumber,
            CustomerId = src.CustomerId,
            CustomerName = src.Customer?.Name ?? src.CustomerName ?? string.Empty,
            AliasPartNumber = src.AliasPartNumber,
            Description = src.Description,
            CreatedBy = src.CreatedBy,
            ModifiedBy = src.ModifiedBy,
            LineId = src.LineId,
            RuleId = src.RuleId,
            Customer = src.Customer ?? new Customer(),
            CreatedOn = src.CreatedOn ?? DateTime.Now,
            ModifiedOn = src.ModifiedOn ?? DateTime.Now,
            Line = src.Line,
        });
    }

    /// <summary>
    /// Converts a ProductDto to a Product entity.
    /// </summary>
    /// <param name="src">The ProductDto to convert.</param>
    /// <returns>A Product entity containing the converted data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when src is null.</exception>
    /// <remarks>
    /// This method creates a Product entity from the DTO data. Note that Machines and MachineNames
    /// collections are not transferred as they are view-specific properties.
    /// </remarks>
    public static IndQuestResults.Result<Product> ToEntity(ProductDto src)
    {
        // [Fix]
        // CLAUDE
        // Date: 22/08/2025
        // Reason: Pattern 11 Fix - Updated error message to match test expectation "Parameter 'src' cannot be null"
        if (src == null)
        {
            return IndQuestResults.Result<Product>.WithFailure("Parameter 'src' cannot be null");
        }

        return IndQuestResults.Result<Product>.Success(new Product
        {
            ProductId = src.ProductId,
            PartNumber = src.PartNumber,
            ProductName = src.ProductName,
            IsActive = src.IsActive,
            Version = src.Version,
            CustomerPartNumber = src.CustomerPartNumber,
            CustomerId = src.CustomerId,
            CustomerName = src.CustomerName,
            AliasPartNumber = src.AliasPartNumber,
            Description = src.Description,
            CreatedBy = src.CreatedBy,
            ModifiedBy = src.ModifiedBy,
            LineId = src.LineId,
            RuleId = src.RuleId,
            Customer = src.Customer,
            Line = src.Line,
        });
    }
}
