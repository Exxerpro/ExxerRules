// <copyright file="S7ErrorCodes.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

#nullable enable

namespace Sharp7.Rx;

/// <summary>
/// Provides utilities for handling and interpreting Siemens S7 PLC error codes.
/// </summary>
/// <remarks>
/// This static class contains error code mappings and logic for determining whether
/// specific error codes indicate connection issues or user errors. It supports
/// the Sharp7 driver's error handling and connection management strategies.
/// </remarks>
public static class S7ErrorCodes
{
    /// <summary>
    /// Contains error codes that do not indicate a connection loss.
    /// </summary>
    /// <remarks>
    /// This list is not exhaustive and should be considered work in progress.
    /// These error codes represent user errors or temporary issues that don't
    /// require connection reestablishment.
    /// </remarks>
    private static readonly HashSet<int> NotDisconnectedErrorCodes =
    [
        0x000000, // OK
        0xC00000, // CPU: Item not available
        0x900000 // CPU: Address out of range
    ];

    /// <summary>
    /// Contains additional descriptive text for specific error codes.
    /// </summary>
    /// <remarks>
    /// This dictionary provides human-readable explanations for common error scenarios
    /// to help with troubleshooting and debugging PLC communication issues.
    /// </remarks>
    private static readonly IReadOnlyDictionary<int, string> AdditionalErrorTexts = new Dictionary<int, string>
    {
        { 0xC00000, "This happens when the DB does not exist." },
        { 0x900000, "This happens when the DB is not long enough." },
        {
            0x40000, """
                     This can happen when the cpu MPI address or rack is wrong, the DB is "optimized", or "PUT/GET communication" is not enabled.
                     See https://snap7.sourceforge.net/snap7_client.html#target_compatibility.
                     """
        },
    };

    /// <summary>
    /// Determines whether a connection should be assumed lost based on the error code.
    /// </summary>
    /// <param name="errorCode">The S7 error code to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the error code indicates a connection loss and reconnection should be attempted;
    /// <c>false</c> if the error code indicates a user error or temporary issue that doesn't require reconnection.
    /// </returns>
    /// <remarks>
    /// Some error codes indicate connection lost, in which case, the driver tries to reestablish connection.
    /// Other error codes indicate a user error, like reading from an unavailable DB or exceeding
    /// the DBs range. In this case the driver should not consider the connection to be lost.
    /// </remarks>
    public static bool AssumeConnectionLost(int errorCode) =>
        !NotDisconnectedErrorCodes.Contains(errorCode);

    /// <summary>
    /// Retrieves additional descriptive text for a specific error code.
    /// </summary>
    /// <param name="errorCode">The S7 error code to look up.</param>
    /// <returns>
    /// A descriptive string explaining the error code, or <c>null</c> if no additional text is available.
    /// </returns>
    /// <remarks>
    /// This method provides human-readable explanations for common error scenarios
    /// to assist with troubleshooting and debugging PLC communication issues.
    /// </remarks>
    public static string? GetAdditionalErrorText(int errorCode) =>
        AdditionalErrorTexts.GetValueOrDefault(errorCode);
}
