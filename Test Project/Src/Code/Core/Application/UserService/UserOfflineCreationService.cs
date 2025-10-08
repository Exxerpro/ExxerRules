// <copyright file="UserOfflineCreationService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.UserService;

using System.Security.Cryptography;
using NSubstitute.Core;

/// <summary>
/// Represents the UserOfflineCreationService.
/// </summary>
public class UserOfflineCreationService : IUserOfflineCreationService
{
    private readonly Random random = new();
    public const string SharedSecret = "SecretKey"; // Shared secret known only to authorized users

    private const long PrimeMultiplier = 777_767_777L;
    private const long PrimeModulo = 323_232_323L;
    private const long MaxDigits = 100_000_000L;

    /// <summary>
    /// Gets the LastGeneratedChallenge.
    /// </summary>
    public List<string> LastGeneratedChallenge { get; } = [];

    /// <summary>
    /// Gets the LastGeneratedResponse.
    /// </summary>
    public Dictionary<string, string> LastGeneratedResponse { get; } = [];

    /// <summary>
    /// Generates an 8-digit random challenge number.
    /// </summary>
    /// <returns></returns>
    public string GenerateChallenge()
    {
        var challenge = this.random.Next(10000000, 99999999).ToString("D8"); // 8-digit challenge
        this.LastGeneratedChallenge.Add(challenge);
        var response = this.ComputeResponse(challenge);
        this.LastGeneratedResponse.Add(challenge, response);
        return challenge;
    }

    /// <summary>
    /// Verifies the response against the challenge using the shared secret algorithm.
    /// </summary>
    /// <returns></returns>
    public bool ValidateResponse(string challenge, string response)
    {
        if (challenge is null || response is null)
        {
            return false;
        }

        var resultChallenge = long.TryParse(challenge, out var challengeNumeric);
        if (resultChallenge == false)
        {
            return false;
        }

        var resultResponse = long.TryParse(response, out var responseNumeric);
        if (resultResponse == false)
        {
            return false;
        }

        return this.VerifyResponse(challengeNumeric, responseNumeric);
    }

    /// <summary>
    /// Verifies the response against the challenge using the shared secret algorithm.
    /// </summary>
    /// <returns></returns>
    public bool ValidateResponse(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            return false;
        }

        var responseExist = this.LastGeneratedResponse.Any(x =>
           /* x.Value.Substring(0, 6) == response.Substring(0, 6)*/
           x.Value == response);
        return responseExist;
    }

    /// <summary>
    /// Executes VerifyResponse operation.
    /// </summary>
    /// <param name="challenge">The challenge.</param>
    /// <param name="response">The response.</param>
    /// <returns>The result of VerifyResponse.</returns>
    public bool VerifyResponse(long challenge, long response)
    {
        var expectedResponse = this.ComputeResponse(challenge);
        var result = expectedResponse == response;
        return result;
    }

    /// <summary>
    /// Computes the response for a given challenge using the shared secret algorithm.
    /// </summary>
    /// <returns></returns>
    public long ComputeResponse(long challenge)
    {
        var largeNumber = challenge * PrimeMultiplier; // Multiply by a large prime number
        var intermediateValue = largeNumber % PrimeModulo; // Modulo prime number
        var result = intermediateValue % MaxDigits; // Modulo prime number
        return result;
    }

    /// <summary>
    /// Computes the response for a given challenge using the shared secret algorithm.
    /// </summary>
    /// <returns></returns>
    public string ComputeResponse(string challenge)
    {
        // we must validate string lenght ??
        // one test is expecting that
        // yes we have 10000000 to 99999999
        if (string.IsNullOrEmpty(challenge) || challenge.Length != 8)
        {
            return string.Empty;
        }

        var resultChallenge = long.TryParse(challenge, out var challengeNumeric);
        if (resultChallenge == false)
        {
            return string.Empty;
        }

        var result = this.ComputeResponse(challengeNumeric);
        return result.ToString("D8");
    }

    /// <summary>
    /// Computes the response for a given challenge using the shared secret algorithm.
    /// </summary>
    /// <returns></returns>
    public long ComputeResponseWithSharedSecret(long challenge)
    {
        var intermediateValue = challenge * PrimeMultiplier; // Multiply by a large prime number
        var stringWithSecret = intermediateValue.ToString() + SharedSecret; // Append shared secret string
        var hash = ComputeSha256Hash(stringWithSecret); // Compute SHA-256 hash
        var first8Characters = hash.Substring(0, 8); // Take the first 8 characters of the hash
        var result = Convert.ToInt64(first8Characters, 16); // Convert to a long integer interpreting as hexadecimal

        return result;
    }

    /// <summary>
    /// Computes the SHA-256 hash of the input string.
    /// </summary>
    /// <returns></returns>
    public static string ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        var builder = new StringBuilder();
        foreach (var byteValue in bytes)
        {
            builder.Append(byteValue.ToString("x2"));
        }

        return builder.ToString();
    }
}
