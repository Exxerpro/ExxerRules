namespace IndFusion.Analyzer.Operations;

/// <summary>
/// Represents a collection of null argument validation errors discovered during a single validation pass.
/// </summary>
public class MultipleNullArgumentsError
{
    /// <summary>
    /// Gets the set of null argument errors that compose this aggregate.
    /// </summary>
    public IReadOnlyList<NullArgumentError> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleNullArgumentsError"/> class using the supplied error instances.
    /// </summary>
    /// <param name="errors">Collection of null argument errors to aggregate.</param>
    public MultipleNullArgumentsError(IEnumerable<NullArgumentError> errors)
    {
        Errors = errors.ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleNullArgumentsError"/> class from the parameter names that failed validation.
    /// </summary>
    /// <param name="parameterNames">Names of the parameters that were found to be <c>null</c>.</param>
    public MultipleNullArgumentsError(params string[] parameterNames)
    {
        Errors = parameterNames.Select(name => new NullArgumentError(name)).ToList().AsReadOnly();
    }

    private const string Joiner = ", ";

    /// <summary>
    /// Returns a comma-separated string that lists all parameter names that failed null validation.
    /// </summary>
    /// <returns>A string describing the null parameter failures.</returns>
    public override string ToString() =>
        $"Multiple null parameters: {string.Join(Joiner, Errors.Select(e => e.ParameterName))}";
}
