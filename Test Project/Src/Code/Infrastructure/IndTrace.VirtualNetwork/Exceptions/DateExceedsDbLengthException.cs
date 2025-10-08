namespace IndTrace.VirtualNetwork.Exceptions;
/// <summary>
/// Represents the DateExceedsDbLengthException.
/// </summary>

public class DateExceedsDbLengthException : DbAccessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateExceedsDbLengthException"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="bytes."">The bytes.".</param>
    public DateExceedsDbLengthException(int dbNo, int dbLength, int dataLength)
        : base($"Data with {dataLength} bytes exceeds length of DB {dbNo} of {dbLength} bytes.")
    {
        this.DbNo = dbNo;
        this.DbLength = dbLength;
        this.DataLength = dataLength;
    }
    /// <summary>
    /// Gets or sets the DataLength.
    /// </summary>

    public int DataLength { get; }
    /// <summary>
    /// Gets or sets the DbLength.
    /// </summary>
    public int DbLength { get; }
    /// <summary>
    /// Gets or sets the DbNo.
    /// </summary>
    public int DbNo { get; }
    /// <summary>
    /// Gets or sets the StatusCode.
    /// </summary>

    public override int StatusCode { get; }

    public override string Title => "Data exceeds DB length";
}
