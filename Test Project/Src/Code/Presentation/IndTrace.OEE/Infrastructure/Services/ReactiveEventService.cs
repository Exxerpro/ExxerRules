// <copyright file="ReactiveEventService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Performance.Request.Command.Create;

namespace IndTrace.OEE.Infrastructure.Services;

using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;
using IndTrace.Domain.Entities;
using IndTrace.OEE.Infrastructure.Channels;
using IndTrace.OEE.Infrastructure.Repository;

/// <summary>
/// Service that processes OEE register data reactively and notifies observers with OEE DTOs.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate ReactiveEventService logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the ReactiveEventService.
/// </summary>
public class ReactiveEventService(IChannelBroker<PerformanceData> broker, KpiDataSink kpiDataSink) : IReactiveEventService, IObservableService
{
    private readonly Subject<OeeRegisterDto> subject = new();

    /// <summary>
    /// Gets the observable stream of OEE register data.
    /// </summary>
    public IObservable<OeeRegisterDto> Stream => this.subject.AsObservable();

    /// <summary>
    /// Processes OEE register data reactively from the channel and notifies observers.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token to stop the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ProcessOeeRegisterAsync(CancellationToken stoppingToken)
    {
        await foreach (var result in broker.ReadAllAsync(stoppingToken))
        {
            // wHY IS THIS CALL HERE? WE ARE NOT USING FLUENT VALIDATION ON THIS CLASS
            // FIX
            // ABR
            // 16 JUN 2025
            // result.Validate();
            var oeeRegister = new OeeRegister
            {
                MachineId = result.MachineId,
                PlcId = result.PlcId,
                TimeStamp = result.TimeStamp,
                ActualCycleTime = result.ActualCycleTime,
                StandardCycleTime = result.StandardCycleTime,
                PlanedProductionTime = result.PlanedProductionTime,
                ProductId = result.BarCodeId,
            };

            var resultOee = OeeRegister.CalculateOee(oeeRegister, result);

            await kpiDataSink.WriteAsync(result, stoppingToken);

            if ((resultOee.IsSuccess || resultOee.HasWarnings) && resultOee.Value is not null)
            {
                var dto = OeeRegisterDto.ToDto(resultOee.Value);

                var kpi = OeeRegister.ToKpiOee(resultOee.Value);

                await kpiDataSink.WriteAsync(kpi, stoppingToken); // Write to QuestDB
                if (dto.Value is not null)
                {
                    this.subject.OnNext(dto.Value); // notify observers
                    this.subject.OnCompleted();
                }
            }
        }
    }
}
