namespace IndTrace.Models.UnitTests.S7.Rx;
/// <summary>
/// Represents the PlcBatchWriterTests.
/// </summary>

public class PlcBatchWriterTests
{
    /// <summary>
    /// Executes WriteBatchValuesPlcAsync_RetriesWhenFirstWriteFails_ThenSucceedsOnSplit operation.
    /// </summary>
    /// <returns>The result of WriteBatchValuesPlcAsync_RetriesWhenFirstWriteFails_ThenSucceedsOnSplit.</returns>
    [Fact]
    public async Task WriteBatchValuesPlcAsync_RetriesWhenFirstWriteFails_ThenSucceedsOnSplit()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var factory = Substitute.For<IS7MultiVarFactory>();
        var valueConverter = Substitute.For<IValueConverter>();

        var multiVar1 = Substitute.For<IS7MultiVar>();
        var multiVar2 = Substitute.For<IS7MultiVar>();
        var multiVar3 = Substitute.For<IS7MultiVar>();

        // First write fails
        multiVar1.Write().Returns(1);
        // Splits: both halves succeed
        multiVar2.Write().Returns(0);
        multiVar3.Write().Returns(0);

        factory.Create().Returns(multiVar1, multiVar2, multiVar3);

        valueConverter.TryWriteToBuffer(Arg.Any<byte[]>(), Arg.Any<object>(), Arg.Any<Type>(), Arg.Any<VariableAddress>())
            .Returns(true);

        var batchWriter = new PlcBatchWriter(factory, valueConverter, logger);

        var tags = new List<PlcBatchWriteTag>
        {
            new("Tag1", typeof(int), 123),
            new("Tag2", typeof(int), 456)
        };

        batchWriter.ParseAndVerify = (alias, type) => new VariableAddress
        {
            Operand = 1,
            Type = 1,
            DbNo = 1,
            Start = 0,
            BufferLength = 4
        };

        // Act
        var result = await batchWriter.WriteBatchValuesPlcAsync(tags, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        factory.Received(3).Create();
        multiVar1.Received(1).Write();
        multiVar2.Received(1).Write();
        multiVar3.Received(1).Write();
    }
}
