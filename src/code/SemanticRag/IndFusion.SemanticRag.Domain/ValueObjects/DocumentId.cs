using System;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.ValueObjects;

/// <summary>
/// Represents a unique document identifier.
/// </summary>
/// <param name="Value">The string value of the document ID.</param>
public record DocumentId(string Value)
{
    /// <summary>
    /// Gets the document ID value.
    /// </summary>
    public string Value { get; init; } = Value;

    /// <summary>
    /// Validates the document ID.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return Result.WithFailure("Document ID cannot be null, empty, or whitespace");

        if (Value.Length > 255)
            return Result.WithFailure("Document ID cannot exceed 255 characters");

        // Check for valid characters (alphanumeric, hyphens, underscores, dots)
        foreach (var c in Value)
        {
            if (!char.IsLetterOrDigit(c) && c != '-' && c != '_' && c != '.')
                return Result.WithFailure("Document ID can only contain letters, digits, hyphens, underscores, and dots");
        }

        return Result.Success();
    }

    /// <summary>
    /// Creates a new DocumentId with validation.
    /// </summary>
    /// <param name="value">The document ID value.</param>
    /// <returns>A Result containing the DocumentId or validation error.</returns>
    public static Result<DocumentId> Create(string value)
    {
        var documentId = new DocumentId(value);
        var validation = documentId.Validate();
        
        return validation.IsFailure 
            ? Result<DocumentId>.WithFailure(validation.Error!)
            : Result<DocumentId>.Success(documentId);
    }

    /// <summary>
    /// Implicit conversion from DocumentId to string.
    /// </summary>
    /// <param name="documentId">The DocumentId to convert.</param>
    /// <returns>The string value.</returns>
    public static implicit operator string(DocumentId documentId) => documentId.Value;

    /// <summary>
    /// Returns a string representation of the DocumentId.
    /// </summary>
    /// <returns>The string value.</returns>
    public override string ToString() => Value;
}
