// <copyright file="IProductService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Products.Events;

namespace IndTrace.Application.Models.Interfaces;

using IndTrace.Application.Products.Commands.Create;
using IndTrace.Application.Products.Services;

/// <summary>
/// Provides product-related operations and services.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure interface is open for extension but closed for modification (OCP - SOLID). Consider using default interface methods or extension methods for future-proofing.
public interface IProductService
{
    /// <summary>
    /// Executes the command to create a new product.
    /// </summary>
    /// <param name="productDto">The product creation data transfer object.</param>
    /// <returns>A result containing the product created event.</returns>
    Task<Result<ProductCreatedEvent>> ExecuteCreateProductCommand(ProductCreationDto productDto);
}
