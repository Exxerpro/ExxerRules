// <copyright file="DbTagMapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Data;

using System.Data;
using System.Text.RegularExpressions;
using Dapper;

/// <summary>
/// Represents the DbTagMapper.
/// </summary>
public class DbTagMapper(IDbConnection db, ILogger<DbTagMapper> logger) : IDbTagMapper
{
    private static readonly Regex DbStringFormatRegex = new(@"^DB\d+\.(S|STRING)\d+\.\d+$", RegexOptions.Compiled);
    private static readonly Regex DbInt16FormatRegex = new(@"^DB\d+\.(DBW\d+|W\d+)$", RegexOptions.Compiled);
    private static readonly Regex DbInt32FormatRegex = new(@"^DB\d+\.(DINT\d+|DBD\d+)$", RegexOptions.Compiled);
    private static readonly Regex DbBoolFormatRegex = new(@"^DB\d+\.X\d+\.[0-7]$", RegexOptions.Compiled);
    private static readonly Regex DbInt32OrRealFormatRegex = new(@"^DB\d+\.(DINT\d+|DBD\d+|D\d+|REAL\d+|R\d+)$", RegexOptions.Compiled);

    // FIX [CRITICAL] CORRECT THIS REGEX TO MATCH BOOLS
    // ABR 17 MAY 2025
    // Corrected regex to match Siemens-style boolean DB addresses like DB257.X10.0

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate DB tag mapper logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public async Task<Dictionary<string, VariableS7>> GetTagsAsync(int machineId)
    {
        try
        {
            const string sql = @"
			SELECT DISTINCT MachineId Name, Address, NetType, 0 as ValueInt, '' as ValueString
			FROM Variables
			WHERE MachineId = @MachineId AND IsActive = 1";

            var query = await db.QueryAsync<(int MachineId, string Name, string Address, string NetType, int ValueInt, string ValueString)>(
                sql, new { MachineId = machineId });

            var result = query.ToList();
            var records = new Dictionary<string, VariableS7>();
            var issues = new List<string>();

            foreach (var q in result)
            {
                if (!PlcTagNames.AllReferenceTagTypes.TryGetValue(q.Name, out var expectedType))
                {
                    // silently skip unexpected tags
                    continue;
                }

                var resolvedType = Type.GetType(q.NetType, false) ?? Type.GetType($"System.{q.NetType}");
                if (resolvedType?.FullName != expectedType.FullName)
                {
                    issues.Add($"Tag '{q.Name}' has NetType '{q.NetType}' (expected: '{expectedType.FullName}').");
                    continue;
                }

                if (!IsValidFormat(q.Address, expectedType))
                {
                    issues.Add($"Tag '{q.Name}' has invalid Address format '{q.Address}' for type '{expectedType.FullName}'.");
                    continue;
                }

                records[q.Name] = new VariableS7
                {
                    MachineId = q.MachineId,
                    Name = q.Name,
                    Address = q.Address,
                    NetType = q.NetType,
                    ValueInt = q.ValueInt,
                    ValueString = q.ValueString,
                    Type = resolvedType ?? typeof(object),
                };
            }

            if (issues.Count <= 0)
            {
                return records;
            }

            var errorMessage = "❌ Tag validation failed:\n" + string.Join("\n", issues);
            logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving VariableS7 list for MachineId {MachineId}.", machineId);
            return [];
        }
    }

    private static bool IsValidFormat(string address, Type type) =>
        type switch
        {
            _ when type == typeof(string) => DbStringFormatRegex.IsMatch(address),
            _ when type == typeof(short) => DbInt16FormatRegex.IsMatch(address),
            _ when type == typeof(int) => DbInt32FormatRegex.IsMatch(address),
            _ when type == typeof(bool) => DbBoolFormatRegex.IsMatch(address),
            _ when type == typeof(double) => DbInt32OrRealFormatRegex.IsMatch(address),
            _ => true, // fallback to permissive if not validated
        };
}
