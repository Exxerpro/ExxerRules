namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Options for code entity extraction.
/// </summary>
/// <param name="ExtractClasses">Whether to extract classes.</param>
/// <param name="ExtractMethods">Whether to extract methods.</param>
/// <param name="ExtractInterfaces">Whether to extract interfaces.</param>
/// <param name="ExtractProperties">Whether to extract properties.</param>
/// <param name="ExtractFields">Whether to extract fields.</param>
/// <param name="ExtractNamespaces">Whether to extract namespaces.</param>
/// <param name="IncludeAccessModifiers">Whether to include access modifiers.</param>
/// <param name="IncludeParameters">Whether to include method parameters.</param>
/// <param name="IncludeReturnTypes">Whether to include return types.</param>
public readonly record struct CodeExtractionOptions(
    bool ExtractClasses = true,
    bool ExtractMethods = true,
    bool ExtractInterfaces = true,
    bool ExtractProperties = true,
    bool ExtractFields = true,
    bool ExtractNamespaces = true,
    bool IncludeAccessModifiers = true,
    bool IncludeParameters = true,
    bool IncludeReturnTypes = true)
{
    /// <summary>
    /// Default options for comprehensive code extraction.
    /// </summary>
    public static CodeExtractionOptions Comprehensive() => new(
        ExtractClasses: true,
        ExtractMethods: true,
        ExtractInterfaces: true,
        ExtractProperties: true,
        ExtractFields: true,
        ExtractNamespaces: true,
        IncludeAccessModifiers: true,
        IncludeParameters: true,
        IncludeReturnTypes: true);

    /// <summary>
    /// Options for minimal code extraction.
    /// </summary>
    public static CodeExtractionOptions Minimal() => new(
        ExtractClasses: true,
        ExtractMethods: false,
        ExtractInterfaces: false,
        ExtractProperties: false,
        ExtractFields: false,
        ExtractNamespaces: true,
        IncludeAccessModifiers: false,
        IncludeParameters: false,
        IncludeReturnTypes: false);
}