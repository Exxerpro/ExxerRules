namespace IndTrace.VirtualNetwork.Exceptions;

/// <summary>
/// Represents an error that occurs when a database already exists.
/// </summary>
public class DbExistsException : DbAccessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbExistsException"/> class.
    /// </summary>
    /// <param name="dbNo">The database number.</param>
    public DbExistsException(int dbNo)
        : base($"DB {dbNo} already exists.")
    {
        this.DbNo = dbNo;
    }

    /// <summary>
    /// Gets the database number.
    /// </summary>
    public int DbNo { get; }

    /// <summary>
    /// Gets the status code.
    /// </summary>
    public override int StatusCode { get; }
    /// <summary>
    /// Gets the title of the exception.
    /// </summary>
    public override string Title => "DB already exists";
}
