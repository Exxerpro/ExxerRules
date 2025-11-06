using Neo4j.Driver;

namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers;

/// <summary>
/// Simple async enumerator for records.
/// </summary>
internal sealed class RecordAsyncEnumerator : IAsyncEnumerator<IRecord>
{
    private readonly IReadOnlyList<IRecord> _records;
    private readonly CancellationToken _cancellationToken;
    private int _index = -1;

    public RecordAsyncEnumerator(IReadOnlyList<IRecord> records, CancellationToken cancellationToken)
    {
        _records = records;
        _cancellationToken = cancellationToken;
    }

    public IRecord Current => _index >= 0 && _index < _records.Count ? _records[_index] : throw new InvalidOperationException();

    public ValueTask<bool> MoveNextAsync()
    {
        _cancellationToken.ThrowIfCancellationRequested();
        _index++;
        return ValueTask.FromResult(_index < _records.Count);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}