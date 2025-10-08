// <copyright file="Operand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Enums;

/// <summary>
/// Defines the S7 PLC memory area operand types used for addressing variables in Siemens PLCs.
/// </summary>
/// <remarks>
/// These byte values correspond to the S7 communication protocol area specifications.
/// The DB value was corrected from 68 to 132 based on Wireshark packet analysis to resolve
/// communication errors with multi-variable write operations.
/// </remarks>
public enum Operand : byte
{
    /// <summary>
    /// Input area (I) - Digital and analog inputs from field devices.
    /// </summary>
    /// <value>Hex value: 0x45 (69 decimal).</value>
    Input = 69,

    /// <summary>
    /// Output area (Q) - Digital and analog outputs to field devices.
    /// </summary>
    /// <value>Hex value: 0x41 (65 decimal).</value>
    Output = 65,

    /// <summary>
    /// Marker area (M) - Internal memory for intermediate calculations and flags.
    /// </summary>
    /// <value>Hex value: 0x4D (77 decimal).</value>
    Marker = 77,

    /// <summary>
    /// Data Block (DB) area in PLC memory used for structured data storage.
    /// </summary>
    /// <value>Hexadecimal: 0x84 (decimal: 132).</value>
    /// ///Db = 68,    /// <remarks>
    ///
    /// This constant was corrected from <c>0x44</c> (68 decimal) to <c>0x84</c> (132 decimal)
    /// on April 27, 2025. Analysis using Wireshark confirmed that write operations to DBs
    /// succeed only when using area code <c>0x84</c>. The previous value led to intermittent
    /// communication failures with certain Siemens S7 controllers.
    /// </remarks>
    Db = 132, // here is the patch
}

// abr 27 abr 2025
/*
 This was captured on wireshark on a request for a write job multi write var
this was failing sometimes silently, sometimes with a too generic error
Sharp7.Rx.S7CommunicationException: Error in MultiVar request for variables: DB254.DBW4,DB254.DBW2: 0x800000, CLI: Invalid CPU answer
      at Sharp7.Rx.Sharp7Connector.EnsureSuccessOrThrow(Int32 result, String message) in C:\Users\Abel Briones\Documents\GitHub\IndTraceV2025\Src\Infrastructure\Sharp7.Rx\Sharp7Connector.cs:line 232
      at Sharp7.Rx.Sharp7Connector.ExecuteMultiVarRequest(IReadOnlyList`1 variableNames, CancellationToken token) in C:\Users\Abel Briones\Documents\GitHub\IndTraceV2025\Src\Infrastructure\Sharp7.Rx\Sharp7Connector.cs:line 108
      at Sharp7.Rx.Sharp7Plc.GetAllValues(Sharp7Connector connector) in C:\Users\Abel Briones\Documents\GitHub\IndTraceV2025\Src\Infrastructure\Sharp7.Rx\Sharp7Plc.cs:line 445
   18:11:30.643 [Error] ()
followed by error on another lecture of tags, very suspicious

below there is a frame of a write job of a single write var, a successfully one
one can see the are is 132, the value is on hex, this is the reason of this change,
after this simple change the error was not seen again, the values were successfully written on the plc,

 *"5000","19.875875","192.168.0.100","192.168.0.100","S7COMM","159","ROSCTR:[Job     ] Function:[Write Var]"
 *"5000","19.875875","192.168.0.100","192.168.0.100","S7COMM","159","ROSCTR:[Job     ] Function:[Write Var]"
 *Frame 5000: 159 bytes on wire (1272 bits), 159 bytes captured (1272 bits) on interface \Device\NPF_Loopback, id 0
   Null/Loopback
   Internet Protocol Version 4, Src: 192.168.0.100, Dst: 192.168.0.100
   Transmission Control Protocol, Src Port: 53371, Dst Port: 102, Seq: 44, Ack: 98, Len: 115
   TPKT, Version: 3, Length: 115
   ISO 8073/X.224 COTP Connection-Oriented Transport Protocol
   S7 Communication
       Header: (Job)
       Parameter: (Write Var)
           Function: Write Var (0x05)
           Item count: 3
           Item [1]: (unknown area 0x44 4.0 BYTE 16)
               Variable specification: 0x12
               Length of following address specification: 10
               Syntax Id: S7ANY (0x10)
               Transport size: BYTE (2)
               Length: 16
               DB number: 256
               Area: Unknown (0x44)
               Address: 0x000020
                   .... .000 0000 0000 0010 0... = Byte Address: 4
                   .... .... .... .... .... .000 = Bit Address: 0
           Item [2]: (unknown area 0x44 8.0 BYTE 16)
               Variable specification: 0x12
               Length of following address specification: 10
               Syntax Id: S7ANY (0x10)
               Transport size: BYTE (2)
               Length: 16
               DB number: 256
               Area: Unknown (0x44)
               Address: 0x000040
                   .... .000 0000 0000 0100 0... = Byte Address: 8
                   .... .... .... .... .... .000 = Bit Address: 0
           Item [3]: (unknown area 0x44 48.0 BYTE 16)
               Variable specification: 0x12
               Length of following address specification: 10
               Syntax Id: S7ANY (0x10)
               Transport size: BYTE (2)
               Length: 16
               DB number: 256
               Area: Unknown (0x44)
               Address: 0x000180
                   .... .000 0000 0001 1000 0... = Byte Address: 48
                   .... .... .... .... .... .000 = Bit Address: 0
       Data
           Item [1]: (Reserved)
               Return code: Reserved (0x00)
               Transport size: BYTE/WORD/DWORD (0x04)
               Length: 16
               Data: 00000064000000000000000000000000
           Item [2]: (Reserved)
               Return code: Reserved (0x00)
               Transport size: BYTE/WORD/DWORD (0x04)
               Length: 16
               Data: 00000064000000000000000000000000
           Item [3]: (Reserved)
               Return code: Reserved (0x00)
               Transport size: BYTE/WORD/DWORD (0x04)
               Length: 16
               Data: 00000011000000000000000000000000

//this was a correct captures

Frame 4995: 81 bytes on wire (648 bits), 81 bytes captured (648 bits) on interface \Device\NPF_Loopback, id 0
   Null/Loopback
   Internet Protocol Version 4, Src: 192.168.0.100, Dst: 192.168.0.100
   Transmission Control Protocol, Src Port: 53368, Dst Port: 102, Seq: 7288, Ack: 5490, Len: 37
   TPKT, Version: 3, Length: 37
   ISO 8073/X.224 COTP Connection-Oriented Transport Protocol
   S7 Communication
       Header: (Job)
       Parameter: (Write Var)
           Function: Write Var (0x05)
           Item count: 1
           Item [1]: (DB 254.DBX 6.0 BYTE 2)
               Variable specification: 0x12
               Length of following address specification: 10
               Syntax Id: S7ANY (0x10)
               Transport size: BYTE (2)
               Length: 2
               DB number: 254
               Area: Data blocks (DB) (0x84)
               Address: 0x000030
                   .... .000 0000 0000 0011 0... = Byte Address: 6
                   .... .... .... .... .... .000 = Bit Address: 0
       Data
           Item [1]: (Reserved)
               Return code: Reserved (0x00)
               Transport size: BYTE/WORD/DWORD (0x04)
               Length: 2
               Data: 0004

 */
