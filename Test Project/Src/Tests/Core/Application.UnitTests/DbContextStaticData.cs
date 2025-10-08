namespace Application.UnitTests;

public static class DbContextStaticData
{
    public static readonly IReadOnlyList<Machine> _fixtureMachines = new List<Machine>
            {
                new Machine
                {
                    MachineId = 0,
                    Name = "End/Start ProcessAsync",
                    Description = "Dummy Machine",
                    Location = "N/A",
                    MachineType = 0,
                    WorkFlowType = 0,
                    EnableAppTraceability = 0,
                    EnableBypassTraceability = 1,
                    RuleId = 0
                },
                new Machine
                {
                    MachineId = 100,
                    Name = "WS100",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 4,
                    WorkFlowType = 1,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 100
                },
                new Machine
                {
                    MachineId = 200,
                    Name = "WS200",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 8,
                    WorkFlowType = 2,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 200
                },
                new Machine
                {
                    MachineId = 300,
                    Name = "WS300",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 8,
                    WorkFlowType = 2,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 300
                },
                new Machine
                {
                    MachineId = 400,
                    Name = "WS400",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 8,
                    WorkFlowType = 2,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 400
                },
                new Machine
                {
                    MachineId = 500,
                    Name = "WS500",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 16,
                    WorkFlowType = 2,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 500
                },
                new Machine
                {
                    MachineId = 600,
                    Name = "WS600",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 16,
                    WorkFlowType = 3,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 600
                },
                new Machine
                {
                    MachineId = 700,
                    Name = "WS700",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 32,
                    WorkFlowType = 3,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 700
                },
                new Machine
                {
                    MachineId = 800,
                    Name = "WS800",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 64,
                    WorkFlowType = 4,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 800
                },
                new Machine
                {
                    MachineId = 900,
                    Name = "WS900",
                    Description = "SPOILER",
                    Location = "SPOILER",
                    MachineType = 128,
                    WorkFlowType = 4,
                    EnableAppTraceability = 1,
                    EnableBypassTraceability = 0,
                    RuleId = 900
                }
            };

    public static readonly IReadOnlyList<Plc> _fixturePlcs = new List<Plc>
{
    new Plc
    {
        PlcId = 100,
        MachineId = 100,
        Name = "S7-1200",
        IpAddress = "192.168.0.100",
        PlcType = "S7-1200",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    },
    new Plc
    {
        PlcId = 400,
        MachineId = 400,
        Name = "S7-1200",
        IpAddress = "192.168.0.4",
        PlcType = "S7-1200",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    },
    new Plc
    {
        PlcId = 500,
        MachineId = 500,
        Name = "S7-1200",
        IpAddress = "192.168.0.30",
        PlcType = "S7-1200",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    },
    new Plc
    {
        PlcId = 600,
        MachineId = 600,
        Name = "S7-1500",
        IpAddress = "192.168.1.100",
        PlcType = "S7-1500",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    },
    new Plc
    {
        PlcId = 700,
        MachineId = 700,
        Name = "S7-1500",
        IpAddress = "192.168.1.101",
        PlcType = "S7-1500",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    },
    new Plc
    {
        PlcId = 800,
        MachineId = 800,
        Name = "S7-1500",
        IpAddress = "192.168.1.102",
        PlcType = "S7-1500",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    },
    new Plc
    {
        PlcId = 900,
        MachineId = 900,
        Name = "S7-1500",
        IpAddress = "192.168.1.103",
        PlcType = "S7-1500",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    },
    new Plc
    {
        PlcId = 200,
        MachineId = 200,
        Name = "S7-1200",
        IpAddress = "192.168.0.101",
        PlcType = "S7-1200",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    },
    new Plc
    {
        PlcId = 300,
        MachineId = 300,
        Name = "S7-1200",
        IpAddress = "192.168.0.102",
        PlcType = "S7-1200",
        PlcBrand = "Siemens",
        Options = "[{\"Rack\": 0, \"Slot\": 1, \"TSAP\" : \"FD.01\"}]",
        CommLibrary = "S7-Link",
        BrandOwner = "Siemens",
        Enabled = 1
    }
};
}
