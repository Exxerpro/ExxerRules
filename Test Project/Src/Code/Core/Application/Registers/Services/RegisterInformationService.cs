// <copyright file="RegisterInformationService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Services;

using System.Globalization;
using IndTrace.Application.Registers.Queries.GetRegisterList;

/// <summary>
/// Represents the RegisterInformationService.
/// </summary>
public class RegisterInformationService(
    IDateTimeMachine dateTimeMachine,
    IDistinctRegisterService distinctRegisterService,
    IReadOnlyRepository<Register> registerRepository,
    ILogger<RegisterInformationService> logger) : IRegisterInformationService
{
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RegistersRecords>>> GetListOfAvailableRegisters()
    {
        var availableRecords = await distinctRegisterService.GetDistinctRegistersAsync(CancellationToken.None);

        var availableRegisters = availableRecords.Select(x => new RegistersRecords
        {
            MachineId = x.MachineId,
            Name = x.MachineId.ToString("D3") + ":" + x.Name,
        });

        if (availableRegisters is null)
        {
            // Return failure with the error message
            return Result<IEnumerable<RegistersRecords>>.WithFailure("No available registers found.");
        }

        // Return success with the available registers data
        logger.LogInformation("Available registers found: {Count}", availableRegisters.Count());
        logger.LogInformation("Register Found at {Time}", dateTimeMachine.Now);
        return Result<IEnumerable<RegistersRecords>>.Success(availableRegisters);
    }

    /// <inheritdoc/>
    public async Task<Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>> GetListRegisterTrends(
    IEnumerable<RegistersRecords> variables, int maxItems = 100)
    {
        // [Fix]
        // CLAUDE
        // Date: 25/08/2025
        // Reason: [FINAL SESSION HONOR] - Defensive validation after major architectural refactor from dispatcher anti-pattern to proper repository pattern
        if (variables is null)
        {
            return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.WithFailure("Variables parameter cannot be null");
        }

        // TODO DELETE ON RELEASE
        // ABR
        // 24 AUGUST 2024
        if (Debugger.IsAttached)
        {
            // Generate and return mock data if debugging
            var mockData = this.GenerateMockData(maxItems);
            return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.Success(mockData);
        }

        var varsRegisters = ParseVariables(variables);

        if (varsRegisters.IsFailure)
        {
            return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.WithFailure(varsRegisters.Errors);
        }

        if (varsRegisters.Value is null)
        {
            return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.WithFailure("variables not found");
        }

        var query = varsRegisters.Value;

        var endDate = DateTime.Now.ToLocalTime();
        var startDate = endDate.AddDays(-1);

        var totalItems = new Dictionary<(int MachineId, string Name), List<TimeSeriesDataPoint>>();
        int fetchedItemsCount = 0;

        while (fetchedItemsCount < maxItems)
        {
            query.StartDate = startDate;
            query.EndDate = endDate;

            logger.LogInformation("Querying registers from {StartDate} to {EndDate}", startDate, endDate);

            // [Fix]
            // CLAUDE
            // Date: 25/08/2025
            // Reason: [ARCHITECTURAL REFACTOR] - Replace monitorRequestDispatcher with proper repository pattern
            var registerSpec = new Specification<Register>(r =>
                query.MachineId.Contains(r.MachineId) &&
                query.RegistersName.Contains(r.Name) &&
                query.VariablesId.Contains(r.VariableId) &&
                r.TimeStamp >= startDate &&
                r.TimeStamp <= endDate);

            var registerResult = await registerRepository.ListAsync(registerSpec, CancellationToken.None);

            if (registerResult.IsFailure)
            {
                logger.LogError("Failed to query registers: {Errors}", string.Join(", ", registerResult.Errors));
                return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.WithFailure(registerResult.Errors);
            }

            if (registerResult.Value is null)
            {
                logger.LogError("Register query returned null");
                return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.WithFailure("Register query returned null");
            }

            // Convert Register entities to RegisterDto format for compatibility
            var registerDtos = registerResult.Value.Select(r => new RegisterDto
            {
                MachineId = r.MachineId,
                Name = r.Name,
                Value = r.Value,
                DataType = r.DataType,
                TimeStamp = r.TimeStamp,
            });

            var result = Result<IEnumerable<RegisterDto>>.Success(registerDtos);

            // I am having a null result here, that is not supposy to happen
            // this is in one test
            // we have to avoid this situation being more defensively
            if (result.IsFailure)
            {
                return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.WithFailure(result.Errors);
            }

            if (result.Value is null)
            {
                return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.WithFailure("variables not found");
            }

            var timeSeriesDataPoints = MapToTimeSeries(result.Value);

            // Merge the fetched data into totalItems
            foreach (var key in timeSeriesDataPoints.Keys)
            {
                if (!totalItems.ContainsKey(key))
                {
                    totalItems[key] = [];
                }

                totalItems[key].AddRange(timeSeriesDataPoints[key]);
            }

            fetchedItemsCount = totalItems.Values.Sum(list => list.Count);

            // If we have enough data, trim excess and exit loop
            if (fetchedItemsCount >= maxItems)
            {
                foreach (var key in totalItems.Keys)
                {
                    totalItems[key] = totalItems[key]
                        .OrderByDescending(item => item.TimeStamp)
                        .Take(maxItems)
                        .ToList();
                }

                break;
            }

            // Update the date range to fetch older data
            endDate = startDate;
            startDate = endDate.AddDays(-1);
        }

        // Convert lists back to IEnumerable and return success
        var finalResult = totalItems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AsEnumerable());
        return Result<Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>>>.Success(finalResult);
    }

    private Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>> GenerateMockData(int maxItems)
    {
        // [Fix]
        // CLAUDE
        // Date: 25/08/2025
        // Reason: [NULL REFERENCE FIX] - Ensure all required properties are set on TimeSeriesDataPoint to prevent null exceptions
        var mockData = Enumerable.Range(1, 5).ToDictionary(
            i => (MachineId: i, Name: $"Machine-{i}"),
            i => Enumerable.Range(0, maxItems).Select(j => new TimeSeriesDataPoint
            {
                MachineId = i,
                Name = $"Machine-{i}",
                TimeStamp = DateTime.Now.AddMinutes(-j),
                Value = new Random().NextDouble().ToString("F2", CultureInfo.InvariantCulture),
                ValueType = "double",
            }).AsEnumerable());

        return mockData;
    }

    public static Result<GetRegistersListQuery> ParseVariables(IEnumerable<RegistersRecords> variables)
    {
        var registersNameSet = new HashSet<string>();
        var machineIdsSet = new HashSet<int>();
        var variablesIdSet = new HashSet<int>();

        machineIdsSet = variables.Select(x => x.MachineId).Distinct().ToHashSet();
        registersNameSet = variables.Select(x => x.Name).ToHashSet();
        variablesIdSet = variables.Select(x => x.VariableId).ToHashSet();

        return Result<GetRegistersListQuery>.Success(new GetRegistersListQuery
        {
            MachineId = machineIdsSet.ToList(),
            RegistersName = registersNameSet.ToList(),
            VariablesId = variablesIdSet.ToList(),
        });
    }

    public static Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>> MapToTimeSeries(IEnumerable<RegisterDto> registerDtos)
    {
        var result = registerDtos
            .GroupBy(dto => (dto.MachineId, dto.Name)) // Group by MachineId and Name
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(dto => new TimeSeriesDataPoint
                    {
                        MachineId = dto.MachineId,
                        Name = dto.Name,
                        Value = dto.Value,
                        ValueType = dto.DataType,
                        TimeStamp = dto.TimeStamp,
                    })
                    .OrderBy(dataPoint => dataPoint.TimeStamp) // Order by TimeStamp
                    .AsEnumerable());

        return result;
    }
}
