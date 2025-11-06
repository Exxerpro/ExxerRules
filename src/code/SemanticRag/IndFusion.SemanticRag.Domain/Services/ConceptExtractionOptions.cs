namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Options for concept extraction.
/// </summary>
/// <param name="MinFrequency">Minimum frequency for concepts.</param>
/// <param name="MaxConcepts">Maximum number of concepts to extract.</param>
/// <param name="IncludeSynonyms">Whether to include synonyms.</param>
/// <param name="IncludeDefinitions">Whether to include concept definitions.</param>
/// <param name="MinLength">Minimum concept length in characters.</param>
/// <param name="MaxLength">Maximum concept length in characters.</param>
public readonly record struct ConceptExtractionOptions(
    int MinFrequency = 2,
    int MaxConcepts = 50,
    bool IncludeSynonyms = true,
    bool IncludeDefinitions = true,
    int MinLength = 3,
    int MaxLength = 50)
{
    /// <summary>
    /// Validates the concept extraction options.
    /// </summary>
    public Result Validate()
    {
        if (MinFrequency < 1)
            return Result.WithFailure("MinFrequency must be at least 1");

        if (MaxConcepts <= 0)
            return Result.WithFailure("MaxConcepts must be greater than 0");

        if (MinLength < 1)
            return Result.WithFailure("MinLength must be at least 1");

        if (MaxLength < MinLength)
            return Result.WithFailure("MaxLength must be greater than or equal to MinLength");

        return Result.Success();
    }
}