using System;

namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Test-only entities and configurations for behavioral logging tests.
/// These support the Seq + Serilog behavioral testing infrastructure.
/// </summary>

/// <summary>
/// Test configuration for LLM service testing.
/// </summary>
public sealed class LLMServiceConfiguration
{
    public int DefaultMaxTokens { get; set; } = 1000;
    public float DefaultTemperature { get; set; } = 0.7f;
    public decimal MaxDailyCost { get; set; } = 100.00m;
}