namespace IndTrace.VirtualNetwork.Exceptions;

/// <summary>
/// Represents an error that occurs when a database number is out of range.
/// </summary>
public class DbOutOfRangeException : DbAccessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbOutOfRangeException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="dbNo">The database number.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public DbOutOfRangeException(string message, int dbNo, Exception? innerException = null)
        : base(message, innerException)
    {
        this.DbNo = dbNo;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbOutOfRangeException"/> class.
    /// </summary>
    /// <param name="dbNo">The database number.</param>
    public DbOutOfRangeException(int dbNo)
        : base($"DB number must be positive, but is {dbNo}.")
    {
        this.DbNo = dbNo;
    }

    /// <summary>
    /// Gets the database number.
    /// </summary>
    public int DbNo { get; }

    /// <summary>
    /// Gets the title of the exception.
    /// </summary>
    public override string Title => "DB out of range";

    /// <summary>
    /// Gets the status code.
    /// </summary>
    public override int StatusCode => throw new NotImplementedException();

    /// <summary>
    /// Throws a <see cref="DbOutOfRangeException"/> if the specified database number is invalid.
    /// </summary>
    /// <param name="dbNo">The database number to validate.</param>
    public static void ThrowIfInvalid(int dbNo)
    {
        if (dbNo < 1)
            throw new DbOutOfRangeException(dbNo);
    }
}
