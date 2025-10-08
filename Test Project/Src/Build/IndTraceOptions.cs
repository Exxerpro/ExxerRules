using System;

[Flags]
public enum IndTraceOptions
{
    All = 1,
    Communications = 2,
    VirtualNetwork = 4,
    S7Monitor = 8,
    Monitor = 16,
    Intelligence = 32,
    OEE = 64,
    HubClient = 128,
    HubServer = 256,
    Tests = 512
}
