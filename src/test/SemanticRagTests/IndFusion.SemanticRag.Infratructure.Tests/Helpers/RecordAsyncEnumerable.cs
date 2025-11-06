using Neo4j.Driver;

namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers;

/// <summary>
/// Simple async enumerable for records.
/// </summary>
internal sealed class RecordAsyncEnumerable : IAsyncEnumerable<IRecord>
{
    private readonly IReadOnlyList<IRecord> _records;

    public RecordAsyncEnumerable(IReadOnlyList<IRecord> records)
    {
        _records = records;
    }

    public IAsyncEnumerator<IRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new RecordAsyncEnumerator(_records, cancellationToken);
}