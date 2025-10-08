using System.Linq;
using IndTrace.S7Rx.Models;

namespace GateWay.Tests.Simulation
{
    /// <summary>
    /// Represents the SimulatedCommandTests.
    /// </summary>
    public class SimulatedCommandTests
    {
        /// <summary>
        /// Executes ParseCommand_ShouldReturnCorrectSimulatedCommand_WhenValidCommandIsGiven operation.
        /// </summary>
        [Fact]
        public void ParseCommand_ShouldReturnCorrectSimulatedCommand_WhenValidCommandIsGiven()
        {
            // Arrange
            string commandLine = "m 100 pn 422290 bc abc cmd 4 ps 1 cs 2";

            List<string> commandsStringList = [commandLine];
            // Act
            var commands = SimulatedCommand.ParseCommands(commandsStringList);

            var result = commands.FirstOrDefault();
            // Assert
            result.ShouldNotBeNull();
            result.MachineId.ShouldBe(100);
            result.PartNumber.ShouldBe("422290");
            result.BarCode.ShouldBe("abc");
            result.Command.ShouldBeEquivalentTo((short)4);
            result.PartStatus.ShouldBe(1);
            result.CycleStatus.ShouldBe(2);
        }
        /// <summary>
        /// Executes ParseCommand_ShouldNotReturnACommand_WhenInvalidCommandIsGiven operation.
        /// </summary>

        [Fact]
        public void ParseCommand_ShouldNotReturnACommand_WhenInvalidCommandIsGiven()
        {
            // Arrange
            string invalidCommandLine = "m 100 pn 422290 bc abc cdsmd 4 ps"; // Missing cs and its value
            List<string> commandsStringList = [invalidCommandLine];

            // Act
            var commands = SimulatedCommand.ParseCommands(commandsStringList);
            // Assert

            commands.ShouldBeEmpty();
        }
        /// <summary>
        /// Executes ParseCommand_NonReturnACommand_WhenNonIntegerValuesAreGivenInIntegerFields operation.
        /// </summary>

        [Fact]
        public void ParseCommand_NonReturnACommand_WhenNonIntegerValuesAreGivenInIntegerFields()
        {
            // Arrange
            string invalidCommandLine = "m 100 pn 422290 bc abc cmd 4 ps notAnInteger cs 2";

            // Act
            List<string> commandsStringList = [invalidCommandLine];
            var commands = SimulatedCommand.ParseCommands(commandsStringList);

            // Assert
            commands.ShouldBeEmpty();
        }
    }
}
