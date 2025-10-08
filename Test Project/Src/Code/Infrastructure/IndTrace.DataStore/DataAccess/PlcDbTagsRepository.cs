using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.ModelsComs;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace IndTrace.DataStore.DataAccess;
/// <summary>
/// Represents the PlcDbTagsRepository.
/// </summary>

public class PlcDbTagsRepository(IOptions<PlcDbOptions> options, ILogger<PlcDbTagsRepository> logger, IDbConnection db) : ITagsRepository
{
    private readonly string connectionString = options.Value.ConnectionString;

    /// <summary>
    /// Retrieves all PLCs asynchronously from the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An enumerable of PLC data.</returns>
    public async Task<IEnumerable<PlcData>> GetPlcsAsync(CancellationToken cancellationToken)
    {
        try
        {
            //TODO
            //FALBACK FOR MALFORMED OPTIONS JSON

            var sql = @"
						SELECT
							p.PlcId,
							p.Enabled,
							p.MachineId,
							p.Name,
							p.IpAddress,
							p.PlcType,
							p.PlcBrand,
							COALESCE(oj.Rack, 0) AS RackNumber,
							COALESCE(oj.Slot, 2) AS CpuMpiAddress,
							COALESCE(ports.Port, 102) AS Port
						FROM Plcs p
						OUTER APPLY OPENJSON(p.Options)
						WITH (
							Rack INT,
							Slot INT,
							TSAP NVARCHAR(10),
							Protocols NVARCHAR(MAX) AS JSON
						) oj
						OUTER APPLY OPENJSON(oj.Protocols)
						WITH (
							Name NVARCHAR(50),
							Ports NVARCHAR(MAX) AS JSON
						) proto
						OUTER APPLY OPENJSON(proto.Ports) WITH (Port INT '$') ports
						WHERE p.Enabled = 1 AND (proto.Name = 'S7' OR proto.Name IS NULL);
						";

            //await using var connection = new SqlConnection(connectionString);
            var plcs = await db.QueryAsync<PlcData>(sql, cancellationToken);
            return plcs;
        }
        catch (SqlException e)
        {
            logger.LogInformation(this.connectionString);
            logger.LogError(e, "Error fetching PLCs from database: {Message}", e.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching PLCs from database: {Message}", e.Message);
            throw;
        }
    }

    /// <summary>
    /// Retrieves distinct database information asynchronously from the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An enumerable of distinct database information.</returns>
    public async Task<IEnumerable<Db>> GetDistinctDbInfosAsync(CancellationToken cancellationToken)
    {
        try
        {
            var sql = @"
				SELECT DISTINCT
					CAST(SUBSTRING(Address, 3, CHARINDEX('.', Address) - 3) AS INT) AS Id
				FROM Variables";

            //await using var connection = new SqlConnection(connectionString);
            var dbs = await db.QueryAsync<Db>(sql, cancellationToken);
            return dbs;
        }
        catch (SqlException e)
        {
            logger.LogInformation(this.connectionString);
            logger.LogError(e, "Error fetching PLCs from database: {Message}", e.Message);
            throw;
        }
        catch (Exception e)
        {
            // Log the error message
            logger.LogError(e, "Error fetching distinct DBs from database: {Message}", e.Message);
            throw;
        }
    }

    /// <summary>
    /// Retrieves tags grouped by machine asynchronously for all machines.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping machine IDs to their tags.</returns>
    public async Task<Dictionary<int, Dictionary<string, VariableS7>>> GetTagsGroupedByMachineAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            const string sql = @"
			SELECT MachineId, Name, Address, NetType, 0 as ValueInt, 0 as ValueReal, 0 as ValueBool, '' as ValueString
			FROM Variables
			WHERE IsActive = 1";

            var query = await db.QueryAsync<(int MachineId, string Name, string Address, string NetType, int ValueInt, string ValueString)>(sql, cancellationToken);
            var groupedRecords = new Dictionary<int, Dictionary<string, VariableS7>>();
            var issues = new List<string>();

            foreach (var q in query)
            {
                if (!PlcTagNames.AllPerformanceTagTypes.TryGetValue(q.Name, out var expectedType) &&
                    !PlcTagNames.AllReferenceTagTypes.TryGetValue(q.Name, out expectedType))
                    continue;
                var resolvedType = Type.GetType(q.NetType, false) ?? Type.GetType($"System.{q.NetType}");
                if (resolvedType?.FullName != expectedType.FullName)
                {
                    issues.Add($"TagDataStore '{q.Name}' has NetType '{q.NetType}' (expected: '{expectedType.FullName}').");
                    //continue;
                }

                if (!IsValidFormat(q.Address, expectedType))
                {
                    issues.Add($"TagDataStore '{q.Name}' has invalid Address format '{q.Address}' for type '{expectedType.FullName}'.");
                    // continue;
                }

                if (!groupedRecords.ContainsKey(q.MachineId))
                    groupedRecords[q.MachineId] = [];

                groupedRecords[q.MachineId][q.Name] = new VariableS7
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

            if (issues.Any())
            {
                var errorMessage = "❌ TagDataStore validation failed:\n" + string.Join("\n", issues);
                logger.LogError(errorMessage);
                //throw new InvalidOperationException(errorMessage);
            }

            return groupedRecords;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving VariableS7 list grouped by MachineId.");
            return [];
        }
    }

    /// <summary>
    /// Retrieves tags grouped by machine asynchronously for the specified machine IDs.
    /// </summary>
    /// <param name="machineId">The machine IDs to filter by.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping machine IDs to their tags.</returns>
    public async Task<Dictionary<int, Dictionary<string, VariableS7>>> GetTagsGroupedByMachineAsync(
        IEnumerable<int> machineId,
        CancellationToken cancellationToken)
    {
        try
        {
            const string sql = @"
			SELECT MachineId, Name, Address, NetType, 0 as ValueInt, 0 as ValueReal, 0 as ValueBool, '' as ValueString
			FROM Variables
			WHERE MachineId IN @machineId AND IsActive = 1";
            // Using Dapper's parameterized query to prevent SQL injection and handle the list of machineId
            var command = new CommandDefinition(
                sql,
                parameters: new { MachineIds = machineId },
                cancellationToken: cancellationToken
            );
            //, Method added to support cancellation token
            // jun 15 2025
            // TO Retrieve only active tags for the specified machines
            // and specify the group by MachineId
            //await using var connection = new SqlConnection(connectionString);
            //var query = await db.QueryAsync<(int MachineId, string Name, string Address, string NetType, int ValueInt, string ValueString)>(sql, new { machineId });
            var query = await db.QueryAsync<(int MachineId, string Name, string Address, string NetType, int ValueInt, string ValueString)>(command);

            var groupedRecords = new Dictionary<int, Dictionary<string, VariableS7>>();
            var issues = new List<string>();

            foreach (var q in query)
            {
                if (!PlcTagNames.AllPerformanceTagTypes.TryGetValue(q.Name, out var expectedType) &&
                    !PlcTagNames.AllReferenceTagTypes.TryGetValue(q.Name, out expectedType))
                    continue;

                var resolvedType = Type.GetType(q.NetType, false) ?? Type.GetType($"System.{q.NetType}");
                if (resolvedType?.FullName != expectedType.FullName)
                {
                    issues.Add($"TagDataStore '{q.Name}' has NetType '{q.NetType}' (expected: '{expectedType.FullName}').");
                    //continue;
                }

                if (!IsValidFormat(q.Address, expectedType))
                {
                    issues.Add($"TagDataStore '{q.Name}' has invalid Address format '{q.Address}' for type '{expectedType.FullName}'.");
                    // continue;
                }

                if (!groupedRecords.ContainsKey(q.MachineId))
                    groupedRecords[q.MachineId] = [];

                groupedRecords[q.MachineId][q.Name] = new VariableS7
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

            if (issues.Any())
            {
                var errorMessage = "❌ TagDataStore validation failed:\n" + string.Join("\n", issues);
                logger.LogError(errorMessage);
                //throw new InvalidOperationException(errorMessage);
            }

            return groupedRecords;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving VariableS7 list grouped by MachineId.");
            return [];
        }
    }

    /// <summary>
    /// Retrieves tags grouped by machine asynchronously for the specified machine IDs and group ID.
    /// </summary>
    /// <param name="machineId">The machine IDs to filter by.</param>
    /// <param name="groupId">The group ID to filter by.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping machine IDs to their tags.</returns>
    public async Task<Dictionary<int, Dictionary<string, VariableS7>>> GetTagsGroupedByMachineAsync(IEnumerable<int> machineId, int groupId,
        CancellationToken cancellationToken)
    {
        try
        {
            const string sql = @"
			SELECT MachineId, Name, Address, NetType, 0 as ValueInt, 0 as ValueReal, 0 as ValueBool, '' as ValueString
			FROM Variables
			WHERE MachineId IN @machineId AND VariableGroupId=@groupId AND IsActive = 1 ";
            // Using Dapper's parameterized query to prevent SQL injection and handle the list of machineId
            var command = new CommandDefinition(
                sql,
                parameters: new { machineId = machineId, groupId = groupId },
                cancellationToken: cancellationToken
            );
            //, Method added to support cancellation token
            // jun 15 2025
            // TO Retrieve only active tags for the specified machines
            // and specify the group by MachineId
            //await using var connection = new SqlConnection(connectionString);
            //var query = await db.QueryAsync<(int MachineId, string Name, string Address, string NetType, int ValueInt, string ValueString)>(sql, new { machineId });
            logger.LogInformation("Executing SQL: {Sql} with parameters: {@Params}", command.CommandText, @command.Parameters);
            var query = await db.QueryAsync<(int MachineId, string Name, string Address, string NetType, int ValueInt, string ValueString)>(command);

            var groupedRecords = new Dictionary<int, Dictionary<string, VariableS7>>();
            var issues = new List<string>();

            foreach (var q in query)
            {
                if (!PlcTagNames.AllPerformanceTagTypes.TryGetValue(q.Name, out var expectedType) &&
                    !PlcTagNames.AllReferenceTagTypes.TryGetValue(q.Name, out expectedType))
                    continue;

                var resolvedType = Type.GetType(q.NetType, false) ?? Type.GetType($"System.{q.NetType}");
                if (resolvedType?.FullName != expectedType.FullName)
                {
                    issues.Add($"TagDataStore '{q.Name}' has NetType '{q.NetType}' (expected: '{expectedType.FullName}').");
                    //continue;
                }

                if (!IsValidFormat(q.Address, expectedType))
                {
                    issues.Add($"TagDataStore '{q.Name}' has invalid Address format '{q.Address}' for type '{expectedType.FullName}'.");
                    // continue;
                }

                if (!groupedRecords.ContainsKey(q.MachineId))
                    groupedRecords[q.MachineId] = [];

                groupedRecords[q.MachineId][q.Name] = new VariableS7
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

            if (issues.Any())
            {
                var errorMessage = "❌ TagDataStore validation failed:\n" + string.Join("\n", issues);
                logger.LogError(errorMessage);
                //throw new InvalidOperationException(errorMessage);
            }

            return groupedRecords;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving VariableS7 list grouped by MachineId.");
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

    private static readonly Regex DbStringFormatRegex = new(@"^DB\d+\.(S|STRING)\d+\.\d+$", RegexOptions.Compiled);
    private static readonly Regex DbInt16FormatRegex = new(@"^DB\d+\.(DBW\d+|W\d+)$", RegexOptions.Compiled);
    private static readonly Regex DbInt32FormatRegex = new(@"^DB\d+\.(DINT\d+|DBD\d+)$", RegexOptions.Compiled);
    private static readonly Regex DbBoolFormatRegex = new(@"^DB\d+\.X\d+\.[0-7]$", RegexOptions.Compiled);
    private static readonly Regex DbInt32OrRealFormatRegex = new(@"^DB\d+\.(DINT\d+|DBD\d+|D\d+|REAL\d+|R\d+)$", RegexOptions.Compiled);

    //FIX [CRITICAL] CORRECT THIS REGEX TO MATCH BOOLS
    //ABR 17 MAY 2025
    // Corrected regex to match Siemens-style boolean DB addresses like DB257.X10.0

    /// <summary>
    /// Retrieves tags for a specific machine asynchronously.
    /// </summary>
    /// <param name="machineId">The machine ID to filter by.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping tag names to their VariableS7 objects.</returns>
    public async Task<Dictionary<string, VariableS7>> GetTagsAsync(int machineId, CancellationToken cancellationToken)
    {
        try
        {
            const string sql = @"
			SELECT DISTINCT Name, Address, NetType, 0 as ValueInt, '' as ValueString
			FROM Variables
			WHERE MachineId = @MachineId AND IsActive = 1";

            var cmd = db.CreateCommand();
            var command = new CommandDefinition(
                sql,
                parameters: new { MachineId = machineId },
                cancellationToken: cancellationToken
            );

            var query = await db.QueryAsync<(string Name, string Address, string NetType, int ValueInt, string ValueString)>(command);

            // Alternative using Dapper directly with parameters (changed)
            // ABR 13 JUN 2025
            // FIX To  be able to propagate cancellation token, using a command directly this was the old implementation code
            //var query = await db.QueryAsync<(string Name, string Address, string NetType, int ValueInt, string ValueString)>(
            //    sql, new { MachineId = machineId });

            var result = query.ToList();
            var records = new Dictionary<string, VariableS7>();
            var issues = new List<string>();

            foreach (var q in result)
            {
                if (!PlcTagNames.AllPerformanceTagTypes.TryGetValue(q.Name, out var expectedType) &&
                    !PlcTagNames.AllReferenceTagTypes.TryGetValue(q.Name, out expectedType))
                    continue;

                var resolvedType = Type.GetType(q.NetType, false) ?? Type.GetType($"System.{q.NetType}");
                if (resolvedType?.FullName != expectedType.FullName)
                {
                    issues.Add($"TagDataStore '{q.Name}' has NetType '{q.NetType}' (expected: '{expectedType.FullName}').");
                    continue;
                }

                if (!IsValidFormat(q.Address, expectedType))
                {
                    issues.Add($"TagDataStore '{q.Name}' has invalid Address format '{q.Address}' for type '{expectedType.FullName}'.");
                    continue;
                }

                records[q.Name] = new VariableS7
                {
                    MachineId = machineId,
                    Name = q.Name,
                    Address = q.Address,
                    NetType = q.NetType,
                    ValueInt = q.ValueInt,
                    ValueString = q.ValueString,
                    Type = resolvedType ?? typeof(object),
                };
            }

            if (issues.Count <= 0) return records;

            var errorMessage = "❌ TagDataStore validation failed:\n" + string.Join("\n", issues);
            logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving VariableS7 list for MachineId {MachineId}.", machineId);
            return [];
        }
    }
}
