using IndTrace.Dependencies.Simulations;

namespace IndTrace.VirtualNetwork.Simulation;

/// <summary>
/// Manages the simulation of PLC data, including time, production, and event counters.
/// </summary>
public class SimulationEngine
{
    private readonly IOptionsMonitor<SimulationSettings> options;
    private DateTime lastSimulatedDate;
    private SimulationSnapshot lastSnapshot = null!;

    private bool firstRun = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationEngine"/> class.
    /// </summary>
    /// <param name="options">The options monitor for simulation settings.</param>
    public SimulationEngine(IOptionsMonitor<SimulationSettings> options)
    {
        this.options = options;
        var settings = this.options.CurrentValue;
        this.lastSimulatedDate = DateTime.Today;
    }

    /// <summary>
    /// Simulates the next step in the PLC data based on current settings and time.
    /// </summary>
    /// <returns>A <see cref="SimulationSnapshot"/> representing the current state of the simulation.</returns>
    public SimulationSnapshot Simulate()
    {
        var settings = this.options.CurrentValue;

        if (this.firstRun)
        {
            var newSnapshot = this.FirstSimulationStep(settings);
            this.lastSnapshot = newSnapshot;
            return newSnapshot;
        }
        else
        {
            var newSnapshot = this.AdvanceSimulation(this.lastSnapshot, settings);
            this.lastSnapshot = newSnapshot;
            return newSnapshot;
        }
    }

    private static int CalculateIncrement(int remaining, double step, double weight, double weightSum, double maxStep)
    {
        if (weightSum == 0) return 0;
        var inc = (int)Math.Min(remaining, Math.Round(maxStep * weight / weightSum));
        return Math.Max(0, inc);
    }

    private static (int, int, int) DistributeIncrements(int totalStep, int remA, int remB, int remC)
    {
        var rand = new Random();
        double wA = rand.NextDouble(), wB = rand.NextDouble(), wC = rand.NextDouble();
        double wSum = wA + wB + wC;
        int incA = Math.Min(remA, (int)Math.Round(totalStep * wA / wSum));
        int incB = Math.Min(remB, (int)Math.Round(totalStep * wB / wSum));
        int incC = Math.Min(remC, (int)Math.Round(totalStep * wC / wSum));
        // Normalize if sum < total, assign remainder to the field with the most remaining
        int assigned = incA + incB + incC;
        int remainder = totalStep - assigned;
        if (remainder > 0)
        {
            if (remA - incA >= remB - incB && remA - incA >= remC - incC) incA += remainder;
            else if (remB - incB >= remC - incC) incB += remainder;
            else incC += remainder;
        }
        return (incA, incB, incC);
    }

    private SimulationSnapshot AdvanceSimulation(SimulationSnapshot lastSnapshot, SimulationSettings settings)
    {
        var now = DateTime.Now;
        var delta = now - this.lastSimulatedDate;
        this.lastSimulatedDate = now;

        // Calculate how many seconds to advance in this step
        var elapsedSeconds = delta.TotalSeconds * settings.SpeedSimulation;
        if (elapsedSeconds <= 0) elapsedSeconds = settings.TimeStep;
        var stepSeconds = Math.Min(elapsedSeconds, settings.TimeStep);

        // Calculate remaining time for each state
        var remainingRun = Math.Max(0, settings.RunningTimeInt - lastSnapshot.RunningTime);
        var remainingStop = Math.Max(0, settings.StoppedTimeInt - lastSnapshot.StoppedTime);
        var remainingFault = Math.Max(0, settings.FaultedTimeInt - lastSnapshot.FaultedTime);
        var totalRemainingTime = Math.Max(1, remainingRun + remainingStop + remainingFault);

        // Distribute stepSeconds among Running, Stopped, Faulted
        var (incRun, incStop, incFault) = DistributeIncrements((int)stepSeconds, remainingRun, remainingStop, remainingFault);

        // Ensure at least one increment if there is quota and stepSeconds > 0
        if (incRun + incStop + incFault == 0 && totalRemainingTime > 0 && stepSeconds > 0)
        {
            if (remainingRun >= remainingStop && remainingRun >= remainingFault) incRun = 1;
            else if (remainingStop >= remainingFault) incStop = 1;
            else incFault = 1;
        }

        // Production increments (proportional to time step)
        var prodStep = stepSeconds / 86400.0; // fraction of a day
        var remainingProdOk = Math.Max(0, (int)(settings.ProductionOk - lastSnapshot.ProductionOk));
        var remainingProdNoK = Math.Max(0, (int)(settings.ProductionNoK - lastSnapshot.ProductionNoK));
        var totalProd = Math.Max(1, remainingProdOk + remainingProdNoK);
        var (incOk, incNoK, _) = DistributeIncrements((int)Math.Round((settings.ProductionOk + settings.ProductionNoK) * prodStep), remainingProdOk, remainingProdNoK, 0);

        // Ensure at least one increment if there is quota and prodStep > 0
        if (incOk + incNoK == 0 && totalProd > 0 && prodStep > 0)
        {
            if (remainingProdOk >= remainingProdNoK) incOk = 1;
            else incNoK = 1;
        }

        // EventCounter increment (proportional to time step)
        var remainingEvent = Math.Max(0, settings.EventCounter - lastSnapshot.EventCounter);
        var incEvent = CalculateIncrement(remainingEvent, prodStep, 1, 1, settings.EventCounter * prodStep);
        if (incEvent < 0) incEvent = 0;

        // Build new snapshot, never allow decrement
        var newSnapshot = new SimulationSnapshot
        {
            TimeStep = settings.TimeStep,
            CurrentTime = lastSnapshot.CurrentTime + (int)stepSeconds,
            RunningTime = Math.Max(lastSnapshot.RunningTime, lastSnapshot.RunningTime + incRun),
            StoppedTime = Math.Max(lastSnapshot.StoppedTime, lastSnapshot.StoppedTime + incStop),
            FaultedTime = Math.Max(lastSnapshot.FaultedTime, lastSnapshot.FaultedTime + incFault),
            ProductionOk = Math.Max(lastSnapshot.ProductionOk, lastSnapshot.ProductionOk + incOk),
            ProductionNoK = Math.Max(lastSnapshot.ProductionNoK, lastSnapshot.ProductionNoK + incNoK),
            EventCounter = Math.Max(lastSnapshot.EventCounter, lastSnapshot.EventCounter + incEvent),
        };
        newSnapshot.SimulatedTime = newSnapshot.RunningTime + newSnapshot.StoppedTime + newSnapshot.FaultedTime;
        newSnapshot.TotalProduction = newSnapshot.ProductionOk + newSnapshot.ProductionNoK;

        return newSnapshot;
    }

    private SimulationSnapshot FirstSimulationStep(SimulationSettings settings)
    {
        this.lastSnapshot = new SimulationSnapshot(settings);
        this.lastSimulatedDate = DateTime.Today;
        double secondsInDay = 86400;
        var progressRatio = settings.CurrentTimeInt / secondsInDay;

        if (progressRatio > 1.0) progressRatio = 1.0; // Clamp to 1.0 if exceeds

        var newSnapshot = StartSimulation(progressRatio, this.lastSnapshot, settings);
        this.firstRun = false;
        this.lastSnapshot = newSnapshot;
        return newSnapshot;
    }

    /// <summary>
    /// Starts the simulation based on a given ratio and target settings.
    /// </summary>
    /// <param name="ratio">The simulation progress ratio (0.0 to 1.0).</param>
    /// <param name="actual">The actual simulation snapshot.</param>
    /// <param name="target">The target simulation settings.</param>
    /// <returns>A new <see cref="SimulationSnapshot"/> representing the initial state of the simulation.</returns>
    public static SimulationSnapshot StartSimulation(double ratio, SimulationSnapshot actual, SimulationSettings target)
    {
        double totalConfiguredTime = target.RunningTimeInt + target.StoppedTimeInt + target.FaultedTimeInt;
        var newSimulation = new SimulationSnapshot
        {
            TimeStep = target.TimeStep,
            CurrentTime = target.CurrentTimeInt,

            RunningTime = SimulateValue(actual.RunningTime, totalConfiguredTime, ratio),
            StoppedTime = SimulateValue(actual.StoppedTime, totalConfiguredTime, ratio),
            FaultedTime = SimulateValue(actual.FaultedTime, totalConfiguredTime, ratio),

            ProductionOk = SimulateValue(actual.ProductionOk, target.ProductionOk, ratio),
            ProductionNoK = SimulateValue(actual.ProductionNoK, target.ProductionNoK, ratio),

            EventCounter = SimulateValue(actual.EventCounter, (double)target.EventCounter, ratio),
        };
        newSimulation.SimulatedTime = newSimulation.RunningTime + newSimulation.StoppedTime + newSimulation.FaultedTime;
        newSimulation.TotalProduction = newSimulation.ProductionOk + newSimulation.ProductionNoK;

        return newSimulation;
    }

    private static float SimulateValue(float current, float target, double ratio)
    {
        var expected = (float)(target * ratio);
        if (expected < current) return current;

        var rnd = new Random();
        var newValue = current + (float)(rnd.NextDouble() * (expected - current));
        return newValue < current ? current : newValue;
    }

    private static int SimulateValue(int currentFloat, double target, double ratio)
    {
        var current = (float)currentFloat;

        var expected = (float)(target * ratio);
        if (expected < current) return (int)current;

        var rnd = new Random();
        var newValue = current + (float)(rnd.NextDouble() * (expected - current));
        return newValue < current ? (int)current : (int)newValue;
    }
}
