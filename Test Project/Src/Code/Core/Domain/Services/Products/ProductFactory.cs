using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;
using IndTrace.Domain.Entities;

namespace IndTrace.Domain.Services.Products;

/// <summary>
/// Pure product creation logic without external dependencies.
/// Handles entity creation and advanced ID parsing logic from PartNumber.
/// Domain service - zero infrastructure dependencies.
/// </summary>
public partial class ProductFactory : IProductFactory
{
    // Compiled, thread-safe regex for extracting the last integer (with or without hyphen)
    // Matches: "ABC-123" → 123, "TEST001" → 1, "L687508" → 687508
    private static readonly Regex LastIntegerRegex = new Regex(@"(\d+)$", RegexOptions.Compiled);

    /// <summary>
    /// Creates a result object containing a new product based on the specified input data, customer, and production
    /// line.
    /// </summary>
    /// <param name="productData">The input data used to construct the product. Cannot be null.</param>
    /// <param name="customer">The customer for whom the product is being created. Cannot be null.</param>
    /// <param name="line">The production line associated with the product creation. Cannot be null.</param>
    /// <returns>A <see cref="Result{Product}"/> representing the outcome of the product creation operation. Contains the created
    /// product if successful; otherwise, includes error information.</returns>
    public Result<Product> CreateResultProduct(ProductInput productData, Customer customer, Line line)
    {
        try
        {
            return CreateProduct(productData, customer, line);
        }
        catch (Exception ex)
        {
            return Result<Product>.WithFailure($"An error {ex.Message} {ex.Source}  {ex.StackTrace} occurred while creating the product productData {productData}.for  customer {customer} line {line}");
        }
    }

    /// <summary>
    /// Creates a Product entity with intelligent ID assignment preparation.
    /// Does NOT set ProductId - that's the persistence layer's responsibility.
    /// </summary>
    public Product CreateProduct(ProductInput productData, Customer customer, Line line)
    {
        if (productData is null)
            throw new ArgumentNullException(nameof(productData));
        if (customer is null)
            throw new ArgumentNullException(nameof(customer));
        if (line is null)
            throw new ArgumentNullException(nameof(line));

        var product = new Product
        {
            // Core product properties from input
            PartNumber = productData.PartNumber ?? string.Empty,
            ProductName = productData.ProductName ?? string.Empty,
            Description = productData.Description ?? string.Empty,
            CustomerPartNumber = productData.CustomerPartNumber ?? string.Empty,
            AliasPartNumber = productData.AliasPartNumber ?? string.Empty,

            // Customer relationship
            CustomerId = customer.CustomerId,
            CustomerName = customer.Name ?? string.Empty,
            Customer = customer,

            // Line relationship
            LineId = line.LineId,
            Line = line,

            // Defaults from original handler
            IsActive = productData.IsActive > 0 ? productData.IsActive : 1,
            Version = productData.Version > 0 ? productData.Version : 1,

            // Audit fields
            CreatedBy = productData.CreatedBy ?? string.Empty,
            CreatedOn = DateTime.Now,
            ModifiedBy = productData.CreatedBy ?? string.Empty,
            ModifiedOn = DateTime.Now,

            // ProductId is NOT set here - persistence layer handles this
            // based on parsing logic and availability checks
            ProductId = 0,

            // RuleId will be set after rule creation
            RuleId = 0
        };

        return product;
    }

    /// <summary>
    /// Attempts to parse the last integer from a part number string.
    /// Preserves EXACT parsing logic from original handler.
    /// </summary>
    /// <param name="partNumber">The part number string to parse</param>
    /// <returns>Tuple with success flag and parsed integer value</returns>
    /// <summary>
    /// Attempts to parse the last integer from a part number string.
    /// Preserves EXACT parsing logic from original handler.
    /// </summary>
    /// <param name="partNumber">The part number string to parse</param>
    /// <returns>Tuple with success flag and parsed integer value</returns>
    public (bool Success, int ParsedId) TryParseLastInteger(string partNumber)
    {
        // Check for null or empty input - exact match to original
        if (string.IsNullOrEmpty(partNumber))
        {
            return (false, 0);
        }

        // Use pattern to find trailing digits in the part number
        // Matches: "ABC-123" → 123, "TEST001" → 1, "L687508" → 687508
        var match = LastIntegerRegex.Match(partNumber);

        if (match.Success && int.TryParse(match.Groups[1].Value, out int parsedValue))
        {
            // If a number is found and is within the range of int, return true and the value
            // Note: int.TryParse guarantees parsedValue is within int range, no additional checks needed
            return (true, Math.Abs(parsedValue));
        }

        // No number found - try recursive parsing for any digits in the string
        return TryParseAnyNumber(partNumber);
    }

    /// <summary>
    /// Fallback method to extract any number from the part number when no trailing digits found.
    /// Searches recursively for any numeric sequence in the part number.
    /// </summary>
    /// <param name="partNumber">The part number to search</param>
    /// <returns>Tuple with success flag and extracted number, or (false, 0) if no numbers found</returns>
    private (bool Success, int ParsedId) TryParseAnyNumber(string partNumber)
    {
        if (string.IsNullOrEmpty(partNumber))
        {
            return (false, 0);
        }

        // Try to find any sequence of digits in the part number
        var anyNumberRegex = new Regex(@"(\d+)", RegexOptions.Compiled);
        var matches = anyNumberRegex.Matches(partNumber);

        if (matches.Count > 0)
        {
            // Try each numeric sequence found, starting from the last one
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                if (int.TryParse(matches[i].Groups[1].Value, out int parsedValue) && parsedValue > 0)
                {
                    return (true, Math.Abs(parsedValue));
                }
            }
        }

        // No valid numbers found in part number - will need database-generated ID
        return (false, 0);
    }

    /// <summary>
    /// Gets a dynamic offset based on the width of the parsed number.
    /// Preserves EXACT offset calculation from original handler.
    /// </summary>
    /// <param name="parsedNumber">The parsed number from PartNumber</param>
    /// <returns>The dynamic offset for ID calculation</returns>
    public int GetDynamicOffset(int parsedNumber)
    {
        // Calculate offset based on the width of the parsed number
        // This maintains visual comparison while avoiding conflicts
        var numberWidth = parsedNumber.ToString().Length;

        // Exact formula from original handler:
        // For single digit: add 10 (e.g., 5 -> 15)
        // For double digit: add 100 (e.g., 23 -> 123)
        // For triple digit: add 1000 (e.g., 123 -> 1123)
        // For four digit: add 10000 (e.g., 1234 -> 11234)
        // And so on...
        var offset = (int)Math.Pow(10, numberWidth);

        return offset;
    }
}
