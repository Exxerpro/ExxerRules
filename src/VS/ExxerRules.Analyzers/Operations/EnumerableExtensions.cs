namespace ExxerRules.Analyzers.Operations;

/// <summary>
/// Provides extension methods for working with enumerables.
/// </summary>
public static class EnumerableExtensions
{
	/// <summary>
	/// Checks if the enumerable is not null and contains at least one element.
	/// </summary>
	/// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
	/// <param name="source">The enumerable to check.</param>
	/// <returns>True if the enumerable is not null and contains at least one element; otherwise, false.</returns>
	public static bool NotNullOrEmpty<T>(this IEnumerable<T> source) => source != null && source.Any();
}
