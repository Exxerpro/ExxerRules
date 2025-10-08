using System.Reactive.Subjects;
using IndTrace.Application.Performance.Request.Command.Create;
using IndTrace.DataStore.Services.OEE.Interfaces;
using IndTrace.Domain.Entities;

namespace IndTrace.DataStore.Services.OEE.Services;

/// <summary>
/// Service that processes OEE register data reactively and notifies observers with OEE DTOs.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate ReactiveEventService logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the ReactiveEventService.
/// </summary>
public class ReactiveEventService(IChannelBroker<PerformanceData> broker, IKpiDataSink kpiDataSink) : IReactiveEventService, IObservableService
{
    private readonly Subject<OeeRegisterDto> subject = new();

    /// <summary>
    /// Gets an observable stream of OEE register DTOs for subscribers.
    /// </summary>
    public IObservable<OeeRegisterDto> Stream => this.subject.AsObservable();

    /// <summary>
    /// Processes OEE register data from the channel broker, calculates OEE, writes results, and notifies observers.
    /// </summary>
    /// <param name="stoppingToken">A token to observe for cancellation.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    public async Task ProcessOeeRegisterAsync(CancellationToken stoppingToken)
    {
        await foreach (var result in broker.ReadAllAsync(stoppingToken))
        {
            //[TODO]
            //WHY IS THIS CALL HERE? WE ARE NOT USING FLUENT VALIDATION ON THIS CLASS
            //FIX
            //ABR
            //16 JUN 2025
            //result.Validate();

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
