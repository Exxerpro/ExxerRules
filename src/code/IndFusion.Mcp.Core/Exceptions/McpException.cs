using System;
using System.Collections.Generic;
using System.Text;

namespace IndFusion.Mcp.Mcp.Core.Exceptions;

/// <summary>
/// Represents errors that occur within the MCP core components.
/// </summary>
public class McpException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="McpException"/> class with a default message.
    /// </summary>
    public McpException() : base("Mcp Exception occurred")
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="McpException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public McpException(string message) : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="McpException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public McpException(string message, Exception innerException) : base(message, innerException)
    { }
}
