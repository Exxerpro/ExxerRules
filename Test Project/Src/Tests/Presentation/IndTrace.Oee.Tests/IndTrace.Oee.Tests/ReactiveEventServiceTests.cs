using IndTrace.Application.Performance.Request.Command.Create;
using IndTrace.DataStore.Services.OEE.Interfaces;
using IndTrace.Domain.Entities;
using IndTrace.OEE.Infrastructure.Channels;
using IndTrace.OEE.Infrastructure.Repository;
using IndTrace.OEE.Infrastructure.Services;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Logging;
using NSubstitute;
using QuestDB;
using QuestDB.Senders;
using Shouldly;
using System;

using System;

using System.Collections.Generic;

using System.Collections.Generic;

using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

using System.Linq;

namespace IndTrace.Oee.Tests;
/// <summary>
/// Represents the ReactiveEventServiceTests.
/// </summary>

public class ReactiveEventServiceTests
{
    private readonly ILogger logger;
    private readonly KpiDataSink kpiDataPort;
    private readonly ISender sender;

    /// <summary>
    /// The port to check for availability.
    /// </summary>
    public static int portToCheck = 9000;

    /// <summary>
    /// Determines whether to skip all tests in this class based on port availability.
    /// </summary>
    public static bool ShouldSkipThisClass => PortChecker.IsPortClosed(9000); // Or based on environment variables, configuration, etc.

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="output">The output.</param>

    public ReactiveEventServiceTests(ITestOutputHelper output)
    {
        var baseAddres = new Uri("http://localhost:9000");

        if (PortChecker.IsPortClosed(portToCheck))
        {
            baseAddres = new Uri("http://localhost:9000");
        }

        logger = XUnitLogger.CreateLogger<RepoPlcService>(output);

        var ILogger = XUnitLogger.CreateLogger<KpiDataSink>(output);
        var httpHandler = new FakeHttpMessageHandler();
        var httpClient = new HttpClient(httpHandler)
        {
            BaseAddress = baseAddres
        };

        if (PortChecker.IsPortClosed(portToCheck))
        {
            sender = Sender.New();
        }
        else
        {
            sender = Sender.New("http::addr=localhost:9000;");
        }

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient("KpiDataPort").Returns(httpClient);

        kpiDataPort = new KpiDataSink(ILogger, httpClient, sender);
    }

    /// <summary>
    /// Executes ProcessOeeRegisterAsync_ShouldEmitDto_AndCallKpiDataPort operation.
    /// </summary>
    /// <returns>The result of ProcessOeeRegisterAsync_ShouldEmitDto_AndCallKpiDataPort.</returns>

    [Fact(Skip = "Conditional skip: localhost:9000 must be open.", SkipWhen = nameof(ShouldSkipThisClass))]
    public async Task ProcessOeeRegisterAsync_ShouldEmitDto_AndCallKpiDataPort()
    {
        var portToCheck = 9000;
        if (PortChecker.IsPortClosed(portToCheck))
        {
            // Skip the test if the port is not open
            return;
        }

        // Arrange
        var broker = new ChannelBroker<PerformanceData>();
        var service = new ReactiveEventService(broker, kpiDataPort);

        var testData = new PerformanceData
        {
            TotalProduction = 100,
            ProductionOk = 95,
            ProductionNoK = 5,
            RunningTime = 1100,
            StoppedTime = 900,
            FaultedTime = 400,
            CurrentTime = 2400,
            MachineId = 10000,
            PlcId = 100,
            TimeStamp = DateTime.UtcNow,
            ActualCycleTime = 11,
            StandardCycleTime = 10,
            PlanedProductionTime = 2400,
            BarCodeId = 100
        };
        var receivedDtos = new List<OeeRegisterDto>();
        using var subscription = service.Stream.Subscribe(dto => receivedDtos.Add(dto));

        var cts = new CancellationTokenSource();

        // Act: Simulate message and trigger processing
        var processingTask = service.ProcessOeeRegisterAsync(cts.Token);

        await broker.WriteAsync(testData, cts.Token);
        await processingTask;

        receivedDtos.ShouldHaveSingleItem();
        receivedDtos[0].MachineId.ShouldBe(testData.MachineId);
    }

    /// <summary>
    /// Executes ProcessOeeRegisterAsync_ShouldNotifyObservers_WhenValidOeeIsComputed operation.
    /// </summary>
    /// <returns>The result of ProcessOeeRegisterAsync_ShouldNotifyObservers_WhenValidOeeIsComputed.</returns>
    [Fact(Skip = "Conditional skip: localhost:9000 must be open.", SkipWhen = nameof(ShouldSkipThisClass))]
    public async Task ProcessOeeRegisterAsync_ShouldNotifyObservers_WhenValidOeeIsComputed()
    {
        var portToCheck = 9000;
        if (PortChecker.IsPortClosed(portToCheck))
        {
            // Skip the test if the port is not open
            return;
        }
        // Arrange
        var broker = new ChannelBroker<PerformanceData>();

        var service = new ReactiveEventService(broker, kpiDataPort);

        var testData = new PerformanceData
        {
            TotalProduction = 100,
            ProductionOk = 95,
            ProductionNoK = 5,
            RunningTime = 1100,
            StoppedTime = 900,
            FaultedTime = 400,
            CurrentTime = 2400,
            MachineId = 10000,
            PlcId = 100,
            TimeStamp = DateTime.UtcNow,
            ActualCycleTime = 11,
            StandardCycleTime = 10,
            PlanedProductionTime = 2400,
            BarCodeId = 100
        };

        var receivedDtos = new List<OeeRegisterDto>();
        var completed = false;

        // Subscribe to the observable
        var subscription = service.Stream.Subscribe(
            dto => receivedDtos.Add(dto),
            () => completed = true
        );

        // Act
        var cts = new CancellationTokenSource();
        var processingTask = service.ProcessOeeRegisterAsync(cts.Token);

        await broker.WriteAsync(testData, cts.Token);

        await processingTask;

        // Assert
        receivedDtos.Count.ShouldBe(1);
        receivedDtos[0].MachineId.ShouldBeEquivalentTo(100);
        completed.ShouldBeTrue();

        subscription.Dispose();
    }

    /// <summary>
    /// Executes ProcessOeeRegisterAsync_ShouldNotNotifyObserver_WhenOeeCalculationFails operation.
    /// </summary>
    /// <returns>The result of ProcessOeeRegisterAsync_ShouldNotNotifyObserver_WhenOeeCalculationFails.</returns>

    [Fact(Skip = "Conditional skip: localhost:9000 must be open.", SkipWhen = nameof(ShouldSkipThisClass))]
    public async Task ProcessOeeRegisterAsync_ShouldNotNotifyObserver_WhenOeeCalculationFails()
    {
        //Skip this test if port 900 is not open
        var portToCheck = 9000;
        if (PortChecker.IsPortClosed(portToCheck))
        {
            return;
        }
        // Arrange
        var broker = new ChannelBroker<PerformanceData>();
        var service = new ReactiveEventService(broker, kpiDataPort);

        var testData = new PerformanceData(); // Default/null values — should trigger failure in OEE

        var receivedDtos = new List<OeeRegisterDto>();
        var completed = false;

        using var subscription = service.Stream.Subscribe(
            dto => receivedDtos.Add(dto),
            () => completed = true
        );

        var cts = new CancellationTokenSource();
        var processingTask = service.ProcessOeeRegisterAsync(cts.Token);

        await broker.WriteAsync(testData, cts.Token);
        // Don't send any data (simulate invalid inputs filtered before write)
        broker.Complete();  // ⬅️ Very important to allow ReadAllAsync to complete
        await processingTask;

        // Assert: Nothing emitted
        receivedDtos.Count.ShouldBe(0);
        completed.ShouldBeFalse();
    }

    /// <summary>
    /// Executes ProcessOeeRegisterAsync_ShouldNotEmit_WhenInvalidDataProvided operation.
    /// </summary>
    /// <returns>The result of ProcessOeeRegisterAsync_ShouldNotEmit_WhenInvalidDataProvided.</returns>

    [Fact(Skip = "Conditional skip: localhost:9000 must be open.", SkipWhen = nameof(ShouldSkipThisClass))]
    public async Task ProcessOeeRegisterAsync_ShouldNotEmit_WhenInvalidDataProvided()
    {
        var portToCheck = 9000;
        if (PortChecker.IsPortClosed(portToCheck))
        {
            // Skip the test if the port is not open
            return;
        }
        // Arrange
        var broker = new ChannelBroker<PerformanceData>();
        var service = new ReactiveEventService(broker, kpiDataPort);

        var receivedDtos = new List<OeeRegisterDto>();
        using var subscription = service.Stream.Subscribe(dto => receivedDtos.Add(dto));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2)); // prevent hanging

        var processingTask = service.ProcessOeeRegisterAsync(cts.Token);

        // Don't send any data (simulate invalid inputs filtered before write)
        broker.Complete();  // ⬅️ Very important to allow ReadAllAsync to complete

        await processingTask;

        // Assert
        receivedDtos.ShouldBeEmpty();
    }
}
