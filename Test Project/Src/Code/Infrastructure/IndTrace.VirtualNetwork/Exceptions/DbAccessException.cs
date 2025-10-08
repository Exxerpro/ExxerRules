namespace IndTrace.VirtualNetwork.Exceptions;

/// <summary>
/// Represents a base class for database access exceptions.
/// </summary>
public abstract class DbAccessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbAccessException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected DbAccessException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public abstract int StatusCode { get; }

    /// <summary>
    /// Gets the title or brief description of the exception.
    /// </summary>
    public abstract string Title { get; }
}
