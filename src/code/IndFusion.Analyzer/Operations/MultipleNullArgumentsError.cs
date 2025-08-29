namespace IndFusion.Analyzers.Operations;

/// <summary>
/// Represents multiple null argument validation errors
/// </summary>
public class MultipleNullArgumentsError
{
	/// <summary>
	/// Collection of null argument errors
	/// </summary>
	public IReadOnlyList<NullArgumentError> Errors { get; }

	/// <summary>
	/// Initializes a new instance with multiple null argument errors
	/// </summary>
	/// <param name="errors">Collection of null argument errors</param>
	public MultipleNullArgumentsError(IEnumerable<NullArgumentError> errors)
	{
		Errors = errors.ToList().AsReadOnly();
	}

	/// <summary>
	/// Initializes a new instance with parameter names
	/// </summary>
	/// <param name="parameterNames">Names of null parameters</param>
	public MultipleNullArgumentsError(params string[] parameterNames)
	{
		Errors = parameterNames.Select(name => new NullArgumentError(name)).ToList().AsReadOnly();
	}

	private const string Joiner = ", ";

	/// <summary>
	/// Returns string representation of all errors
	/// </summary>
	public override string ToString() =>
		$"Multiple null parameters: {string.Join(Joiner, Errors.Select(e => e.ParameterName))}";
}

