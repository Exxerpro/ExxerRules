// <copyright file="CreateProductCommandValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Products.Commands.Create;

/// <summary>
/// Validator for <see cref="CreateProductCommand"/> that ensures all required product data is valid and complete.
/// </summary>
/// <remarks>
/// This validator implements comprehensive validation rules for product creation including:
/// - Product information validation (part number, name, customer)
/// - Business rule validation with JSON format checking
/// - Recipe validation with cycle time constraints
/// - Workflow validation using nested validators.
/// </remarks>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductCommandValidator"/> class
    /// and configures all validation rules for product creation.
    /// </summary>
    public CreateProductCommandValidator()
    {
        // Product must exist
        this.RuleFor(v => v.Product)
            .NotNull().WithMessage("Product must not be null.");

        this.When(v => v.Product is not null, () =>
        {
            this.RuleFor(v => v.Product.PartNumber)
                .NotEmpty().WithMessage("PartNumber must not be empty.")
                .MinimumLength(3).WithMessage("PartNumber must be at least 3 characters long.");

            this.RuleFor(v => v.Product.ProductName)
                .NotEmpty().WithMessage("ProductName must not be empty.")
                .MaximumLength(100).WithMessage("ProductName must not exceed 100 characters.");

            this.RuleFor(v => v.Product.CustomerName)
                .NotEmpty().WithMessage("CustomerName must not be empty.")
                .MaximumLength(100).WithMessage("CustomerName must not exceed 100 characters.");
        });

        // Rule must exist
        this.RuleFor(v => v.Rule)
            .NotNull().WithMessage("Rule must not be null.");

        this.When(v => v.Rule is not null, () =>
        {
            this.RuleFor(v => v.Rule.RuleJson)
                .NotEmpty().WithMessage("RuleJson must not be empty.")
                .Must(this.BeValidJson).WithMessage("RuleJson must be a valid JSON format.");
        });

        // Recipe must exist
        this.RuleFor(v => v.Recipe)
            .NotNull().WithMessage("Recipe must not be null.");

        this.When(v => v.Recipe is not null, () =>
        {
            this.RuleFor(v => v.Recipe.CycleTimeMinimum)
                .GreaterThan(0).WithMessage("CycleTimeMinimum must be greater than 0.");

            this.RuleFor(v => v.Recipe.CycleTimeMaximum)
                .GreaterThan(0).WithMessage("CycleTimeMaximum must be greater than 0.");

            this.RuleFor(v => v.Recipe.CycleTimeMaximum)
                .GreaterThan(v => v.Recipe.CycleTimeMinimum)
                .WithMessage("CycleTimeMaximum must be greater than CycleTimeMinimum.");
        });

        // Workflows
        this.RuleFor(v => v.WorkFlows)
            .NotEmpty().WithMessage("WorkFlows list must not be empty.")
            .Must(wf => wf != null && wf.Count > 0)
            .WithMessage("WorkFlows list must contain at least one valid WorkFlowDto.");

        this.RuleForEach(v => v.WorkFlows).SetValidator(new WorkFlowDtoValidator());
    }

    /// <summary>
    /// Validates that a string contains valid JSON format.
    /// </summary>
    /// <param name="jsonString">The string to validate as JSON.</param>
    /// <returns>True if the string is valid JSON; otherwise, false.</returns>
    /// <remarks>
    /// Uses System.Text.Json.JsonDocument.Parse for validation to ensure
    /// the JSON string can be properly parsed and deserialized.
    /// </remarks>
    private bool BeValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return false;
        }

        try
        {
            System.Text.Json.JsonDocument.Parse(jsonString);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
