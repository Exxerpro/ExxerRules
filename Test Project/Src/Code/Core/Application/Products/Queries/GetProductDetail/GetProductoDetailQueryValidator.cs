// <copyright file="GetProductoDetailQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Products.Queries.GetProductDetail;

/// <summary>
/// Represents the GetProductoDetailQueryValidator.
/// </summary>
public class GetProductoDetailQueryValidator : AbstractValidator<GetProductDetailQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductoDetailQueryValidator"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetProductoDetailQueryValidator()
    {
        this.RuleFor(v => v.ProductId).GreaterThan(0).LessThan(100);
    }
}
