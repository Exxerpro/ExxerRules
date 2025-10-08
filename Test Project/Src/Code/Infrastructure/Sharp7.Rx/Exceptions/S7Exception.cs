// <copyright file="S7Exception.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx;

/// <summary>
/// The base class for all S7 related exceptions.
/// </summary>
public abstract class S7Exception : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="S7Exception"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected S7Exception(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="S7Exception"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    protected S7Exception(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Represents an exception that occurs during S7 communication.
/// </summary>
public class S7CommunicationException : S7Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="S7CommunicationException"/> class with a specified error message, S7 error code, and S7 error text.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="s7ErrorCode">The S7 error code.</param>
    /// <param name="s7ErrorText">The S7 error text.</param>
    public S7CommunicationException(string message, int s7ErrorCode, string s7ErrorText)
        : base(message)
    {
        this.S7ErrorCode = s7ErrorCode;
        this.S7ErrorText = s7ErrorText;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="S7CommunicationException"/> class with a specified error message, a reference to the inner exception that is the cause of this exception, S7 error code, and S7 error text.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    /// <param name="s7ErrorCode">The S7 error code.</param>
    /// <param name="s7ErrorText">The S7 error text.</param>
    public S7CommunicationException(string message, Exception innerException, int s7ErrorCode, string s7ErrorText)
        : base(message, innerException)
    {
        this.S7ErrorCode = s7ErrorCode;
        this.S7ErrorText = s7ErrorText;
    }

    /// <summary>
    /// Gets the S7 error code.
    /// </summary>
    public int S7ErrorCode { get; }

    /// <summary>
    /// Gets the S7 error text.
    /// </summary>
    public string S7ErrorText { get; }
}

/// <summary>
/// Represents an exception that occurs when there is a data type mismatch during S7 communication.
/// </summary>
public class DataTypeMissmatchException : S7Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeMissmatchException"/> class with a specified error message, type, and variable address.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="type">The type that caused the mismatch.</param>
    /// <param name="address">The variable address involved in the mismatch.</param>
    internal DataTypeMissmatchException(string message, Type type, VariableAddress address)
        : base(message)
    {
        this.Type = type;
        this.Address = address.ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeMissmatchException"/> class with a specified error message, a reference to the inner exception that is the cause of this exception, type, and variable address.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    /// <param name="type">The type that caused the mismatch.</param>
    /// <param name="address">The variable address involved in the mismatch.</param>
    internal DataTypeMissmatchException(string message, Exception innerException, Type type, VariableAddress address)
        : base(message, innerException)
    {
        this.Type = type;
        this.Address = address.ToString();
    }

    /// <summary>
    /// Gets the address of the variable that caused the mismatch.
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Gets the type that caused the mismatch.
    /// </summary>
    public Type Type { get; }
}

/// <summary>
/// Represents an exception that occurs when an unsupported S7 type is encountered.
/// </summary>
public class UnsupportedS7TypeException : S7Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedS7TypeException"/> class with a specified error message, type, and variable address.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="type">The unsupported type.</param>
    /// <param name="address">The variable address involved.</param>
    internal UnsupportedS7TypeException(string message, Type type, VariableAddress address)
        : base(message)
    {
        this.Type = type;
        this.Address = address.ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedS7TypeException"/> class with a specified error message, a reference to the inner exception that is the cause of this exception, type, and variable address.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    /// <param name="type">The unsupported type.</param>
    /// <param name="address">The variable address involved.</param>
    internal UnsupportedS7TypeException(string message, Exception innerException, Type type, VariableAddress address)
        : base(message, innerException)
    {
        this.Type = type;
        this.Address = address.ToString();
    }

    /// <summary>
    /// Gets the address of the variable involved.
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Gets the unsupported type.
    /// </summary>
    public Type Type { get; }
}

/// <summary>
/// Represents an exception that occurs when an invalid S7 address is provided.
/// </summary>
public class InvalidS7AddressException : S7Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidS7AddressException"/> class with a specified error message and the invalid input string.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="input">The invalid input string.</param>
    public InvalidS7AddressException(string message, string input)
        : base(message)
    {
        this.Input = input;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidS7AddressException"/> class with a specified error message, a reference to the inner exception that is the cause of this exception, and the invalid input string.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    /// <param name="input">The invalid input string.</param>
    public InvalidS7AddressException(string message, Exception innerException, string input)
        : base(message, innerException)
    {
        this.Input = input;
    }

    /// <summary>
    /// Gets the invalid input string.
    /// </summary>
    public string Input { get; }
}
