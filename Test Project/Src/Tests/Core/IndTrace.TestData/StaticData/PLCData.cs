namespace IndTrace.TestData.StaticData;

//[Fix]
//CLAUDE
//Date: 27/08/2025
//Reason: [Pattern Consolidation] - Moved static PLC data from DbContextStaticData for hybrid strategy optimization

/// <summary>
/// Raw PLC test data for static loading strategy - optimized for fast access.
/// This is a good candidate for static conversion due to stable, small dataset.
/// </summary>
internal static class PLCData
{

    /// <summary>
    /// Gets the predefined list of PLCs.
    /// </summary>
    public static List<Plc>
        GetPlcs() => new()
        {
            new Plc()
            {
                PlcId = 500,
                MachineId = 500,
                Name = "S7-1500",
                IpAddress = "192.168.0.140",
                PlcType = "S7-1500",
            },
                  new Plc()
            {
                PlcId = 600,
                MachineId = 600,
                Name = "S7-1500",
                IpAddress = "192.168.1.45",
                PlcType = "S7-1500",
            }
        };

}
