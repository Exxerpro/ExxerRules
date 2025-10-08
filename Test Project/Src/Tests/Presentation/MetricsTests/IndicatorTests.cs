using Xunit;
using IndTrace.UI.Models.Metrics;
using Shouldly;

namespace IndTrace.Application.UnitTests;

/// <summary>
/// Contains unit tests for various indicator calculations in the metrics system.
/// </summary>
public class IndicatorTests
{
    /// <summary>
    /// Executes WorkInProgress_ShouldCalculate_Correctly operation.
    /// </summary>
    [Fact]
    public void WorkInProgress_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.WorkInProgress
        {
            Data = new double[,]
            {
                { 1 },  // In Progress
                { 0 },  // Not In Progress
                { 1 }   // In Progress
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(2);
    }

    /// <summary>
    /// Executes WorkInProgress_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void WorkInProgress_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.WorkInProgress
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes AverageCycleTime_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void AverageCycleTime_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.AverageCycleTime
        {
            Data = new double[,]
            {
                { 45 },
                { 50 },
                { 55 }
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(50);
    }

    /// <summary>
    /// Executes AverageCycleTime_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void AverageCycleTime_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.AverageCycleTime
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes UtilizationRate_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void UtilizationRate_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.UtilizationRate
        {
            Data = new double[,]
            {
                { 400, 480 }  // TotalProductionTime, TotalAvailableTime
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBeInRange(83.3, 83.34);
    }

    /// <summary>
    /// Executes UtilizationRate_ShouldHandle_Division_By_Zero operation.
    /// </summary>

    [Fact]
    public void UtilizationRate_ShouldHandle_Division_By_Zero()
    {
        // Arrange
        var indicator = new MetricsClasses.UtilizationRate
        {
            Data = new double[,]
            {
                { 400, 0 }  // TotalProductionTime, TotalAvailableTime
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(double.PositiveInfinity);
    }

    /// <summary>
    /// Executes TaktTime_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void TaktTime_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.TaktTime
        {
            Data = new double[,]
            {
                { 480, 120 }  // TotalAvailableProductionTime, CustomerDemand
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(4);
    }

    /// <summary>
    /// Executes TaktTime_ShouldHandle_Division_By_Zero operation.
    /// </summary>

    [Fact]
    public void TaktTime_ShouldHandle_Division_By_Zero()
    {
        // Arrange
        var indicator = new MetricsClasses.TaktTime
        {
            Data = new double[,]
            {
                { 480, 0 }  // TotalAvailableProductionTime, CustomerDemand
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(double.PositiveInfinity);
    }

    /// <summary>
    /// Executes Downtime_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void Downtime_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.Downtime
        {
            Data = new double[,]
            {
                { 10, 20 },  // Downtime period 1
                { 30, 40 }   // Downtime period 2
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(20);
    }

    /// <summary>
    /// Executes Downtime_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void Downtime_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.Downtime
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes ProcessEfficiency_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void ProcessEfficiency_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.ProcessEfficiency
        {
            Data = new double[,]
            {
                { 400, 500 }  // SumOfCycleTimes, TotalTime
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(80);
    }

    /// <summary>
    /// Executes ProcessEfficiency_ShouldHandle_Division_By_Zero operation.
    /// </summary>

    [Fact]
    public void ProcessEfficiency_ShouldHandle_Division_By_Zero()
    {
        // Arrange
        var indicator = new MetricsClasses.ProcessEfficiency
        {
            Data = new double[,]
            {
                { 400, 0 }  // SumOfCycleTimes, TotalTime
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(double.PositiveInfinity);
    }

    /// <summary>
    /// Executes ScrapRate_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void ScrapRate_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.ScrapRate
        {
            Data = new double[,]
            {
                { 200, 10 }  // TotalProduced, ScrappedParts
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(5);
    }

    /// <summary>
    /// Executes ScrapRate_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void ScrapRate_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.ScrapRate
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes OnTimeDeliveryRate_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void OnTimeDeliveryRate_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.OnTimeDeliveryRate
        {
            Data = new double[,]
            {
                { 90, 100 }  // OnTimeDeliveries, TotalDeliveries
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(90);
    }

    /// <summary>
    /// Executes OnTimeDeliveryRate_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void OnTimeDeliveryRate_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.OnTimeDeliveryRate
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes FirstPassYield_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void FirstPassYield_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.FirstPassYield
        {
            Data = new double[,]
            {
                { 85, 100 }  // PartsPassingInspection, TotalProduced
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(85);
    }

    /// <summary>
    /// Executes FirstPassYield_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void FirstPassYield_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.FirstPassYield
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes OverallEquipmentEffectiveness_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void OverallEquipmentEffectiveness_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.OverallEquipmentEffectiveness
        {
            Data = new double[,]
            {
                { 0.9, 0.85, 0.95 }  // AvailabilityRate, PerformanceRate, QualityRate
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBeInRange(0.726, 0.727);
    }

    /// <summary>
    /// Executes OverallEquipmentEffectiveness_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void OverallEquipmentEffectiveness_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.OverallEquipmentEffectiveness
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes ProductionRate_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void ProductionRate_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.ProductionRate
        {
            Data = new double[,]
            {
                { 200, 8 }  // TotalProducedParts, TotalProductionTime
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(25);
    }

    /// <summary>
    /// Executes ProductionRate_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void ProductionRate_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.ProductionRate
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes ReworkRate_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void ReworkRate_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.ReworkRate
        {
            Data = new double[,]
            {
                { 100, 15 }  // TotalProduced, ReworkedParts
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(15);
    }

    /// <summary>
    /// Executes ReworkRate_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void ReworkRate_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.ReworkRate
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes CustomerRejects_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void CustomerRejects_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.CustomerRejects
        {
            Data = new double[,]
            {
                { 3 }  // CustomerRejectsCount
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(3);
    }

    /// <summary>
    /// Executes CustomerRejects_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void CustomerRejects_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.CustomerRejects
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes SetupTime_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void SetupTime_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.SetupTime
        {
            Data = new double[,]
            {
                { 10, 40 }  // SetupStartTime, SetupEndTime
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(30);
    }

    /// <summary>
    /// Executes SetupTime_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void SetupTime_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.SetupTime
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes ChangeoverTime_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void ChangeoverTime_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.ChangeoverTime
        {
            Data = new double[,]
            {
                { 20, 65 }  // ChangeoverStartTime, ChangeoverEndTime
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(45);
    }

    /// <summary>
    /// Executes ChangeoverTime_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void ChangeoverTime_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.ChangeoverTime
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes InventoryTurnover_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void InventoryTurnover_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.InventoryTurnover
        {
            Data = new double[,]
            {
                { 500000, 100000 }  // CostOfGoodsSold, AverageInventory
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(5);
    }

    /// <summary>
    /// Executes InventoryTurnover_ShouldHandle_Division_By_Zero operation.
    /// </summary>

    [Fact]
    public void InventoryTurnover_ShouldHandle_Division_By_Zero()
    {
        // Arrange
        var indicator = new MetricsClasses.InventoryTurnover
        {
            Data = new double[,]
            {
                { 500000, 0 }  // CostOfGoodsSold, AverageInventory
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(double.PositiveInfinity);
    }

    /// <summary>
    /// Executes OrderFulfillmentCycleTime_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void OrderFulfillmentCycleTime_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.OrderFulfillmentCycleTime
        {
            Data = new double[,]
            {
                { 10, 13 }  // OrderReceiptTime, OrderDeliveryTime
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(3);
    }

    /// <summary>
    /// Executes OrderFulfillmentCycleTime_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void OrderFulfillmentCycleTime_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.OrderFulfillmentCycleTime
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }

    /// <summary>
    /// Executes MeanTimeBetweenFailures_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void MeanTimeBetweenFailures_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.MeanTimeBetweenFailures
        {
            Data = new double[,]
            {
                { 10000, 5 }  // TotalOperatingTime, NumberOfFailures
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(2000);
    }

    /// <summary>
    /// Executes MeanTimeBetweenFailures_ShouldHandle_Division_By_Zero operation.
    /// </summary>

    [Fact]
    public void MeanTimeBetweenFailures_ShouldHandle_Division_By_Zero()
    {
        // Arrange
        var indicator = new MetricsClasses.MeanTimeBetweenFailures
        {
            Data = new double[,]
            {
                { 10000, 0 }  // TotalOperatingTime, NumberOfFailures
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(double.PositiveInfinity);
    }

    /// <summary>
    /// Executes MeanTimeToRepair_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void MeanTimeToRepair_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.MeanTimeToRepair
        {
            Data = new double[,]
            {
                { 50, 10 }  // TotalRepairTime, NumberOfRepairs
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(5);
    }

    /// <summary>
    /// Executes MeanTimeToRepair_ShouldHandle_Division_By_Zero operation.
    /// </summary>

    [Fact]
    public void MeanTimeToRepair_ShouldHandle_Division_By_Zero()
    {
        // Arrange
        var indicator = new MetricsClasses.MeanTimeToRepair
        {
            Data = new double[,]
            {
                { 50, 0 }  // TotalRepairTime, NumberOfRepairs
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(double.PositiveInfinity);
    }

    /// <summary>
    /// Executes LaborEfficiency_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void LaborEfficiency_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.LaborEfficiency
        {
            Data = new double[,]
            {
                { 1000, 500 }  // TotalOutput, TotalLaborHours
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(2);
    }

    /// <summary>
    /// Executes LaborEfficiency_ShouldHandle_Division_By_Zero operation.
    /// </summary>

    [Fact]
    public void LaborEfficiency_ShouldHandle_Division_By_Zero()
    {
        // Arrange
        var indicator = new MetricsClasses.LaborEfficiency
        {
            Data = new double[,]
            {
                { 1000, 0 }  // TotalOutput, TotalLaborHours
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(double.PositiveInfinity);
    }

    /// <summary>
    /// Executes ScheduleAdherence_ShouldCalculate_Correctly operation.
    /// </summary>

    [Fact]
    public void ScheduleAdherence_ShouldCalculate_Correctly()
    {
        // Arrange
        var indicator = new MetricsClasses.ScheduleAdherence
        {
            Data = new double[,]
            {
                { 90, 100 }  // OnScheduleProduction, TotalScheduledProduction
            }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(90);
    }

    /// <summary>
    /// Executes ScheduleAdherence_ShouldHandle_Empty_Data operation.
    /// </summary>

    [Fact]
    public void ScheduleAdherence_ShouldHandle_Empty_Data()
    {
        // Arrange
        var indicator = new MetricsClasses.ScheduleAdherence
        {
            Data = new double[,] { }
        };

        // Act
        indicator.CalculateValue();

        // Assert
        indicator.Value.ShouldBe(0);
    }
}
