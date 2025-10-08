// <copyright file="GetProductDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Products.Queries.GetProductDetail;

/// <summary>
/// Represents the GetProductDetailQuery.
/// </summary>
public class GetProductDetailQuery : IMonitorRequest<ProductDto>
{
    /// <summary>
    /// Gets or sets the ProductId.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the ProductName.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
}
