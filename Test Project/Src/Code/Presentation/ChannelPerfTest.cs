using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Channels;

namespace ConsoleApp1;
/// <summary>
/// Represents the ChannelPerfTest.
/// </summary>

public class ChannelPerfTest
{
    private readonly Channel<long> _channel = Channel.CreateUnbounded<long>();
    private long _counter = 0;
    /// <summary>
    /// Executes RunAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of RunAsync.</returns>

    public Task RunAsync(CancellationToken cancellationToken)
    {
        var writer = Task.Run(() => WriterLoopAsync(cancellationToken), cancellationToken);
        var reader = Task.Run(() => ReaderLoopAsync(cancellationToken), cancellationToken);

        return Task.WhenAll(writer, reader);
    }

    private async Task WriterLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _counter++;
                await _channel.Writer.WriteAsync(Interlocked.Increment(ref _counter), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // expected on cancellation
        }
        finally
        {
            _channel.Writer.Complete();
        }
    }

    private async Task ReaderLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await _channel.Reader.WaitToReadAsync(cancellationToken))
            {
                await Task.Delay(1000, cancellationToken); // Simulate processing every second
                int messageCount = 0;

                while (_channel.Reader.TryRead(out _))
                {
                    messageCount++;
                }

                Console.WriteLine($"Messages read: {messageCount}");
            }
        }
        catch (OperationCanceledException)
        {
            // expected on cancellation
        }
    }
    /// <summary>
    /// Executes perf1 operation.
    /// </summary>
    /// <returns>The result of perf1.</returns>

    public async Task perf1()
    {
        Console.WriteLine("Hello, World!");
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var test = new ChannelPerfTest();

        await test.RunAsync(cts.Token);
    }
    /// <summary>
    /// Executes perf2Async operation.
    /// </summary>
    /// <returns>The result of perf2Async.</returns>

    public async Task perf2Async()
    {
        var c = Channel.CreateUnbounded<long>();

        _ = Task.Run(async () =>
        {
            for (int i = 0; ; i++)
            {
                await c.Writer.WriteAsync(i);
            }
        });

        long consumed = 0;

        _ = Task.Run(() =>
        {
            while (true)
            {
                long start = consumed;
                Thread.Sleep(1000);
                long end = consumed;

                Console.WriteLine($"{end - start:N0}");
            }
        });

        await foreach (var item in c.Reader.ReadAllAsync())
        {
            consumed++;
        }
    }
}
