namespace IndTrace.VirtualNetwork.Exceptions;

/// <summary>
/// Exception thrown when a requested database block (DB) is not found in the virtual network.
/// </summary>
public class DbNotFoundException : DbAccessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbNotFoundException"/> class with the specified database number.
    /// </summary>
    /// <param name="dbNo">The database number that was not found.</param>
    public DbNotFoundException(int dbNo)
        : base($"DB {dbNo} not found.")
    {
        this.DbNo = dbNo;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbNotFoundException"/> class with a custom message, database number, and optional inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="dbNo">The database number that was not found.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
    public DbNotFoundException(string message, int dbNo, Exception? innerException = null)
        : base(message, innerException)
    {
        this.DbNo = dbNo;
    }
    /// <summary>
    /// Gets or sets the DbNo.
    /// </summary>

    public int DbNo { get; }

    /// <summary>
    /// Gets the HTTP status code associated with this exception.
    /// </summary>
    /// <value>The HTTP status code for this error condition.</value>
    public override int StatusCode { get; }

    /// <summary>
    /// Gets a brief title describing the type of error.
    /// </summary>
    /// <value>A descriptive title for this exception type.</value>
    public override string Title => "DB not found";
}
