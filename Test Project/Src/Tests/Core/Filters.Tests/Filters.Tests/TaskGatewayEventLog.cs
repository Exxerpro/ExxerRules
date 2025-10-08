namespace IndTrace.Filters.Tests;

public static class TaskGatewayEventLog
{
    public static void LogEvents(IEnumerable<TaskGatewayRequest> events, string title = "Event Log")
    {
        Console.WriteLine($"\n=== {title} ===");
        foreach (var e in events.OrderBy(e => e.TimeStamp))
        {
            Console.WriteLine(e);
        }
    }

    public static void LogEvents(IEnumerable<TaskGatewayRequest> events, ITestOutputHelper output, string title = "Event Log")
    {
        output.WriteLine($"\n=== {title} ===");
        foreach (var e in events.Where(e => e is not null).OrderBy(e => e.TimeStamp))
        {
            if (e is null || e.ToString() is null)
            {
                continue;
            }
            output.WriteLine(e.BarCode);
        }
    }

    public static string FormatAsUiTable(IEnumerable<TaskGatewayRequest> events)
    {
        var sb = new StringBuilder();
        sb.AppendLine("| TimeStamp | ProductId | PartNumber | MachineId | Name |");
        sb.AppendLine("|-----------|-----------|------------|-----------|------|");

        foreach (var e in events.OrderBy(e => e.TimeStamp))
        {
            sb.AppendLine($"| {e.TimeStamp:HH:mm:ss} | {e.BarCode} | {e.PartNumber} | {e.MachineId} | {e.Name} |");
        }

        return sb.ToString();
    }

    public static void LogAsUiTable(IEnumerable<TaskGatewayRequest> events, ITestOutputHelper output)
    {
        output.WriteLine("| TimeStamp | ProductId | PartNumber | MachineId | Name |");
        output.WriteLine("|-----------|-----------|------------|-----------|------|");

        foreach (var e in events.OrderBy(e => e.TimeStamp))
        {
            output.WriteLine($"| {e.TimeStamp:HH:mm:ss} | {e.BarCode} | {e.PartNumber} | {e.MachineId} | {e.Name} |");
        }
    }

    public static void LogAsUiTable(IEnumerable<TaskGatewayRequest> events)
    {
        Console.WriteLine("| TimeStamp | ProductId | PartNumber | MachineId | Name |");
        Console.WriteLine("|-----------|-----------|------------|-----------|------|");

        foreach (var e in events.OrderBy(e => e.TimeStamp))
        {
            Console.WriteLine($"| {e.TimeStamp:HH:mm:ss} | {e.BarCode} | {e.PartNumber} | {e.MachineId} | {e.Name} |");
        }
    }
}
