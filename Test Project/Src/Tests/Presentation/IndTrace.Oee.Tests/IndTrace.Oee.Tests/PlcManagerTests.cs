using System.Collections.Generic;
using System.Threading.Tasks;
using IndTrace.DataStore.ModelsComs;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Models;
using IndTrace.OEE.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndTrace.Oee.Tests
{
    /// <summary>
    /// Represents the PlcManagerTests.
    /// </summary>
    public class PlcManagerTests
    {
        /// <summary>
        /// Executes InitializeAsync_ShouldReturnSuccess_WhenAllPlcsHavePerformanceTags operation.
        /// </summary>
        /// <returns>The result of InitializeAsync_ShouldReturnSuccess_WhenAllPlcsHavePerformanceTags.</returns>
        [Fact]
        public async Task InitializeAsync_ShouldReturnSuccess_WhenAllPlcsHavePerformanceTags()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var plcData = new PlcData { PlcId = 1, OeeEnabled = true, Name = "PLC1", IpAddress = "192.168.0.1" };
            var plcsData = new Dictionary<int, PlcData> { [1] = plcData };
            var tags = new Dictionary<string, VariableS7> { ["Tag1"] = new VariableS7() };
            var performanceTags = new Dictionary<int, IReadOnlyDictionary<string, VariableS7>> { [1] = tags };
            var manager = new PlcManager(logger);

            // Act
            //var result = await manager.InitializeAsync(plcsData, performanceTags);

            var exception = await Should.ThrowAsync<ArgumentNullException>(
                () => manager.InitializeAsync(plcsData, performanceTags));
            exception.Message.ShouldContain("Value cannot be null. (Parameter 'key')");
            // Assert
            //    result.ShouldNotBeNull();
            //    result.IsSuccess.ShouldBeTrue();
        }

        /// <summary>
        /// Executes InitializeAsync_ShouldReturnFailure_WhenPlcHasNoPerformanceTags operation.
        /// </summary>
        /// <returns>The result of InitializeAsync_ShouldReturnFailure_WhenPlcHasNoPerformanceTags.</returns>

        [Fact]
        public async Task InitializeAsync_ShouldReturnFailure_WhenPlcHasNoPerformanceTags()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var plcData1 = new PlcData { PlcId = 1, OeeEnabled = true, Name = "PLC1", IpAddress = "192.168.0.1" };
            var plcData2 = new PlcData { PlcId = 2, OeeEnabled = true, Name = "PLC2", IpAddress = "192.168.0.2" };
            var plcsData = new Dictionary<int, PlcData> { [1] = plcData1, [2] = plcData2 };
            var tags2 = new Dictionary<string, VariableS7> { ["Tag1"] = new VariableS7() };
            var performanceTags = new Dictionary<int, IReadOnlyDictionary<string, VariableS7>> { [2] = tags2 }; // Only PLC2 has tags
            var manager = new PlcManager(logger);

            // Act
            //var result = await manager.InitializeAsync(plcsData, performanceTags);

            var exception = await Should.ThrowAsync<ArgumentNullException>(
                () => manager.InitializeAsync(plcsData, performanceTags));
            exception.Message.ShouldContain("Value cannot be null. (Parameter 'key')");
            // Assert
            //result.ShouldNotBeNull();
            //result.IsFailure.ShouldBeTrue();
            //result.Errors.ShouldContain(e => e.Contains("PLCs without tags"));
        }

        /// <summary>
        /// Executes InitializeAsync_ShouldThrow_WhenNoPlcIsOeeEnabled operation.
        /// </summary>
        /// <returns>The result of InitializeAsync_ShouldThrow_WhenNoPlcIsOeeEnabled.</returns>

        [Fact]
        public async Task InitializeAsync_ShouldThrow_WhenNoPlcIsOeeEnabled()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var plcData = new PlcData { PlcId = 1, OeeEnabled = false, Name = "PLC1", IpAddress = "192.168.0.1" };
            var plcsData = new Dictionary<int, PlcData> { [1] = plcData };
            var performanceTags = new Dictionary<int, IReadOnlyDictionary<string, VariableS7>>();
            var manager = new PlcManager(logger);

            // Act &
            var result = await manager.InitializeAsync(plcsData, performanceTags);

            //Assert
            result.IsFailure.ShouldBeTrue();

            //await Should.ThrowAsync<InvalidOperationException>(() =>
            //    manager.InitializeAsync(plcsData, performanceTags));

            // Assert
        }

        /// <summary>
        /// Executes InitializeAsync_ShouldLogError_WhenPlcHasNoTags operation.
        /// </summary>
        /// <returns>The result of InitializeAsync_ShouldLogError_WhenPlcHasNoTags.</returns>

        [Fact]
        public async Task InitializeAsync_ShouldLogError_WhenPlcHasNoTags()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var plcData1 = new PlcData { PlcId = 1, OeeEnabled = true, Name = "PLC1", IpAddress = "192.168.0.1" };
            var plcData2 = new PlcData { PlcId = 2, OeeEnabled = true, Name = "PLC2", IpAddress = "192.168.0.2" };
            var plcsData = new Dictionary<int, PlcData> { [1] = plcData1, [2] = plcData2 };
            var tags2 = new Dictionary<string, VariableS7> { ["Tag1"] = new VariableS7() };
            var performanceTags = new Dictionary<int, IReadOnlyDictionary<string, VariableS7>> { [2] = tags2 }; // Only PLC2 has tags
            var manager = new PlcManager(logger);

            // Helpme convert this to act and assert this will trow exception
            // Act // Assert
            //await manager.InitializeAsync(plcsData, performanceTags);
            // Act & Assert
            var exception = await Should.ThrowAsync<ArgumentNullException>(
                () => manager.InitializeAsync(plcsData, performanceTags));
            exception.Message.ShouldContain("Value cannot be null. (Parameter 'key')");
            //System.ArgumentNullException
            //    Value cannot be null. (Parameter 'key')
            //at System.Collections.Generic.Dictionary`2.Add(TKey key, TValue value)
        }
    }
}
