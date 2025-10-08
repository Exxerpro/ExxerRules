// <copyright file="RegisterCleanerTests.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.UnitTests.Cycles.Services;

using IndTrace.Application.Cycles.Services;
using Meziantou.Extensions.Logging.Xunit;

/// <summary>
/// Unit tests for RegisterCleaner.
/// </summary>
public class RegisterCleanerTests
{
    private readonly ILogger<RegisterCleaner> _logger;
    private readonly RegisterCleaner _cleaner;
    private readonly DateTime _testTimestamp = new(2025, 9, 17, 12, 0, 0);

    public RegisterCleanerTests(ITestOutputHelper output)
    {
        _logger = XUnitLogger.CreateLogger<RegisterCleaner>(output);
        _cleaner = new RegisterCleaner(_logger);
    }

    [Fact]
    public void CleanRegisters_WithNullRegisters_ShouldReturnFailure()
    {
        // Act
        var result = _cleaner.CleanRegisters(null!, 1, 100, _testTimestamp);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("registers");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CleanRegisters_WithInvalidCycleId_ShouldReturnFailure(int cycleId)
    {
        // Arrange
        var registers = new Dictionary<string, Register>();

        // Act
        var result = _cleaner.CleanRegisters(registers, cycleId, 100, _testTimestamp);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("cycleId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CleanRegisters_WithInvalidMachineId_ShouldReturnFailure(int machineId)
    {
        // Arrange
        var registers = new Dictionary<string, Register>();

        // Act
        var result = _cleaner.CleanRegisters(registers, 1, machineId, _testTimestamp);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("machineId");
    }

    [Fact]
    public void CleanRegisters_WithDirtyRegisters_ShouldCleanAndSetMetadata()
    {
        // Arrange
        var registers = new Dictionary<string, Register>
        {
            ["temp"] = new Register
            {
                RegisterId = 999,
                Name = "  Temperature\n\r\t",
                Description = "Temp\tsensor\nreading",
                Value = "25.5\r\n",
                CycleId = 0,
                MachineId = 0,
                TimeStamp = DateTime.MinValue
            },
            ["pressure"] = new Register
            {
                Name = "Pressure",
                Description = string.Empty,
                Value = string.Empty
            }
        };

        // Act
        var result = _cleaner.CleanRegisters(registers, 123, 456, _testTimestamp);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var cleanedList = result.Value.ToList();
        cleanedList.Count.ShouldBe(2);

        var tempRegister = cleanedList.First(r => r.Name == "Temperature");
        tempRegister.Name.ShouldBe("Temperature");
        tempRegister.Description.ShouldBe("Tempsensorreading");
        tempRegister.Value.ShouldBe("25.5");
        tempRegister.RegisterId.ShouldBe(0);
        tempRegister.CycleId.ShouldBe(123);
        tempRegister.MachineId.ShouldBe(456);
        tempRegister.TimeStamp.ShouldBe(_testTimestamp);

        var pressureRegister = cleanedList.First(r => r.Name == "Pressure");
        pressureRegister.Description.ShouldBe(string.Empty);
        pressureRegister.Value.ShouldBe(string.Empty);
    }

    [Fact]
    public void CleanRegisters_WithNullRegisterInDictionary_ShouldSkipIt()
    {
        // Arrange
        var registers = new Dictionary<string, Register>
        {
            ["good"] = new Register { Name = "Good", Value = "123" },
            ["null"] = null!,
            ["another"] = new Register { Name = "Another", Value = "456" }
        };

        // Act
        var result = _cleaner.CleanRegisters(registers, 1, 100, _testTimestamp);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(2);
        result.Value.Any(r => r.Name == "Good").ShouldBeTrue();
        result.Value.Any(r => r.Name == "Another").ShouldBeTrue();
    }

    [Fact]
    public void CleanRegisters_WithEmptyDictionary_ShouldReturnEmptyList()
    {
        // Arrange
        var registers = new Dictionary<string, Register>();

        // Act
        var result = _cleaner.CleanRegisters(registers, 1, 100, _testTimestamp);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(0);
    }
}
