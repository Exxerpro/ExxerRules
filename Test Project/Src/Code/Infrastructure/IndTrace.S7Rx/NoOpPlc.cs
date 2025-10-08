using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Sharp7.Rx.BatchRead;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;

namespace IndTrace.S7Rx;

internal sealed class NoOpSharp7Connector : ISharp7Connector
{
    private readonly BehaviorSubject<ConnectionState> _state = new(Sharp7.Rx.Enums.ConnectionState.Initial);

    public IObservable<ConnectionState> ConnectionState => _state.AsObservable();

    public ILogger Logger { get; set; } = LoggerFactory.Create(builder => { }).CreateLogger("NoOpSharp7Connector");

    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(1);

    public Task<bool> Connect() => Task.FromResult(true);

    public Task Disconnect()
    {
        return Task.CompletedTask;
    }

    public Task<bool> WriteBatchValuesPlcAsync(IEnumerable<PlcBatchWriteTag> tags, CancellationToken token = default)
        => Task.FromResult(true);

    public Task<IReadOnlyDictionary<string, byte[]>> ExecuteMultiVarRequestGateway(IReadOnlyList<string> variableNames, CancellationToken token = default)
        => Task.FromResult((IReadOnlyDictionary<string, byte[]>)new Dictionary<string, byte[]>());

    public Task<IReadOnlyDictionary<string, byte[]>> ExecuteMultiVarRequest(IReadOnlyList<string> variableNames, CancellationToken token = default)
        => Task.FromResult((IReadOnlyDictionary<string, byte[]>)new Dictionary<string, byte[]>());

    public Task DisconnectS7Client() => Task.CompletedTask;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task<byte[]> ReadBytes(Operand operand, ushort startByteAddress, ushort bytesToRead, ushort dbNo, CancellationToken token)
        => Task.FromResult(Array.Empty<byte>());

    public Task<byte[]> ReadBytesS7Client(Operand operand, ushort startByteAddress, ushort bytesToRead, ushort dbNo, CancellationToken token)
        => Task.FromResult(Array.Empty<byte>());

    public Task WriteBit(Operand operand, ushort startByteAddress, byte bitAdress, bool value, ushort dbNo, CancellationToken token)
        => Task.CompletedTask;

    public Task WriteBytes(Operand operand, ushort startByteAddress, byte[] data, ushort dbNo, ushort bytesToWrite, CancellationToken token)
        => Task.CompletedTask;

    public void EnsureConnectionS7Valid() { }

    public void Dispose()
    {
        _state.Dispose();
    }
}

internal sealed class NoOpPlc : IPlc
{
    public NoOpPlc()
    {
        this.Logger = LoggerFactory.Create(builder => { }).CreateLogger("NoOpPlc");
        var connector = new NoOpSharp7Connector { Logger = this.Logger };
        this.S7Connector = connector;
        this.s7Connector = connector;
    }

    public Task<IEnumerable<PlcBatchReadResult>> GetBatchValuesPlcAsync(IEnumerable<(string Alias, Type Type, int StatusValueId)> tags, CancellationToken token = default)
        => Task.FromResult<IEnumerable<PlcBatchReadResult>>(Array.Empty<PlcBatchReadResult>());

    public ILogger Logger { get; set; }

    public ISharp7Connector S7Connector { get; set; }

    public ISharp7Connector s7Connector { get; set; }

    public IObservable<ConnectionState> ConnectionState => Observable.Return(Sharp7.Rx.Enums.ConnectionState.Initial);

    public Task SetValue<TValue>(string variableName, TValue value, CancellationToken token = default)
        => Task.CompletedTask;

    public Task<TValue> GetValue<TValue>(string variableName, CancellationToken token = default)
    {
        if (typeof(TValue).IsValueType)
        {
            var inst = Activator.CreateInstance(typeof(TValue));
            return Task.FromResult((TValue)inst!);
        }
        if (typeof(TValue) == typeof(string))
        {
            return Task.FromResult((TValue)(object)string.Empty);
        }
        var ctor = typeof(TValue).GetConstructor(Type.EmptyTypes);
        if (ctor is not null)
        {
            var obj = ctor.Invoke(null);
            return Task.FromResult((TValue)obj);
        }
        throw new NotSupportedException($"No-op PLC cannot provide a default instance for type {typeof(TValue).FullName}.");
    }

    public Task<object> GetValue(string variableName, CancellationToken token = default)
        => Task.FromResult((object)string.Empty);

    public IObservable<TValue> CreateNotification<TValue>(string variableName, TransmissionMode transmissionMode)
        => Observable.Empty<TValue>();

    public IObservable<object> CreateNotification(string variableName, TransmissionMode transmissionMode)
        => Observable.Empty<object>();

    public Task<TValue> GetValueS7Client<TValue>(string variableName, CancellationToken token = default)
        => GetValue<TValue>(variableName, token);

    public Task InitializeConnection(CancellationToken token = default) => Task.CompletedTask;

    public void Dispose() { }
}
