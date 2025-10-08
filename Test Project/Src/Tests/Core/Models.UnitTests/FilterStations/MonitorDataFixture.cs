namespace IndTrace.Models.UnitTests.FilterStations;

public static class MonitorDataFixture
{
    public static ApplicationConfiguration GenerateConfiguration(int customers, int partsPerCustomer)
    {
        var list = new List<MachineProductMap>();
        int productId = 1;
        for (int cust = 1; cust <= customers; cust++)
        {
            for (int i = 1; i <= partsPerCustomer; i++, productId++)
            {
                list.Add(new MachineProductMap
                {
                    MachineId = 100 + i,
                    PartNumber = $"PN{cust:D2}_{i:D3}",
                    CustomerId = cust,
                    ProductId = productId,
                    CustomerName = $"Customer{cust}"
                });
            }
        }

        return new ApplicationConfiguration
        {
            MachineProductCompatibility = list
        };
    }

    public static System.Collections.Generic.Dictionary<int, ControllerMonitor> GenerateControllerMonitors(int count, int customerId)
    {
        var dict = new System.Collections.Generic.Dictionary<int, ControllerMonitor>();
        for (int i = 1; i <= count; i++)
        {
            var part = $"PN{customerId:D2}_{(i % 20) + 1:D3}";
            dict[i] = new ControllerMonitor
            {
                MachineId = 100 + (i % 50),
                PartNumber = part,
                Name = $"WS{100 + (i % 10)}",
                Description = $"Monitor for {part}"
            };
        }
        return dict;
    }

    public static System.Collections.Generic.Dictionary<int, StationMonitor> GenerateStationMonitors(int count, int customerId)
    {
        var dict = new System.Collections.Generic.Dictionary<int, StationMonitor>();
        for (int i = 1; i <= count; i++)
        {
            var part = $"PN{customerId:D2}_{(i % 20) + 1:D3}";
            dict[i] = new StationMonitor
            {
                MachineId = 200 + (i % 50),
                PartNumber = part,
                Description = $"Station {part}",
                TimeStamp = DateTime.Now.AddSeconds(-i)
            };
        }
        return dict;
    }
}
