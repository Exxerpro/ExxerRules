using Sharp7.Rx;

namespace IndTrace.Models.UnitTests.S7.Rx;
/// <summary>
/// Represents the Sharp7PlcTests.
/// </summary>

public class Sharp7PlcTests
{
    private readonly Sharp7Plc _sut;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public Sharp7PlcTests()
    {
        _sut = new Sharp7Plc();
    }

    /// <summary>
    /// Executes GetBatchValuesPlcAsync_ShouldReturnConvertedValues operation.
    /// </summary>
    /// <returns>The result of GetBatchValuesPlcAsync_ShouldReturnConvertedValues.</returns>

    [Fact(Skip = "Bad designed test")]
    public async Task GetBatchValuesPlcAsync_ShouldReturnConvertedValues()
    {
        // Arrange
        var tags = new List<(string Alias, Type Type, int StatusValueId)>
        {
            ("Db1.DBW0", typeof(int), 101),
            ("Db1.DBB2", typeof(bool), 102)
        };

        byte[] buffer = { 0x01, 0x00, 0x01, 0x00 }; // Mocked raw memory block
        //the sut is not mocked so this is no make it any sense, what is this test testing, is even needed
        //candidate to delete
        //[TODO] [DELETE]
        // Delete non-sense test or rewrite it to make sense
        // I DONT WANT TO DELETE THIS TEST I WANT A ROBUST TEST FOR THIS BEHAVIOR
        //abr 20-08-2025
        _sut.When(x => x.GetValue<byte[]>(Arg.Any<string>(), Arg.Any<CancellationToken>()))
            .DoNotCallBase();

        _sut.GetValue<byte[]>(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(buffer));

        // Act
        var results = await _sut.GetBatchValuesPlcAsync(tags, TestContext.Current.CancellationToken);

        // Assert
        results.ShouldNotBeNull();
    }
}
