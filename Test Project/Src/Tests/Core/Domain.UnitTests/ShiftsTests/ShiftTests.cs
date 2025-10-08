using IndTrace.Domain.Models;
using Microsoft.Extensions.Time.Testing;

namespace IndTrace.Domain.UnitTests.ShiftsTests;

/// <summary>
/// Unit tests for Shift domain entity
/// </summary>
public class ShiftTests
{
    /// <summary>
    /// Executes Shift_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully operation.
    /// </summary>
    [Fact]
    public void Shift_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully()
    {
        // Arrange
        var shiftId = 1;
        var startBy = DateTime.Now;
        var duration = TimeSpan.FromHours(8);
        var endTime = startBy.Add(duration);
        var shiftType = "Morning";
        var maxDuration = TimeSpan.FromHours(16);
        var minDuration = TimeSpan.FromHours(8);
        var normalDuration = TimeSpan.FromHours(8.5);
        var cyclesOk = 100;

        // Act
        var shift = new Shift(new DateTimeMachine())
        {
            ShiftId = shiftId,
            StartBy = startBy,
            Duration = duration,
            EndTime = endTime,
            ShiftType = shiftType,
            MaxDuration = maxDuration,
            MinDuration = minDuration,
            NormalDuration = normalDuration,
            CyclesOk = cyclesOk
        };

        // Assert
        shift.ShouldNotBeNull();
        shift.ShiftId.ShouldBe(shiftId);
        shift.StartBy.ShouldBe(startBy);
        shift.Duration.ShouldBe(duration);
        shift.EndTime.ShouldBe(endTime);
        shift.ShiftType.ShouldBe(shiftType);
        shift.MaxDuration.ShouldBe(maxDuration);
        shift.MinDuration.ShouldBe(minDuration);
        shift.NormalDuration.ShouldBe(normalDuration);
        shift.CyclesOk.ShouldBe(cyclesOk);
    }
    /// <summary>
    /// Executes Shift_WithDefaultConstructor_ShouldInitializeToExpectedDefaults operation.
    /// </summary>

    [Fact]
    public void Shift_WithDefaultConstructor_ShouldInitializeToExpectedDefaults()
    {
        // Arrange & Act
        var shift = new Shift(new DateTimeMachine());

        // Assert
        shift.ShouldNotBeNull();
        shift.ShiftId.ShouldBe(0);
        shift.StartBy.ShouldBe(default);
        shift.Duration.ShouldBe(default);
        shift.EndTime.ShouldBe(default);
        shift.ShiftType.ShouldBe(string.Empty);
        shift.MaxDuration.ShouldBe(TimeSpan.FromHours(16));
        shift.MinDuration.ShouldBe(TimeSpan.FromHours(16));
        shift.NormalDuration.ShouldBe(TimeSpan.FromHours(8.5));
        shift.CyclesOk.ShouldBe(0);
    }
    /// <summary>
    /// Executes Shift_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void Shift_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange
        var shift = new Shift(new DateTimeMachine());
        var shiftId = 2;
        var startBy = DateTime.Now.AddHours(1);
        var duration = TimeSpan.FromHours(12);
        var endTime = startBy.Add(duration);
        var shiftType = "Evening";
        var cyclesOk = 150;

        // Act
        shift.ShiftId = shiftId;
        shift.StartBy = startBy;
        shift.Duration = duration;
        shift.EndTime = endTime;
        shift.ShiftType = shiftType;
        shift.CyclesOk = cyclesOk;

        // Assert
        shift.ShiftId.ShouldBe(shiftId);
        shift.StartBy.ShouldBe(startBy);
        shift.Duration.ShouldBe(duration);
        shift.EndTime.ShouldBe(endTime);
        shift.ShiftType.ShouldBe(shiftType);
        shift.CyclesOk.ShouldBe(cyclesOk);
    }
    /// <summary>
    /// Executes Shift_Shift_Properties_ShouldSetAndGetCorrectly operation.
    /// </summary>

    [Fact]
    public void Shift_Shift_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var shift = new Shift(new DateTimeMachine());
        var now = DateTime.UtcNow;
        var duration = TimeSpan.FromHours(8);

        // Act
        shift.ShiftId = 1;
        shift.ShiftType = "Morning";
        shift.StartBy = now;
        shift.Duration = duration;
        shift.EndTime = now.Add(duration);
        shift.MaxDuration = TimeSpan.FromHours(10);
        shift.MinDuration = TimeSpan.FromHours(6);
        shift.NormalDuration = TimeSpan.FromHours(8);
        shift.CyclesOk = 150;

        // Assert
        shift.ShiftId.ShouldBe(1);
        shift.ShiftType.ShouldBe("Morning");
        shift.StartBy.ShouldBe(now);
        shift.Duration.ShouldBe(duration);
        shift.EndTime.ShouldBe(now.Add(duration));
        shift.MaxDuration.ShouldBe(TimeSpan.FromHours(10));
        shift.MinDuration.ShouldBe(TimeSpan.FromHours(6));
        shift.NormalDuration.ShouldBe(TimeSpan.FromHours(8));
        shift.CyclesOk.ShouldBe(150);
    }
    /// <summary>
    /// Executes IsRunningNow_ShouldBeFalse_WhenCurrentTimeIsBeforeStart operation.
    /// </summary>

    [Fact]
    public void IsRunningNow_ShouldBeFalse_WhenCurrentTimeIsBeforeStart()
    {
        // Arrange
        var shift = new Shift(new DateTimeMachine())
        {
            StartBy = DateTime.Now.AddHours(1),
            Duration = TimeSpan.FromHours(8)
        };

        // Act & Assert
        shift.IsRunningNow.ShouldBeFalse();
    }
    /// <summary>
    /// Executes IsRunningNow_ShouldBeTrue_WhenCurrentTimeIsAtStart operation.
    /// </summary>

    [Fact]
    public void IsRunningNow_ShouldBeTrue_WhenCurrentTimeIsAtStart()
    {
        // Arrange
        var shift = new Shift(new DateTimeMachine())
        {
            StartBy = DateTime.Now,
            Duration = TimeSpan.FromHours(8)
        };

        // Act & Assert
        shift.IsRunningNow.ShouldBeTrue();
    }
    /// <summary>
    /// Executes IsRunningNow_ShouldBeTrue_WhenCurrentTimeIsDuringShift operation.
    /// </summary>

    [Fact]
    public void IsRunningNow_ShouldBeTrue_WhenCurrentTimeIsDuringShift()
    {
        // Arrange
        var shift = new Shift(new DateTimeMachine())
        {
            StartBy = DateTime.Now.AddHours(-4),
            Duration = TimeSpan.FromHours(8)
        };

        // Act & Assert
        shift.IsRunningNow.ShouldBeTrue();
    }
    /// <summary>
    /// Executes IsRunningNow_ShouldBeTrue_WhenCurrentTimeIsAtEnd operation.
    /// </summary>

    [Fact]
    public void IsRunningNow_ShouldBeTrue_WhenCurrentTimeIsAtEnd()
    {
        DateTimeOffset today = new(2020, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        var date = new DateTimeMachine(new FakeTimeProvider(today));

        // Arrange
        var shift = new Shift(date)
        {
            StartBy = date.Now.AddHours(-9).AddMilliseconds(1),
            Duration = TimeSpan.FromHours(9)
        };

        // Act & Assert
        shift.IsRunningNow.ShouldBeTrue();
    }
    /// <summary>
    /// Executes IsRunningNow_ShouldBeFalse_WhenCurrentTimeIsAfterEnd operation.
    /// </summary>

    [Fact]
    public void IsRunningNow_ShouldBeFalse_WhenCurrentTimeIsAfterEnd()
    {
        // Arrange
        var shift = new Shift(new DateTimeMachine())
        {
            StartBy = DateTime.Now.AddHours(-9),
            Duration = TimeSpan.FromHours(8)
        };

        // Act & Assert
        shift.IsRunningNow.ShouldBeFalse();
    }
    /// <summary>
    /// Executes Shift_WhenShiftIsCreated_ShouldHaveDefaultValues operation.
    /// </summary>

    [Fact]
    public void Shift_WhenShiftIsCreated_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var shift = new Shift(new DateTimeMachine());

        // Assert
        shift.ShouldNotBeNull();
        shift.ShiftId.ShouldBe(0);
        shift.StartBy.ShouldBe(default);
        shift.Duration.ShouldBe(default);
        shift.EndTime.ShouldBe(default);
        shift.ShiftType.ShouldBe(string.Empty);
        shift.MaxDuration.ShouldBe(TimeSpan.FromHours(16));
        shift.MinDuration.ShouldBe(TimeSpan.FromHours(16));
        shift.NormalDuration.ShouldBe(TimeSpan.FromHours(8.5));
        shift.CyclesOk.ShouldBe(0);
    }
    /// <summary>
    /// Executes Shift_WhenShiftIsConfigured_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Shift_WhenShiftIsConfigured_ShouldBeValid()
    {
        // Arrange
        var startBy = DateTime.Today.AddHours(6); // 6 AM
        var duration = TimeSpan.FromHours(8);
        var endTime = startBy.Add(duration);

        var shift = new Shift(new DateTimeMachine())
        {
            ShiftId = 1,
            StartBy = startBy,
            Duration = duration,
            EndTime = endTime,
            ShiftType = "Morning",
            MaxDuration = TimeSpan.FromHours(16),
            MinDuration = TimeSpan.FromHours(8),
            NormalDuration = TimeSpan.FromHours(8.5),
            CyclesOk = 100
        };

        // Act & Assert
        shift.ShouldNotBeNull();
        shift.ShiftId.ShouldBe(1);
        shift.StartBy.ShouldBe(startBy);
        shift.Duration.ShouldBe(duration);
        shift.EndTime.ShouldBe(endTime);
        shift.ShiftType.ShouldBe("Morning");
        shift.MaxDuration.ShouldBe(TimeSpan.FromHours(16));
        shift.MinDuration.ShouldBe(TimeSpan.FromHours(8));
        shift.NormalDuration.ShouldBe(TimeSpan.FromHours(8.5));
        shift.CyclesOk.ShouldBe(100);
    }
    /// <summary>
    /// Executes Shift_WhenShiftHasZeroCycles_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Shift_WhenShiftHasZeroCycles_ShouldBeValid()
    {
        // Arrange
        var shift = new Shift(new DateTimeMachine())
        {
            ShiftId = 1,
            StartBy = DateTime.Now,
            Duration = TimeSpan.FromHours(8),
            EndTime = DateTime.Now.AddHours(8),
            ShiftType = "Night",
            CyclesOk = 0
        };

        // Act & Assert
        shift.ShouldNotBeNull();
        shift.CyclesOk.ShouldBe(0);
    }
    /// <summary>
    /// Executes Shift_WhenShiftHasNegativeCycles_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Shift_WhenShiftHasNegativeCycles_ShouldBeValid()
    {
        // Arrange
        var shift = new Shift(new DateTimeMachine())
        {
            ShiftId = 1,
            StartBy = DateTime.Now,
            Duration = TimeSpan.FromHours(8),
            EndTime = DateTime.Now.AddHours(8),
            ShiftType = "Night",
            CyclesOk = -1
        };

        // Act & Assert
        shift.ShouldNotBeNull();
        shift.CyclesOk.ShouldBe(-1);
    }
}
