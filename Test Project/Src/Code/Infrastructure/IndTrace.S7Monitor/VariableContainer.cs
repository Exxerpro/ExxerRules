// <copyright file="VariableContainer.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Manages a collection of PLC variable records and their subscriptions for real-time monitoring.
/// </summary>
public class VariableContainer : IDisposable
{
    private readonly IDisposable subscriptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableContainer"/> class.
    /// </summary>
    /// <param name="variableRecords">The list of variable records.</param>
    /// <param name="subscriptions">The subscription disposable.</param>
    private VariableContainer(IReadOnlyList<VariableRecord> variableRecords, IDisposable subscriptions)
    {
        this.subscriptions = subscriptions;
        this.VariableRecords = variableRecords;
    }

    /// <summary>
    /// Gets the collection of variable records managed by this container.
    /// </summary>
    public IReadOnlyList<VariableRecord> VariableRecords { get; }

    /// <summary>
    /// Disposes of the variable subscriptions and releases associated resources.
    /// </summary>
    public void Dispose()
    {
        this.subscriptions.Dispose();
    }

    /// <summary>
    /// Initializes a new variable container with the specified PLC and variable addresses.
    /// </summary>
    /// <param name="plc">The PLC interface to connect to.</param>
    /// <param name="variables">The list of variable addresses to monitor.</param>
    /// <returns>A new <see cref="VariableContainer"/> instance with configured variable monitoring.</returns>
    public static VariableContainer Initialize(IPlc plc, IReadOnlyList<string> variables)
    {
        var records = variables
            .Select((v, i) => new VariableRecord
            {
                Address = v,
                Description = string.Empty,
                RowIdx = i,
                Value = new Text("init", CustomStyles.Note),
            })
            .ToList();

        var disposables = new CompositeDisposable();
        foreach (var rec in records)
        {
            try
            {
                var disp = plc.CreateNotification(rec.Address, TransmissionMode.OnChange)
                    .Subscribe(
                        data =>
                        {
                            Console.WriteLine($"Data updated for {rec.Address}: {data}"); // Debugging output
                            rec.Value = data;
                        },
                        ex =>
                        {
                            Console.WriteLine($"Error updating {rec.Address}: {ex.Message}"); // Debugging output
                            rec.Value = new Text(ex.Message, CustomStyles.Error);
                        });
            }
            catch (Exception e)
            {
                rec.Value = new Text(e.Message, CustomStyles.Error);
            }
        }

        return new VariableContainer(records, disposables);
    }

    /// <summary>
    /// Initializes a new variable container with multiple PLCs and their associated variable data.
    /// </summary>
    /// <param name="plcs">A dictionary of PLC data models keyed by PLC ID.</param>
    /// <returns>A new <see cref="VariableContainer"/> instance with configured multi-PLC variable monitoring.</returns>
    public static VariableContainer Initialize(Dictionary<int, PlcDataModels> plcs)
    {
        var disposables = new CompositeDisposable();
        var records = new List<VariableRecord>();

        foreach (var (id, plcdata) in plcs)
        {
            var tags = plcdata.Tags;
            var prefix = "plc" + plcdata.PlcId.ToString("D3") + ":";
            var idxOffset = records.Count;

            records.AddRange(tags
               .Select((t, i) => new VariableRecord
               {
                   Address = t.Address,
                   Description = prefix + t.Description,
                   PlcId = plcdata.PlcId,
                   RowIdx = i + idxOffset,
                   Value = new Text("t", CustomStyles.Note),
               })
               .ToList());
        }

        foreach (var rec in records)
        {
            var plc = plcs[rec.PlcId].Plc;
            try
            {
                var disposable =
                    plc.CreateNotification(rec.Address, TransmissionMode.OnChange)
                        .Subscribe(
                            data => rec.Value = data,
                            ex => rec.Value = new Text(ex.Message, CustomStyles.Error));
                disposables.Add(disposable);
            }
            catch (Exception e)
            {
                rec.Value = new Text(e.Message, CustomStyles.Error);
            }
        }

        return new VariableContainer(records, disposables);
    }
}
