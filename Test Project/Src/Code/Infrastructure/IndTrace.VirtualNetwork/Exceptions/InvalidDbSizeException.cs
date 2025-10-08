namespace IndTrace.VirtualNetwork.Exceptions;

/// <summary>
/// Represents an error that occurs when an invalid database size is specified.
/// </summary>
/// <param name="size">The invalid size that was provided.</param>
public class InvalidDbSizeException(int size) : DbAccessException($"DB size must be > 0 but was {size}.")
{
    /// <summary>
    /// Gets the invalid size that was provided.
    /// </summary>
    public int Size { get; } = size;

    /// <summary>
    /// Gets the status code.
    /// </summary>
    public override int StatusCode { get; }
    /// <summary>
    /// Gets the title of the exception.
    /// </summary>
    public override string Title => "Invalid DB size";

    /// <summary>
    /// Throws an <see cref="InvalidDbSizeException"/> if the specified size is invalid.
    /// </summary>
    /// <param name="size">The size to validate.</param>
    public static void ThrowIfInvalid(int size)
    {
        if (size < 1)
            throw new InvalidDbSizeException(size);
    }
}
