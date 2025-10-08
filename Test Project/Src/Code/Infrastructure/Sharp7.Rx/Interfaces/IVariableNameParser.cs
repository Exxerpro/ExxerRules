// <copyright file="IVariableNameParser.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

#nullable enable

namespace Sharp7.Rx.Interfaces;

/// <summary>
/// Defines a contract for parsing variable names into variable addresses for PLC communication.
/// </summary>
public interface IVariableNameParser
{
    /// <summary>
    /// Parses a variable name string into a variable address structure.
    /// </summary>
    /// <param name="input">The variable name string to parse.</param>
    /// <returns>A variable address containing the parsed PLC address information.</returns>
    VariableAddress Parse(string input);
}
