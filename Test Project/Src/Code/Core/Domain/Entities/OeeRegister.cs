// <copyright file="OeeRegister.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents an OEE (Overall Equipment Effectiveness) register, including production metrics, PLC signals, and KPI calculations.
/// </summary>
public class OeeRegister : IEntityRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OeeRegister"/> class.
    /// </summary>
    public OeeRegister()
    {
        this.KpiOee = null!;
    }

    /// <summary>
    /// Returns a string representation of the OEE register.
    /// </summary>
    /// <returns>A string containing the OEE register ID, machine ID, OEE percentage, and timestamp.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"OEE {this.OeeRegisterId} (Machine {this.MachineId}): {this.Oee:P1} at {this.TimeStamp:yyyy-MM-dd HH:mm}";

    /// <summary>
    /// Gets or sets the unique identifier for the OEE register.
    /// </summary>
    public int OeeRegisterId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the OEE register.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC identifier associated with the OEE register.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the OEE register.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the application flag from the PLC.
    /// </summary>
    public int ApplicationFlag { get; set; }

    /// <summary>
    /// Gets or sets the event counter from the PLC.
    /// </summary>
    public int EventCounter { get; set; }

    /// <summary>
    /// Gets or sets the current time from the PLC.
    /// </summary>
    public int CurrentTime { get; set; }

    /// <summary>
    /// Gets or sets the running time from the PLC.
    /// </summary>
    public int RunningTime { get; set; }

    /// <summary>
    /// Gets or sets the stopped time from the PLC.
    /// </summary>
    public int StoppedTime { get; set; }

    /// <summary>
    /// Gets or sets the faulted time from the PLC.
    /// </summary>
    public int FaultedTime { get; set; }

    /// <summary>
    /// Gets or sets the status fault reason from the PLC.
    /// </summary>
    public int StatusFaultReason { get; set; }

    /// <summary>
    /// Gets or sets the product identifier associated with the OEE register.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the total production value.
    /// </summary>
    public double TotalProduction { get; set; }

    /// <summary>
    /// Gets or sets the standard cycle time.
    /// </summary>
    public double StandardCycleTime { get; set; }

    /// <summary>
    /// Gets or sets the actual cycle time.
    /// </summary>
    public double ActualCycleTime { get; set; }

    /// <summary>
    /// Gets or sets the planned production time.
    /// </summary>
    public double PlanedProductionTime { get; set; }

    /// <summary>
    /// Gets or sets the reject event counter.
    /// </summary>
    public int RejectEventCounter { get; set; }

    /// <summary>
    /// Gets or sets the status reject value.
    /// </summary>
    public int StatusReject { get; set; }

    /// <summary>
    /// Gets or sets the quantity of rejected units.
    /// </summary>
    public double RejectQuantityUnits { get; set; }

    /// <summary>
    /// Gets or sets the quantity of OK production units.
    /// </summary>
    public double ProductionOk { get; set; }

    /// <summary>
    /// Gets or sets the quantity of NOK production units.
    /// </summary>
    public double ProductionNoK { get; set; }

    /// <summary>
    /// Gets or sets the associated KPI OEE entity.
    /// </summary>
    public KpiOee KpiOee { get; set; }

    /// <summary>
    /// Gets or sets the OEE value (not mapped to the database).
    /// </summary>
    public double Oee { get; set; }

    /// <summary>
    /// Gets or sets the availability value (not mapped to the database).
    /// </summary>
    public double Availability { get; set; }

    /// <summary>
    /// Gets or sets the performance value (not mapped to the database).
    /// </summary>
    public double Performance { get; set; }

    /// <summary>
    /// Gets or sets the quality value (not mapped to the database).
    /// </summary>
    public double Quality { get; set; }

    /// <summary>
    /// Creates a KpiOee object from the current OEE register with sanitized metrics.
    /// </summary>
    /// <param name="register">The OEE register to convert.</param>
    /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
    /// <returns>A new KpiOee object with sanitized metrics.</returns>
    public static KpiOee ToKpiOee(OeeRegister register, IDateTimeMachine? dateTimeMachine = null)
    {
        ArgumentNullException.ThrowIfNull(register);

        var dtm = dateTimeMachine ?? new DateTimeMachine();

        // Sanitize values before Math.Round to prevent NaN/Infinity propagation
        var quality = ClampMetric(register.Quality, 0.0, 1.0);
        var availability = ClampMetric(register.Availability, 0.0, 1.0);
        var performance = ClampMetric(register.Performance, 0.0, 1.5);
        var oee = ClampMetric(register.Oee, 0.0, 1.0);

        return new KpiOee
        {
            TimeStamp = dtm.Now,
            Quality = Math.Round(quality, 6),
            Availability = Math.Round(availability, 6),
            Performance = Math.Round(performance, 6),
            Oee = Math.Round(oee, 6),
        };
    }

    /// <summary>
    /// Calculates OEE metrics and returns a result containing the updated register and any warnings or errors.
    /// </summary>
    /// <param name="register">The OEE register to update.</param>
    /// <param name="data">The performance data to use for calculation.</param>
    /// <returns>A <see cref="Result{OeeRegister}"/> containing the updated register and any warnings or errors.</returns>
    public static Result<OeeRegister> CalculateOee(OeeRegister register, PerformanceData data)
    {
        var warnings = new List<string>();
        var errors = new List<string>();

        var result = Result<OeeRegister>.Success(new OeeRegister());

        if (register == null || data == null)
        {
            return Result<OeeRegister>.WithFailure("Inputs cannot be null");
        }

        // Normalize invalid production values
        if (data.TotalProduction < 0)
        {
            warnings.Add("TotalProduction was negative and was clamped to zero.");
            data.TotalProduction = Math.Max(0, data.TotalProduction);
        }

        if (data.ProductionOk < 0 || data.ProductionOk > data.TotalProduction)
        {
            warnings.Add("ProductionOk was outside valid range and was clamped.");
            data.ProductionOk = Math.Max(0, Math.Min(data.ProductionOk, data.TotalProduction));
        }

        if (data.ProductionNoK < 0 || data.ProductionNoK > data.TotalProduction - data.ProductionOk)
        {
            warnings.Add("ProductionNoK was outside valid range and was clamped.");
            data.ProductionNoK = Math.Max(0, Math.Min(data.ProductionNoK, data.TotalProduction - data.ProductionOk));
        }

        if (data.CurrentTime < 0)
        {
            warnings.Add("CurrentTime was negative and clamped to zero.");
            data.CurrentTime = Math.Max(0, data.CurrentTime);
        }

        if (data.RunningTime < 0)
        {
            warnings.Add("RunningTime was negative and clamped to zero.");
            data.RunningTime = Math.Max(0, data.RunningTime);
        }

        if (data.StoppedTime < 0)
        {
            warnings.Add("StoppedTime was negative and clamped to zero.");
            data.StoppedTime = Math.Max(0, data.StoppedTime);
        }

        if (data.FaultedTime < 0)
        {
            warnings.Add("FaultedTime was negative and clamped to zero.");
            data.FaultedTime = Math.Max(0, data.FaultedTime);
        }

        if (register.PlanedProductionTime < 0)
        {
            warnings.Add("PlannedProductionTime was negative and clamped to zero.");
            register.PlanedProductionTime = Math.Max(0, register.PlanedProductionTime);
        }

        if (register.ActualCycleTime < 0)
        {
            warnings.Add("ActualCycleTime was negative and clamped to zero.");
            register.ActualCycleTime = Math.Max(0, register.ActualCycleTime);
        }

        if (register.StandardCycleTime < 0)
        {
            warnings.Add("StandardCycleTime was negative and clamped to zero.");
            register.StandardCycleTime = Math.Max(0, register.StandardCycleTime);
        }

        // Note: Individual clamping already handled above with warnings

        // Clamp PlanedProductionTime to CurrentTime
        if (register.PlanedProductionTime > data.CurrentTime)
        {
            warnings.Add($"PlannedProductionTime ({register.PlanedProductionTime}) was greater than CurrentTime ({data.CurrentTime}) and was clamped.");
            register.PlanedProductionTime = data.CurrentTime;
        }

        // Note: PlanedProductionTime negative check already handled above at lines 47-50

        // Scale down FaultedTime + StoppedTime if they exceed CurrentTime
        // Use long arithmetic to prevent integer overflow in the comparison
        if (((long)data.FaultedTime + data.StoppedTime) > data.CurrentTime && data.CurrentTime > 0)
        {
            double totalTime = (double)data.FaultedTime + data.StoppedTime;
            if (totalTime > 0)
            {
                double scale = data.CurrentTime / totalTime;

                // Prevent overflow by using double arithmetic and clamping
                double scaledFaulted = data.FaultedTime * scale;
                double scaledStopped = data.StoppedTime * scale;

                data.FaultedTime = (int)Math.Min(Math.Round(scaledFaulted), data.CurrentTime);
                data.StoppedTime = (int)Math.Min(Math.Round(scaledStopped), data.CurrentTime);
                warnings.Add("FaultedTime + StoppedTime exceeded CurrentTime and were proportionally scaled down.");
            }
            else
            {
                // Handle edge case where both are zero but somehow triggered the condition
                data.FaultedTime = 0;
                data.StoppedTime = 0;
            }
        }

        // Transfer shared fields (initial transfer - may be updated by fallback logic below)
        register.TotalProduction = data.TotalProduction;
        register.CurrentTime = data.CurrentTime;
        register.RunningTime = data.RunningTime;
        register.StoppedTime = data.StoppedTime;
        register.FaultedTime = data.FaultedTime;
        register.ApplicationFlag = data.ApplicationFlag;
        register.EventCounter = data.EventCounter;
        register.StatusFaultReason = data.StatusFaultReason;

        // Fallback PlanedProductionTime (only if it was originally zero, not clamped)
        // Note: We need to track if it was originally zero vs clamped from negative
        if (register.PlanedProductionTime <= 0 && !warnings.Any(w => w.Contains("PlannedProductionTime was negative")))
        {
            register.PlanedProductionTime = data.RunningTime + data.StoppedTime + data.FaultedTime;
            warnings.Add("PlanedProductionTime was zero and computed from time aggregates.");
        }

        // Fallback StandardCycleTime
        if (register.StandardCycleTime <= 0)
        {
            register.StandardCycleTime = register.ActualCycleTime > 0 ? register.ActualCycleTime : 1.0;
            warnings.Add("StandardCycleTime was zero and computed from ActualCycleTime or set to 1.0.");
        }

        // Fallback RunningTime
        if (data.RunningTime <= 0)
        {
            data.RunningTime = data.CurrentTime - data.StoppedTime;
            warnings.Add("RunningTime was zero and computed from CurrentTime - StoppedTime.");

            // Check if computed value is negative and clamp
            if (data.RunningTime < 0)
            {
                data.RunningTime = 0;
                warnings.Add("Computed RunningTime was negative and clamped to zero.");
            }

            // CRITICAL FIX: Synchronize register with updated data
            register.RunningTime = data.RunningTime;
        }

        // Fallback TotalProduction
        if (data.TotalProduction <= 0)
        {
            data.TotalProduction = data.ProductionOk + data.ProductionNoK;
            warnings.Add("TotalProduction was zero and computed from OK + NOK counts.");
        }

        // Quality
        register.Quality = SafeRatio(data.ProductionOk, data.TotalProduction, 0.0, 1.0, 1.0);

        // Availability
        register.Availability = SafeRatio(data.RunningTime, register.PlanedProductionTime, 0.0, 1.0, 0.0);

        // Performance
        double perfNumerator = register.StandardCycleTime * data.TotalProduction;
        register.Performance = SafeRatio(perfNumerator, data.RunningTime, 0.0, 1.5, 1.0);

        // OEE
        double rawOee = register.Availability * register.Performance * register.Quality;
        register.Oee = ClampMetric(rawOee, 0.0, 1.0);

        // MVP estimation: treat each warning as a proxy for missing/estimated data.
        // Compute a simple missing data ratio and a derived confidence score.
        var missingDataRatio = warnings.Count switch
        {
            0 => 0.0,
            <= 2 => 0.1,
            <= 5 => 0.25,
            <= 10 => 0.5,
            _ => 0.75,
        };
        var confidence = Math.Max(0.1, 1.0 - missingDataRatio); // guarantee a floor of 0.1

        result = (warnings.Count, errors.Count) switch
        {
            ( > 0, > 0) => Result<OeeRegister>.CombineErrors(errors, warnings, register),
            ( > 0, 0) => Result<OeeRegister>.WithWarnings(warnings, register, confidence, missingDataRatio),
            (0, > 0) => Result<OeeRegister>.WithFailure(errors, register),
            _ => Result<OeeRegister>.Success(register),
        };

        return result;
    }

    /// <summary>
    /// Clamps a metric value between the specified minimum and maximum.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The clamped value.</returns>
    public static double ClampMetric(double value, double min, double max)
    {
        // Handle special values first
        if (double.IsNaN(value))
        {
            return min; // Default to minimum for NaN
        }

        if (double.IsPositiveInfinity(value))
        {
            return max;
        }

        if (double.IsNegativeInfinity(value))
        {
            return min;
        }

        // Normal clamping logic
        return value switch
        {
            var v when v < min => min,
            var v when v > max => max,
            _ => value,
        };
    }

    /// <summary>
    /// Safely calculates a ratio, clamping the result and providing a fallback if the denominator is zero or negative.
    /// </summary>
    /// <param name="numerator">The numerator value.</param>
    /// <param name="denominator">The denominator value.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="fallback">The fallback value if the denominator is zero or negative.</param>
    /// <returns>The calculated and clamped ratio, or the fallback value.</returns>
    public static double SafeRatio(double numerator, double denominator, double min, double max, double fallback)
    {
        // Handle NaN or Infinity inputs immediately
        if (double.IsNaN(numerator) || double.IsNaN(denominator) ||
            double.IsInfinity(numerator) || double.IsInfinity(denominator))
        {
            return fallback;
        }

        if (denominator <= 0)
        {
            return fallback;
        }

        var value = numerator / denominator;

        // Check for NaN or Infinity results from division
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return fallback;
        }

        return ClampMetric(value, min, max);
    }
}
