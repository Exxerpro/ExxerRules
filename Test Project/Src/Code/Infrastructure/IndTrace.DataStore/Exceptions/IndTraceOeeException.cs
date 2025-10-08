using System.Text.Json;

namespace IndTrace.DataStore.Exceptions
{
    /// <summary>
    /// Represents errors that occur during OEE operations, particularly related to PLC communication.
    /// </summary>
    [Serializable]
    public class IndTraceOeeException : Exception
    {
        /// <summary>
        /// Gets the IP address associated with the error.
        /// </summary>
        public string IpAddress { get; init; } = string.Empty; // Ensure non-null default value

        /// <summary>
        /// Gets the variable key associated with the error.
        /// </summary>
        public string VariableKey { get; init; } = string.Empty; // Ensure non-null default value

        /// <summary>
        /// Gets the duplicate address that caused the error, if applicable.
        /// </summary>
        public string DuplicateAddress { get; init; } = string.Empty; // Ensure non-null default value

        /// <summary>
        /// Initializes a new instance of the <see cref="IndTraceOeeException"/> class with detailed error information.
        /// </summary>
        /// <param name="ipAddress">The IP address where the error occurred.</param>
        /// <param name="key">The variable key associated with the error.</param>
        /// <param name="address">The duplicate address that caused the error.</param>
        /// <param name="message">The error message.</param>
        public IndTraceOeeException(string ipAddress, string key, string address, string message)
            : base(message)
        {
            this.IpAddress = ipAddress;
            this.VariableKey = key;
            this.DuplicateAddress = address;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndTraceOeeException"/> class.
        /// </summary>
        public IndTraceOeeException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndTraceOeeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public IndTraceOeeException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndTraceOeeException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public IndTraceOeeException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Converts the exception to a JSON string representation.
        /// </summary>
        /// <returns>A JSON string containing the exception details.</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Creates an exception instance from a JSON string representation.
        /// </summary>
        /// <param name="json">The JSON string containing exception details.</param>
        /// <returns>A new instance of <see cref="IndTraceOeeException"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when deserialization results in a null object.</exception>
        public static IndTraceOeeException FromJson(string json)
        {
            var deserialized = JsonSerializer.Deserialize<IndTraceOeeException>(json);
            if (deserialized == null)
            {
                throw new InvalidOperationException("Deserialization resulted in a null object.");
            }
            return deserialized;
        }
    }
}
