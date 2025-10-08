// <copyright file="StatisticalExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.Performance;

/// <summary>
/// Provides extension methods for statistical calculations on double arrays.
/// </summary>
public static class StatisticalExtensions
{
    /// <summary>
    /// Computes the arithmetic mean of the sample values.
    /// </summary>
    /// <param name="samples">The array of sample values.</param>
    /// <returns>The arithmetic mean of the samples.</returns>
    public static double ComputeMean(this double[] samples)
    {
        return samples.Average();
    }

    /// <summary>
    /// Computes the standard deviation of the sample values.
    /// </summary>
    /// <param name="samples">The array of sample values.</param>
    /// <returns>The standard deviation of the samples.</returns>
    public static double ComputeStandardDeviation(this double[] samples)
    {
        var mean = samples.ComputeMean();
        return Math.Sqrt(samples.Sum(x => Math.Pow(x - mean, 2)) / samples.Length);
    }

    /// <summary>
    /// Computes the standard deviation of the sample values using a pre-calculated mean.
    /// </summary>
    /// <param name="samples">The array of sample values.</param>
    /// <param name="mean">The pre-calculated mean value.</param>
    /// <returns>The standard deviation of the samples.</returns>
    public static double ComputeStandardDeviation(this double[] samples, double mean)
    {
        return Math.Sqrt(samples.Sum(x => Math.Pow(x - mean, 2)) / samples.Length);
    }

    /// <summary>
    /// Computes the mode (most frequently occurring value) of the sample values.
    /// </summary>
    /// <param name="samples">The array of sample values.</param>
    /// <returns>The mode of the samples.</returns>
    public static double ComputeMode(this double[] samples)
    {
        return samples.GroupBy(v => v)
            .OrderByDescending(g => g.Count())
            .First().Key;
    }

    /// <summary>
    /// Computes the skewness of the sample values using pre-calculated mean and standard deviation.
    /// </summary>
    /// <param name="samples">The array of sample values.</param>
    /// <param name="mean">The pre-calculated mean value.</param>
    /// <param name="stdDev">The pre-calculated standard deviation.</param>
    /// <returns>The skewness of the samples.</returns>
    public static double ComputeSkewness(this double[] samples, double mean, double stdDev)
    {
        return samples.Sum(x => Math.Pow((x - mean) / stdDev, 3)) / samples.Length;
    }

    /// <summary>
    /// Computes the skewness of the sample values.
    /// </summary>
    /// <param name="samples">The array of sample values.</param>
    /// <returns>The skewness of the samples.</returns>
    public static double ComputeSkewness(this double[] samples)
    {
        var mean = samples.ComputeMean();
        var stdDev = samples.ComputeStandardDeviation();
        return samples.Sum(x => Math.Pow((x - mean) / stdDev, 3)) / samples.Length;
    }

    /// <summary>
    /// Computes the excess kurtosis of the sample values using pre-calculated mean and standard deviation.
    /// </summary>
    /// <param name="samples">The array of sample values.</param>
    /// <param name="mean">The pre-calculated mean value.</param>
    /// <param name="stdDev">The pre-calculated standard deviation.</param>
    /// <returns>The excess kurtosis of the samples.</returns>
    public static double ComputeKurtosis(this double[] samples, double mean, double stdDev)
    {
        var kurtosis = samples.Sum(x => Math.Pow((x - mean) / stdDev, 4)) / samples.Length;
        return kurtosis - 3; // Excess kurtosis
    }

    /// <summary>
    /// Computes the excess kurtosis of the sample values.
    /// </summary>
    /// <param name="samples">The array of sample values.</param>
    /// <returns>The excess kurtosis of the samples.</returns>
    public static double ComputeKurtosis(this double[] samples)
    {
        var mean = samples.ComputeMean();
        var stdDev = samples.ComputeStandardDeviation();
        var kurtosis = samples.Sum(x => Math.Pow((x - mean) / stdDev, 4)) / samples.Length;
        return kurtosis - 3; // Excess kurtosis
    }
}
