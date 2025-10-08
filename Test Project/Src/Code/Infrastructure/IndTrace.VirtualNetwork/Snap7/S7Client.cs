namespace IndTrace.VirtualNetwork.Snap7;

/// <summary>
/// Represents a Snap7 client for communicating with Siemens S7 PLCs.
/// </summary>
public class S7Client
{
    private const int MsgTextLen = 1024;
    // Error codes
    /// <summary>
    /// Error code for negotiating PDU.
    /// </summary>
    public static readonly uint ErrNegotiatingPdu = 0x00100000;
    /// <summary>
    /// Error code for invalid client parameters.
    /// </summary>
    public static readonly uint ErrCliInvalidParams = 0x00200000;
    /// <summary>
    /// Error code for job pending.
    /// </summary>
    public static readonly uint ErrCliJobPending = 0x00300000;
    /// <summary>
    /// Error code for too many items.
    /// </summary>
    public static readonly uint ErrCliTooManyItems = 0x00400000;
    /// <summary>
    /// Error code for invalid word length.
    /// </summary>
    public static readonly uint ErrCliInvalidWordLen = 0x00500000;
    /// <summary>
    /// Error code for partial data written.
    /// </summary>
    public static readonly uint ErrCliPartialDataWritten = 0x00600000;
    /// <summary>
    /// Error code for size over PDU.
    /// </summary>
    public static readonly uint ErrCliSizeOverPdu = 0x00700000;
    /// <summary>
    /// Error code for invalid PLC answer.
    /// </summary>
    public static readonly uint ErrCliInvalidPlcAnswer = 0x00800000;
    /// <summary>
    /// Error code for address out of range.
    /// </summary>
    public static readonly uint ErrCliAddressOutOfRange = 0x00900000;
    /// <summary>
    /// Error code for invalid transport size.
    /// </summary>
    public static readonly uint ErrCliInvalidTransportSize = 0x00A00000;
    /// <summary>
    /// Error code for write data size mismatch.
    /// </summary>
    public static readonly uint ErrCliWriteDataSizeMismatch = 0x00B00000;
    /// <summary>
    /// Error code for item not available.
    /// </summary>
    public static readonly uint ErrCliItemNotAvailable = 0x00C00000;
    /// <summary>
    /// Error code for invalid value.
    /// </summary>
    public static readonly uint ErrCliInvalidValue = 0x00D00000;
    /// <summary>
    /// Error code for cannot start PLC.
    /// </summary>
    public static readonly uint ErrCliCannotStartPlc = 0x00E00000;
    /// <summary>
    /// Error code for already run.
    /// </summary>
    public static readonly uint ErrCliAlreadyRun = 0x00F00000;
    /// <summary>
    /// Error code for cannot stop PLC.
    /// </summary>
    public static readonly uint ErrCliCannotStopPlc = 0x01000000;
    /// <summary>
    /// Error code for cannot copy RAM to ROM.
    /// </summary>
    public static readonly uint ErrCliCannotCopyRamToRom = 0x01100000;
    /// <summary>
    /// Error code for cannot compress.
    /// </summary>
    public static readonly uint ErrCliCannotCompress = 0x01200000;
    /// <summary>
    /// Error code for already stop.
    /// </summary>
    public static readonly uint ErrCliAlreadyStop = 0x01300000;
    /// <summary>
    /// Error code for function not available.
    /// </summary>
    public static readonly uint ErrCliFunNotAvailable = 0x01400000;
    /// <summary>
    /// Error code for upload sequence failed.
    /// </summary>
    public static readonly uint ErrCliUploadSequenceFailed = 0x01500000;
    /// <summary>
    /// Error code for invalid data size received.
    /// </summary>
    public static readonly uint ErrCliInvalidDataSizeRecvd = 0x01600000;
    /// <summary>
    /// Error code for invalid block type.
    /// </summary>
    public static readonly uint ErrCliInvalidBlockType = 0x01700000;
    /// <summary>
    /// Error code for invalid block number.
    /// </summary>
    public static readonly uint ErrCliInvalidBlockNumber = 0x01800000;
    /// <summary>
    /// Error code for invalid block size.
    /// </summary>
    public static readonly uint ErrCliInvalidBlockSize = 0x01900000;
    /// <summary>
    /// Error code for download sequence failed.
    /// </summary>
    public static readonly uint ErrCliDownloadSequenceFailed = 0x01A00000;
    /// <summary>
    /// Error code for insert refused.
    /// </summary>
    public static readonly uint ErrCliInsertRefused = 0x01B00000;
    /// <summary>
    /// Error code for delete refused.
    /// </summary>
    public static readonly uint ErrCliDeleteRefused = 0x01C00000;
    /// <summary>
    /// Error code for need password.
    /// </summary>
    public static readonly uint ErrCliNeedPassword = 0x01D00000;
    /// <summary>
    /// Error code for invalid password.
    /// </summary>
    public static readonly uint ErrCliInvalidPassword = 0x01E00000;
    /// <summary>
    /// Error code for no password to set or clear.
    /// </summary>
    public static readonly uint ErrCliNoPasswordToSetOrClear = 0x01F00000;
    /// <summary>
    /// Error code for job timeout.
    /// </summary>
    public static readonly uint ErrCliJobTimeout = 0x02000000;
    /// <summary>
    /// Error code for partial data read.
    /// </summary>
    public static readonly uint ErrCliPartialDataRead = 0x02100000;
    /// <summary>
    /// Error code for buffer too small.
    /// </summary>
    public static readonly uint ErrCliBufferTooSmall = 0x02200000;
    /// <summary>
    /// Error code for function refused.
    /// </summary>
    public static readonly uint ErrCliFunctionRefused = 0x02300000;
    /// <summary>
    /// Error code for destroying.
    /// </summary>
    public static readonly uint ErrCliDestroying = 0x02400000;
    /// <summary>
    /// Error code for invalid parameter number.
    /// </summary>
    public static readonly uint ErrCliInvalidParamNumber = 0x02500000;
    /// <summary>
    /// Error code for cannot change parameter.
    /// </summary>
    public static readonly uint ErrCliCannotChangeParam = 0x02600000;

    // Area ID
    /// <summary>
    /// Peripheral input area ID.
    /// </summary>
    public static readonly byte S7AreaPe = 0x81;
    /// <summary>
    /// Peripheral output area ID.
    /// </summary>
    public static readonly byte S7AreaPa = 0x82;
    /// <summary>
    /// Marker area ID.
    /// </summary>
    public static readonly byte S7AreaMk = 0x83;
    /// <summary>
    /// Data block area ID.
    /// </summary>
    public static readonly byte S7AreaDb = 0x84;
    /// <summary>
    /// Counter area ID.
    /// </summary>
    public static readonly byte S7AreaCt = 0x1C;
    /// <summary>
    /// Timer area ID.
    /// </summary>
    public static readonly byte S7AreaTm = 0x1D;

    // Word Length
    /// <summary>
    /// Word length for bit.
    /// </summary>
    public static readonly int S7WlBit = 0x01;
    /// <summary>
    /// Word length for byte.
    /// </summary>
    public static readonly int S7WlByte = 0x02;
    /// <summary>
    /// Word length for word.
    /// </summary>
    public static readonly int S7WlWord = 0x04;
    /// <summary>
    /// Word length for double word.
    /// </summary>
    public static readonly int S7WldWord = 0x06;
    /// <summary>
    /// Word length for real (float).
    /// </summary>
    public static readonly int S7WlReal = 0x08;
    /// <summary>
    /// Word length for counter.
    /// </summary>
    public static readonly int S7WlCounter = 0x1C;
    /// <summary>
    /// Word length for timer.
    /// </summary>
    public static readonly int S7WlTimer = 0x1D;

    // Block type
    /// <summary>
    /// Organization Block (OB) type.
    /// </summary>
    public static readonly byte BlockOb = 0x38;
    /// <summary>
    /// Data Block (DB) type.
    /// </summary>
    public static readonly byte BlockDb = 0x41;
    /// <summary>
    /// System Data Block (SDB) type.
    /// </summary>
    public static readonly byte BlockSdb = 0x42;
    /// <summary>
    /// Function (FC) type.
    /// </summary>
    public static readonly byte BlockFc = 0x43;
    /// <summary>
    /// System Function (SFC) type.
    /// </summary>
    public static readonly byte BlockSfc = 0x44;
    /// <summary>
    /// Function Block (FB) type.
    /// </summary>
    public static readonly byte BlockFb = 0x45;
    /// <summary>
    /// System Function Block (SFB) type.
    /// </summary>
    public static readonly byte BlockSfb = 0x46;

    // Sub Block Type
    /// <summary>
    /// Sub-block type for Organization Block (OB).
    /// </summary>
    public static readonly byte SubBlkOb = 0x08;
    /// <summary>
    /// Sub-block type for Data Block (DB).
    /// </summary>
    public static readonly byte SubBlkDb = 0x0A;
    /// <summary>
    /// Sub-block type for System Data Block (SDB).
    /// </summary>
    public static readonly byte SubBlkSdb = 0x0B;
    /// <summary>
    /// Sub-block type for Function (FC).
    /// </summary>
    public static readonly byte SubBlkFc = 0x0C;
    /// <summary>
    /// Sub-block type for System Function (SFC).
    /// </summary>
    public static readonly byte SubBlkSfc = 0x0D;
    /// <summary>
    /// Sub-block type for Function Block (FB).
    /// </summary>
    public static readonly byte SubBlkFb = 0x0E;
    /// <summary>
    /// Sub-block type for System Function Block (SFB).
    /// </summary>
    public static readonly byte SubBlkSfb = 0x0F;

    // Block languages
    /// <summary>
    /// Gets the block language type for Statement List (AWL).
    /// </summary>
    public static readonly byte BlockLangAwl = 0x01;

    /// <summary>
    /// Gets the block language type for Ladder Logic (KOP).
    /// </summary>
    public static readonly byte BlockLangKop = 0x02;

    /// <summary>
    /// Gets the block language type for Function Block Diagram (FUP).
    /// </summary>
    public static readonly byte BlockLangFup = 0x03;

    /// <summary>
    /// Gets the block language type for Structured Control Language (SCL).
    /// </summary>
    public static readonly byte BlockLangScl = 0x04;

    /// <summary>
    /// Gets the block language type for Data Block (DB).
    /// </summary>
    public static readonly byte BlockLangDb = 0x05;
    public static readonly byte BlockLangGraph = 0x06;

    /// <summary>
    /// Gets the maximum number of variables for multiread/write operations.
    /// </summary>
    public static readonly int MaxVars = 20;

    // Client Connection Type
    public static readonly ushort ConntypePg = 0x01;  // Connect to the PLC as a PG
    public static readonly ushort ConntypeOp = 0x02;  // Connect to the PLC as an OP
    public static readonly ushort ConntypeBasic = 0x03;  // Basic connection

    // Job
    private const int JobComplete = 0;
    private const int JobPending = 1;

    private IntPtr client;

    /// <summary>
    /// Represents a data item for S7 operations, including area, word length, result, DB number, start, amount, and pointer to data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S7DataItem
    {
        /// <summary>
        /// The area of the data item.
        /// </summary>
        public int Area;
        /// <summary>
        /// The word length of the data item.
        /// </summary>
        public int WordLen;
        /// <summary>
        /// The result of the operation.
        /// </summary>
        public int Result;
        /// <summary>
        /// The DB number of the data item.
        /// </summary>
        public int DBNumber;
        /// <summary>
        /// The start address of the data item.
        /// </summary>
        public int Start;
        /// <summary>
        /// The amount of data items.
        /// </summary>
        public int Amount;
        /// <summary>
        /// Pointer to the data.
        /// </summary>
        public IntPtr PData;
    }

    /// <summary>
    /// Represents a list of S7 blocks.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // <- "maybe" we don't need
    public struct S7BlocksList
    {
        /// <summary>
        /// The count of Organization Blocks (OBs).
        /// </summary>
        public int OBCount;
        /// <summary>
        /// The count of Function Blocks (FBs).
        /// </summary>
        public int FBCount;
        /// <summary>
        /// The count of Functions (FCs).
        /// </summary>
        public int FCCount;
        /// <summary>
        /// The count of System Function Blocks (SFBs).
        /// </summary>
        public int SFBCount;
        /// <summary>
        /// The count of System Functions (SFCs).
        /// </summary>
        public int SFCCount;
        /// <summary>
        /// The count of Data Blocks (DBs).
        /// </summary>
        public int DBCount;
        /// <summary>
        /// The count of System Data Blocks (SDBs).
        /// </summary>
        public int SDBCount;
    };

    // Packed Block Info
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    protected struct Us7BlockInfo
    {
        public int BlkType;
        public int BlkNumber;
        public int BlkLang;
        public int BlkFlags;
        public int MC7Size;  // The real size in bytes
        public int LoadSize;
        public int LocalData;
        public int SBBLength;
        public int CheckSum;
        public int Version;
        // Chars info
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public char[] CodeDate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public char[] IntfDate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public char[] Author;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public char[] Family;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public char[] Header;
    };

    private Us7BlockInfo uBlockInfo;

    /// <summary>
    /// Represents managed block information for S7, including type, number, language, flags, and metadata.
    /// </summary>
    public struct S7BlockInfo
    {
        /// <summary>
        /// The block type.
        /// </summary>
        public int BlkType;
        /// <summary>
        /// The block number.
        /// </summary>
        public int BlkNumber;
        /// <summary>
        /// The block language.
        /// </summary>
        public int BlkLang;
        /// <summary>
        /// The block flags.
        /// </summary>
        public int BlkFlags;
        /// <summary>
        /// The real size of the block in bytes.
        /// </summary>
        public int Mc7Size;
        /// <summary>
        /// The load size of the block.
        /// </summary>
        public int LoadSize;
        /// <summary>
        /// The local data size.
        /// </summary>
        public int LocalData;
        /// <summary>
        /// The SBB length.
        /// </summary>
        public int SbbLength;
        /// <summary>
        /// The checksum of the block.
        /// </summary>
        public int CheckSum;
        /// <summary>
        /// The version of the block.
        /// </summary>
        public int Version;
        // Chars info
        /// <summary>
        /// The code date.
        /// </summary>
        public string CodeDate;
        /// <summary>
        /// The interface date.
        /// </summary>
        public string IntfDate;
        /// <summary>
        /// The author of the block.
        /// </summary>
        public string Author;
        /// <summary>
        /// The family of the block.
        /// </summary>
        public string Family;
        /// <summary>
        /// The header of the block.
        /// </summary>
        public string Header;
    };

    /// <summary>
    /// Gets an array of S7 block types.
    /// </summary>
    public ushort[] Ts7BlocksOfType = [];

    // Packed Order Code + Version
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    protected struct Us7OrderCode
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public char[] Code;
        public byte V1;
        public byte V2;
        public byte V3;
    };

    private Us7OrderCode uOrderCode;

    /// <summary>
    /// Represents managed order code and version information for S7.
    /// </summary>
    public struct S7OrderCode
    {
        /// <summary>
        /// The order code.
        /// </summary>
        public string Code;
        /// <summary>
        /// Version byte 1.
        /// </summary>
        public byte V1;
        /// <summary>
        /// Version byte 2.
        /// </summary>
        public byte V2;
        /// <summary>
        /// Version byte 3.
        /// </summary>
        public byte V3;
    };

    // Packed CPU Info
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    protected struct Us7CpuInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
        public char[] ModuleTypeName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        public char[] SerialNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        public char[] ASName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)]
        public char[] Copyright;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        public char[] ModuleName;
    };

    private Us7CpuInfo uCpuInfo;

    /// <summary>
    /// Represents managed CPU information for S7, including module type, serial number, and other metadata.
    /// </summary>
    public struct S7CpuInfo
    {
        /// <summary>
        /// The module type name.
        /// </summary>
        public string ModuleTypeName;
        /// <summary>
        /// The serial number.
        /// </summary>
        public string SerialNumber;
        /// <summary>
        /// The AS name.
        /// </summary>
        public string AsName;
        /// <summary>
        /// The copyright information.
        /// </summary>
        public string Copyright;
        /// <summary>
        /// The module name.
        /// </summary>
        public string ModuleName;
    }

    /// <summary>
    /// Represents CP (communication processor) information for S7, including PDU length, connections, and rates.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S7CpInfo
    {
        /// <summary>
        /// Maximum PDU length.
        /// </summary>
        public int MaxPduLengt;
        /// <summary>
        /// Maximum number of connections.
        /// </summary>
        public int MaxConnections;
        /// <summary>
        /// Maximum MPI rate.
        /// </summary>
        public int MaxMpiRate;
        /// <summary>
        /// Maximum bus rate.
        /// </summary>
        public int MaxBusRate;
    };

    /// <summary>
    /// Represents the header of a SZL (System-Zustands-Liste) structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SzlHeader
    {
        /// <summary>
        /// Length of the header.
        /// </summary>
        public ushort LENTHDR;
        /// <summary>
        /// Number of records.
        /// </summary>
        public ushort NDR;
    };

    /// <summary>
    /// Represents a SZL (System-Zustands-Liste) structure for S7.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S7Szl
    {
        /// <summary>
        /// The SZL header.
        /// </summary>
        public SzlHeader Header;
        /// <summary>
        /// The data bytes.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4000 - 4)]
        public byte[] Data;
    };

    /// <summary>
    /// Represents a list of SZL structures for S7.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S7SzlList
    {
        /// <summary>
        /// The SZL header.
        /// </summary>
        public SzlHeader Header;
        /// <summary>
        /// The data items.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x2000 - 2)]
        public ushort[] Data;
    };

    /// <summary>
    /// Represents S7 protection settings, including various protection flags.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S7Protection  // Packed S7Protection
    {
        /// <summary>
        /// Protection level 1.
        /// </summary>
        public ushort SchSchal;
        /// <summary>
        /// Protection level 2.
        /// </summary>
        public ushort SchPar;
        /// <summary>
        /// Protection level 3.
        /// </summary>
        public ushort SchRel;
        /// <summary>
        /// Protection level 4.
        /// </summary>
        public ushort BartSch;
        /// <summary>
        /// Protection level 5.
        /// </summary>
        public ushort AnlSch;
    };

    // C++ time struct, functions to convert it from/to DateTime are provided ;-)
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    protected struct CppTm
    {
        public int TmSec;
        public int TmMin;
        public int TmHour;
        public int TmMday;
        public int TmMon;
        public int TmYear;
        public int TmWday;
        public int TmYday;
        public int TmIsdst;
    }

    private CppTm tm;

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern IntPtr Cli_Create();

    /// <summary>
    /// Initializes a new instance of the <see cref="S7Client"/> class.
    /// </summary>
    public S7Client()
    {
        this.client = Cli_Create();
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_Destroy(ref IntPtr client);

    /// <summary>
    /// Finalizes an instance of the <see cref="S7Client"/> class.
    /// </summary>
    ~S7Client()
    {
        Cli_Destroy(ref this.client);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_Connect(IntPtr client);
    /// <summary>
    /// Connects to the PLC.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Connect()
    {
        return Cli_Connect(this.client);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ConnectTo(
        IntPtr client,
        [MarshalAs(UnmanagedType.LPStr)] string address,
        int rack,
        int slot
    );

    /// <summary>
    /// Connects to the PLC at a specified address, rack, and slot.
    /// </summary>
    /// <param name="address">The IP address or hostname of the PLC.</param>
    /// <param name="rack">The rack number of the PLC.</param>
    /// <param name="slot">The slot number of the CPU.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ConnectTo(string address, int rack, int slot)
    {
        return Cli_ConnectTo(this.client, address, rack, slot);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_SetConnectionParams(
        IntPtr client,
        [MarshalAs(UnmanagedType.LPStr)] string address,
        ushort localTsap,
        ushort remoteTsap
    );

    /// <summary>
    /// Sets the connection parameters for the PLC.
    /// </summary>
    /// <param name="address">The IP address or hostname of the PLC.</param>
    /// <param name="localTsap">The local TSAP (Transport Service Access Point).</param>
    /// <param name="remoteTsap">The remote TSAP.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetConnectionParams(string address, ushort localTsap, ushort remoteTsap)
    {
        return Cli_SetConnectionParams(this.client, address, localTsap, remoteTsap);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_SetConnectionType(IntPtr client, ushort connectionType);
    /// <summary>
    /// Sets the connection type for the PLC.
    /// </summary>
    /// <param name="connectionType">The connection type (e.g., <see cref="ConntypePg"/>, <see cref="ConntypeOp"/>, <see cref="ConntypeBasic"/>).</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetConnectionType(ushort connectionType)
    {
        return Cli_SetConnectionType(this.client, connectionType);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_Disconnect(IntPtr client);
    /// <summary>
    /// Disconnects from the PLC.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Disconnect()
    {
        return Cli_Disconnect(this.client);
    }

    // Get/SetParam needs a void* parameter, internally it decides the kind of pointer
    // in accord to ParamNumber.
    // To avoid the use of unsafe code we split the DLL functions and use overloaded methods.

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_GetParam")]
    protected static extern int Cli_GetParam_i16(IntPtr client, int paramNumber, ref short intValue);
    /// <summary>
    /// Gets a 16-bit integer parameter from the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref short intValue)
    {
        return Cli_GetParam_i16(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_GetParam")]
    protected static extern int Cli_GetParam_u16(IntPtr client, int paramNumber, ref ushort intValue);
    /// <summary>
    /// Gets an unsigned 16-bit integer parameter from the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref ushort intValue)
    {
        return Cli_GetParam_u16(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_GetParam")]
    protected static extern int Cli_GetParam_i32(IntPtr client, int paramNumber, ref int intValue);
    /// <summary>
    /// Gets a 32-bit integer parameter from the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref int intValue)
    {
        return Cli_GetParam_i32(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_GetParam")]
    protected static extern int Cli_GetParam_u32(IntPtr client, int paramNumber, ref uint intValue);
    /// <summary>
    /// Gets an unsigned 32-bit integer parameter from the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref uint intValue)
    {
        return Cli_GetParam_u32(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_GetParam")]
    protected static extern int Cli_GetParam_i64(IntPtr client, int paramNumber, ref long intValue);
    /// <summary>
    /// Gets a 64-bit integer parameter from the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref long intValue)
    {
        return Cli_GetParam_i64(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_GetParam")]
    protected static extern int Cli_GetParam_u64(IntPtr client, int paramNumber, ref ulong intValue);
    /// <summary>
    /// Gets an unsigned 64-bit integer parameter from the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref ulong intValue)
    {
        return Cli_GetParam_u64(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_SetParam")]
    protected static extern int Cli_SetParam_i16(IntPtr client, int paramNumber, ref short intValue);
    /// <summary>
    /// Sets a 16-bit integer parameter in the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set the parameter to.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref short intValue)
    {
        return Cli_SetParam_i16(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_SetParam")]
    protected static extern int Cli_SetParam_u16(IntPtr client, int paramNumber, ref ushort intValue);
    /// <summary>
    /// Sets an unsigned 16-bit integer parameter in the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set the parameter to.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref ushort intValue)
    {
        return Cli_SetParam_u16(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_SetParam")]
    protected static extern int Cli_SetParam_i32(IntPtr client, int paramNumber, ref int intValue);
    /// <summary>
    /// Sets a 32-bit integer parameter in the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set the parameter to.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref int intValue)
    {
        return Cli_SetParam_i32(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_SetParam")]
    protected static extern int Cli_SetParam_u32(IntPtr client, int paramNumber, ref uint intValue);
    /// <summary>
    /// Sets an unsigned 32-bit integer parameter in the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set the parameter to.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref uint intValue)
    {
        return Cli_SetParam_u32(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_SetParam")]
    protected static extern int Cli_SetParam_i64(IntPtr client, int paramNumber, ref long intValue);
    /// <summary>
    /// Sets a 64-bit integer parameter in the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set the parameter to.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref long intValue)
    {
        return Cli_SetParam_i64(this.client, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_SetParam")]
    protected static extern int Cli_SetParam_u64(IntPtr client, int paramNumber, ref ulong intValue);
    /// <summary>
    /// Sets an unsigned 64-bit integer parameter in the PLC.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set the parameter to.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref ulong intValue)
    {
        return Cli_SetParam_u64(this.client, paramNumber, ref intValue);
    }

    /// <summary>
    /// Represents the callback for S7 client completion.
    /// </summary>
    /// <param name="usrPtr">User-defined pointer.</param>
    /// <param name="opCode">Operation code.</param>
    /// <param name="opResult">Operation result.</param>
    public delegate void S7CliCompletion(IntPtr usrPtr, int opCode, int opResult);

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_SetAsCallback(IntPtr client, S7CliCompletion completion, IntPtr usrPtr);
    /// <summary>
    /// Sets the asynchronous callback function for client operations.
    /// </summary>
    /// <param name="completion">The callback function.</param>
    /// <param name="usrPtr">User-defined pointer to be passed to the callback.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetAsCallBack(S7CliCompletion completion, IntPtr usrPtr)
    {
        return Cli_SetAsCallback(this.client, completion, usrPtr);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ReadArea(IntPtr client, int area, int dbNumber, int start, int amount, int wordLen, byte[] buffer);
    /// <summary>
    /// Reads data from a specified area of the PLC.
    /// </summary>
    /// <param name="area">The memory area to read from (e.g., <see cref="S7AreaDb"/>, <see cref="S7AreaMk"/>).</param>
    /// <param name="dbNumber">The data block number (if applicable, 0 otherwise).</param>
    /// <param name="start">The starting address in the area.</param>
    /// <param name="amount">The number of items to read.</param>
    /// <param name="wordLen">The word length of the items (e.g., <see cref="S7WlByte"/>, <see cref="S7WlWord"/>).</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ReadArea(int area, int dbNumber, int start, int amount, int wordLen, byte[] buffer)
    {
        return Cli_ReadArea(this.client, area, dbNumber, start, amount, wordLen, buffer);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Cli_ReadArea")]
    protected static extern int Cli_ReadArea_ptr(IntPtr client, int area, int dbNumber, int start, int amount, int wordLen, IntPtr pointer);
    /// <summary>
    /// Reads data from a specified area of the PLC using a pointer to the buffer.
    /// </summary>
    /// <param name="area">The memory area to read from.</param>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="start">The starting address.</param>
    /// <param name="amount">The number of items to read.</param>
    /// <param name="wordLen">The word length.</param>
    /// <param name="pointer">A pointer to the buffer to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ReadArea(int area, int dbNumber, int start, int amount, int wordLen, IntPtr pointer)
    {
        return Cli_ReadArea_ptr(this.client, area, dbNumber, start, amount, wordLen, pointer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_WriteArea(IntPtr client, int area, int dbNumber, int start, int amount, int wordLen, byte[] buffer);
    /// <summary>
    /// Writes data to a specified area of the PLC.
    /// </summary>
    /// <param name="area">The memory area to write to.</param>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="start">The starting address.</param>
    /// <param name="amount">The number of items to write.</param>
    /// <param name="wordLen">The word length.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int WriteArea(int area, int dbNumber, int start, int amount, int wordLen, byte[] buffer)
    {
        return Cli_WriteArea(this.client, area, dbNumber, start, amount, wordLen, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ReadMultiVars(IntPtr client, ref S7DataItem item, int itemsCount);
    /// <summary>
    /// Reads multiple variables from the PLC in a single request.
    /// </summary>
    /// <param name="items">An array of <see cref="S7DataItem"/> structures describing the variables to read.</param>
    /// <param name="itemsCount">The number of items in the <paramref name="items"/> array.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ReadMultiVars(S7DataItem[] items, int itemsCount)
    {
        return Cli_ReadMultiVars(this.client, ref items[0], itemsCount);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_WriteMultiVars(IntPtr client, ref S7DataItem item, int itemsCount);
    /// <summary>
    /// Writes multiple variables to the PLC in a single request.
    /// </summary>
    /// <param name="items">An array of <see cref="S7DataItem"/> structures describing the variables to write.</param>
    /// <param name="itemsCount">The number of items in the <paramref name="items"/> array.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int WriteMultiVars(S7DataItem[] items, int itemsCount)
    {
        return Cli_WriteMultiVars(this.client, ref items[0], itemsCount);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_DBRead(IntPtr client, int dbNumber, int start, int size, byte[] buffer);
    /// <summary>
    /// Reads data from a specific data block (DB) in the PLC.
    /// </summary>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="start">The starting byte address within the data block.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int DbRead(int dbNumber, int start, int size, byte[] buffer)
    {
        return Cli_DBRead(this.client, dbNumber, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_DBWrite(IntPtr client, int dbNumber, int start, int size, byte[] buffer);
    /// <summary>
    /// Writes data to a specific data block (DB) in the PLC.
    /// </summary>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="start">The starting byte address within the data block.</param>
    /// <param name="size">The number of bytes to write.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int DbWrite(int dbNumber, int start, int size, byte[] buffer)
    {
        return Cli_DBWrite(this.client, dbNumber, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_MBRead(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Reads data from the marker (M) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the marker area.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int MbRead(int start, int size, byte[] buffer)
    {
        return Cli_MBRead(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_MBWrite(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Writes data to the marker (M) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the marker area.</param>
    /// <param name="size">The number of bytes to write.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int MbWrite(int start, int size, byte[] buffer)
    {
        return Cli_MBWrite(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_EBRead(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Reads data from the input (E) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the input area.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int EbRead(int start, int size, byte[] buffer)
    {
        return Cli_EBRead(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_EBWrite(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Writes data to the input (E) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the input area.</param>
    /// <param name="size">The number of bytes to write.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int EbWrite(int start, int size, byte[] buffer)
    {
        return Cli_EBWrite(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ABRead(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Reads data from the output (A) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the output area.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AbRead(int start, int size, byte[] buffer)
    {
        return Cli_ABRead(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ABWrite(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Writes data to the output (A) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the output area.</param>
    /// <param name="size">The number of bytes to write.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AbWrite(int start, int size, byte[] buffer)
    {
        return Cli_ABWrite(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_TMRead(IntPtr client, int start, int amount, ushort[] buffer);
    /// <summary>
    /// Reads timer (T) values from the PLC.
    /// </summary>
    /// <param name="start">The starting timer number.</param>
    /// <param name="amount">The number of timers to read.</param>
    /// <param name="buffer">The ushort array to store the read timer values.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int TmRead(int start, int amount, ushort[] buffer)
    {
        return Cli_TMRead(this.client, start, amount, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_TMWrite(IntPtr client, int start, int amount, ushort[] buffer);
    /// <summary>
    /// Writes timer (T) values to the PLC.
    /// </summary>
    /// <param name="start">The starting timer number.</param>
    /// <param name="amount">The number of timers to write.</param>
    /// <param name="buffer">The ushort array containing the timer values to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int TmWrite(int start, int amount, ushort[] buffer)
    {
        return Cli_TMWrite(this.client, start, amount, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_CTRead(IntPtr client, int start, int amount, ushort[] buffer);
    /// <summary>
    /// Reads counter (C) values from the PLC.
    /// </summary>
    /// <param name="start">The starting counter number.</param>
    /// <param name="amount">The number of counters to read.</param>
    /// <param name="buffer">The ushort array to store the read counter values.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int CtRead(int start, int amount, ushort[] buffer)
    {
        return Cli_CTRead(this.client, start, amount, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_CTWrite(IntPtr client, int start, int amount, ushort[] buffer);
    /// <summary>
    /// Writes counter (C) values to the PLC.
    /// </summary>
    /// <param name="start">The starting counter number.</param>
    /// <param name="amount">The number of counters to write.</param>
    /// <param name="buffer">The ushort array containing the counter values to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int CtWrite(int start, int amount, ushort[] buffer)
    {
        return Cli_CTWrite(this.client, start, amount, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ListBlocks(IntPtr client, ref S7BlocksList list);
    /// <summary>
    /// Lists all blocks in the PLC.
    /// </summary>
    /// <param name="list">A <see cref="S7BlocksList"/> structure to populate with block counts.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ListBlocks(ref S7BlocksList list)
    {
        return Cli_ListBlocks(this.client, ref list);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetAgBlockInfo(IntPtr client, int blockType, int blockNum, ref Us7BlockInfo info);
    /// <summary>
    /// Gets information about a specific block from the PLC (AG block info).
    /// </summary>
    /// <param name="blockType">The type of the block (e.g., <see cref="BlockDb"/>, <see cref="BlockOb"/>).</param>
    /// <param name="blockNum">The number of the block.</param>
    /// <param name="info">A <see cref="S7BlockInfo"/> structure to populate with block information.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetAgBlockInfo(int blockType, int blockNum, ref S7BlockInfo info)
    {
        int res = Cli_GetAgBlockInfo(this.client, blockType, blockNum, ref this.uBlockInfo);
        // Packed->Managed
        if (res == 0)
        {
            info.BlkType = this.uBlockInfo.BlkType;
            info.BlkNumber = this.uBlockInfo.BlkNumber;
            info.BlkLang = this.uBlockInfo.BlkLang;
            info.BlkFlags = this.uBlockInfo.BlkFlags;
            info.Mc7Size = this.uBlockInfo.MC7Size;
            info.LoadSize = this.uBlockInfo.LoadSize;
            info.LocalData = this.uBlockInfo.LocalData;
            info.SbbLength = this.uBlockInfo.SBBLength;
            info.CheckSum = this.uBlockInfo.CheckSum;
            info.Version = this.uBlockInfo.Version;
            // Chars info
            info.CodeDate = new string(this.uBlockInfo.CodeDate);
            info.IntfDate = new string(this.uBlockInfo.IntfDate);
            info.Author = new string(this.uBlockInfo.Author);
            info.Family = new string(this.uBlockInfo.Family);
            info.Header = new string(this.uBlockInfo.Header);
        }
        return res;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetPgBlockInfo(IntPtr client, ref Us7BlockInfo info, byte[] buffer, int size);
    /// <summary>
    /// Gets information about a specific block from the PLC (PG block info).
    /// </summary>
    /// <param name="info">A <see cref="S7BlockInfo"/> structure to populate with block information.</param>
    /// <param name="buffer">The byte array containing the block data.</param>
    /// <param name="size">The size of the block data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetPgBlockInfo(ref S7BlockInfo info, byte[] buffer, int size)
    {
        int res = Cli_GetPgBlockInfo(this.client, ref this.uBlockInfo, buffer, size);
        // Packed->Managed
        if (res == 0)
        {
            info.BlkType = this.uBlockInfo.BlkType;
            info.BlkNumber = this.uBlockInfo.BlkNumber;
            info.BlkLang = this.uBlockInfo.BlkLang;
            info.BlkFlags = this.uBlockInfo.BlkFlags;
            info.Mc7Size = this.uBlockInfo.MC7Size;
            info.LoadSize = this.uBlockInfo.LoadSize;
            info.LocalData = this.uBlockInfo.LocalData;
            info.SbbLength = this.uBlockInfo.SBBLength;
            info.CheckSum = this.uBlockInfo.CheckSum;
            info.Version = this.uBlockInfo.Version;
            // Chars info
            info.CodeDate = new string(this.uBlockInfo.CodeDate);
            info.IntfDate = new string(this.uBlockInfo.IntfDate);
            info.Author = new string(this.uBlockInfo.Author);
            info.Family = new string(this.uBlockInfo.Family);
            info.Header = new string(this.uBlockInfo.Header);
        }
        return res;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ListBlocksOfType(IntPtr client, int blockType, ushort[] list, ref int itemsCount);
    /// <summary>
    /// Lists blocks of a specific type in the PLC.
    /// </summary>
    /// <param name="blockType">The type of blocks to list.</param>
    /// <param name="list">The ushort array to populate with block numbers.</param>
    /// <param name="itemsCount">The number of items found.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ListBlocksOfType(int blockType, ushort[] list, ref int itemsCount)
    {
        return Cli_ListBlocksOfType(this.client, blockType, list, ref itemsCount);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_Upload(IntPtr client, int blockType, int blockNum, byte[] usrData, ref int size);
    /// <summary>
    /// Uploads a block from the PLC.
    /// </summary>
    /// <param name="blockType">The type of the block.</param>
    /// <param name="blockNum">The number of the block.</param>
    /// <param name="usrData">The byte array to store the uploaded data.</param>
    /// <param name="size">The size of the uploaded data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Upload(int blockType, int blockNum, byte[] usrData, ref int size)
    {
        return Cli_Upload(this.client, blockType, blockNum, usrData, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_FullUpload(IntPtr client, int blockType, int blockNum, byte[] usrData, ref int size);
    /// <summary>
    /// Performs a full upload of a block from the PLC.
    /// </summary>
    /// <param name="blockType">The type of the block.</param>
    /// <param name="blockNum">The number of the block.</param>
    /// <param name="usrData">The byte array to store the uploaded data.</param>
    /// <param name="size">The size of the uploaded data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int FullUpload(int blockType, int blockNum, byte[] usrData, ref int size)
    {
        return Cli_FullUpload(this.client, blockType, blockNum, usrData, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_Download(IntPtr client, int blockNum, byte[] usrData, int size);
    /// <summary>
    /// Downloads a block to the PLC.
    /// </summary>
    /// <param name="blockNum">The number of the block.</param>
    /// <param name="usrData">The byte array containing the data to download.</param>
    /// <param name="size">The size of the data to download.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Download(int blockNum, byte[] usrData, int size)
    {
        return Cli_Download(this.client, blockNum, usrData, size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_Delete(IntPtr client, int blockType, int blockNum);
    /// <summary>
    /// Deletes a block from the PLC.
    /// </summary>
    /// <param name="blockType">The type of the block.</param>
    /// <param name="blockNum">The number of the block.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Delete(int blockType, int blockNum)
    {
        return Cli_Delete(this.client, blockType, blockNum);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_DBGet(IntPtr client, int dbNumber, byte[] usrData, ref int size);
    /// <summary>
    /// Gets a data block (DB) from the PLC.
    /// </summary>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="usrData">The byte array to store the data block.</param>
    /// <param name="size">The size of the data block.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int DbGet(int dbNumber, byte[] usrData, ref int size)
    {
        return Cli_DBGet(this.client, dbNumber, usrData, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_DBFill(IntPtr client, int dbNumber, int fillChar);
    /// <summary>
    /// Fills a data block (DB) in the PLC with a specified character.
    /// </summary>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="fillChar">The character to fill the data block with.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int DbFill(int dbNumber, int fillChar)
    {
        return Cli_DBFill(this.client, dbNumber, fillChar);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetPlcDateTime(IntPtr client, ref CppTm tm);
    /// <summary>
    /// Gets the PLC's date and time.
    /// </summary>
    /// <param name="dt">The output <see cref="DateTime"/> variable to store the PLC's date and time.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetPlcDateTime(ref DateTime dt)
    {
        int res = Cli_GetPlcDateTime(this.client, ref this.tm);
        if (res == 0)
        {
            // Packed->Managed
            DateTime plcDt = new DateTime(this.tm.TmYear + 1900, this.tm.TmMon + 1, this.tm.TmMday, this.tm.TmHour, this.tm.TmMin, this.tm.TmSec);
            dt = plcDt;
        }
        return res;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_SetPlcDateTime(IntPtr client, ref CppTm tm);
    /// <summary>
    /// Sets the PLC's date and time.
    /// </summary>
    /// <param name="dt">The <see cref="DateTime"/> value to set the PLC's date and time to.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetPlcDateTime(DateTime dt)
    {

        // Managed->Packed
        this.tm.TmYear = dt.Year - 1900;
        this.tm.TmMon = dt.Month - 1;
        this.tm.TmMday = dt.Day;
        this.tm.TmHour = dt.Hour;
        this.tm.TmMin = dt.Minute;
        this.tm.TmSec = dt.Second;

        return Cli_SetPlcDateTime(this.client, ref this.tm);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_SetPlcSystemDateTime(IntPtr client);
    /// <summary>
    /// Sets the PLC's date and time to the system's current date and time.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetPlcSystemDateTime()
    {
        return Cli_SetPlcSystemDateTime(this.client);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetOrderCode(IntPtr client, ref Us7OrderCode info);
    /// <summary>
    /// Gets the PLC's order code.
    /// </summary>
    /// <param name="info">A <see cref="S7OrderCode"/> structure to populate with the order code information.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetOrderCode(ref S7OrderCode info)
    {
        int res = Cli_GetOrderCode(this.client, ref this.uOrderCode);
        // Packed->Managed
        if (res == 0)
        {
            info.Code = new string(this.uOrderCode.Code);
            info.V1 = this.uOrderCode.V1;
            info.V2 = this.uOrderCode.V2;
            info.V3 = this.uOrderCode.V3;
        }
        return res;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetCpuInfo(IntPtr client, ref Us7CpuInfo info);
    /// <summary>
    /// Gets information about the PLC's CPU.
    /// </summary>
    /// <param name="info">A <see cref="S7CpuInfo"/> structure to populate with CPU information.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetCpuInfo(ref S7CpuInfo info)
    {
        int res = Cli_GetCpuInfo(this.client, ref this.uCpuInfo);
        // Packed->Managed
        if (res == 0)
        {
            info.ModuleTypeName = new string(this.uCpuInfo.ModuleTypeName);
            info.SerialNumber = new string(this.uCpuInfo.SerialNumber);
            info.AsName = new string(this.uCpuInfo.ASName);
            info.Copyright = new string(this.uCpuInfo.Copyright);
            info.ModuleName = new string(this.uCpuInfo.ModuleName);
        }
        return res;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetCpInfo(IntPtr client, ref S7CpInfo info);

    /// <summary>
    /// Gets information about the PLC's Communication Processor (CP).
    /// </summary>
    /// <param name="info">A <see cref="S7CpInfo"/> structure to populate with CP information.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetCpInfo(ref S7CpInfo info)
    {
        return Cli_GetCpInfo(this.client, ref info);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ReadSZL(IntPtr client, int id, int index, ref S7Szl data, ref int size);
    /// <summary>
    /// Reads SZL (System State List) data from the PLC.
    /// </summary>
    /// <param name="id">The SZL ID.</param>
    /// <param name="index">The index within the SZL.</param>
    /// <param name="data">A <see cref="S7Szl"/> structure to populate with the read data.</param>
    /// <param name="size">The size of the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ReadSzl(int id, int index, ref S7Szl data, ref int size)
    {
        return Cli_ReadSZL(this.client, id, index, ref data, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ReadSZLList(IntPtr client, ref S7SzlList list, ref int itemsCount);
    /// <summary>
    /// Reads a list of SZL (System State List) items from the PLC.
    /// </summary>
    /// <param name="list">A <see cref="S7SzlList"/> structure to populate with the list of SZL items.</param>
    /// <param name="itemsCount">The number of items in the list.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ReadSzlList(ref S7SzlList list, ref int itemsCount)
    {
        return Cli_ReadSZLList(this.client, ref list, ref itemsCount);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_PlcHotStart(IntPtr client);
    /// <summary>
    /// Performs a hot start on the PLC.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int PlcHotStart()
    {
        return Cli_PlcHotStart(this.client);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_PlcColdStart(IntPtr client);
    /// <summary>
    /// Performs a cold start on the PLC.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int PlcColdStart()
    {
        return Cli_PlcColdStart(this.client);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_PlcStop(IntPtr client);
    /// <summary>
    /// Stops the PLC.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int PlcStop()
    {
        return Cli_PlcStop(this.client);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_CopyRamToRom(IntPtr client, uint timeout);
    /// <summary>
    /// Copies data from RAM to ROM in the PLC.
    /// </summary>
    /// <param name="timeout">The timeout for the operation in milliseconds.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int PlcCopyRamToRom(uint timeout)
    {
        return Cli_CopyRamToRom(this.client, timeout);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_Compress(IntPtr client, uint timeout);
    /// <summary>
    /// Compresses the memory in the PLC.
    /// </summary>
    /// <param name="timeout">The timeout for the operation in milliseconds.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int PlcCompress(uint timeout)
    {
        return Cli_Compress(this.client, timeout);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetPlcStatus(IntPtr client, ref int status);
    /// <summary>
    /// Gets the current status of the PLC.
    /// </summary>
    /// <param name="status">The output variable to store the PLC status.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int PlcGetStatus(ref int status)
    {
        return Cli_GetPlcStatus(this.client, ref status);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetProtection(IntPtr client, ref S7Protection protection);
    /// <summary>
    /// Gets the protection settings of the PLC.
    /// </summary>
    /// <param name="protection">A <see cref="S7Protection"/> structure to populate with protection information.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetProtection(ref S7Protection protection)
    {
        return Cli_GetProtection(this.client, ref protection);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_SetSessionPassword(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string password);
    /// <summary>
    /// Sets the session password for the PLC.
    /// </summary>
    /// <param name="password">The password to set.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetSessionPassword(string password)
    {
        return Cli_SetSessionPassword(this.client, password);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_ClearSessionPassword(IntPtr client);
    /// <summary>
    /// Clears the session password for the PLC.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int ClearSessionPassword()
    {
        return Cli_ClearSessionPassword(this.client);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_IsoExchangeBuffer(IntPtr client, byte[] buffer, ref int size);
    /// <summary>
    /// Exchanges data with the PLC using the ISO protocol.
    /// </summary>
    /// <param name="buffer">The byte array for data exchange.</param>
    /// <param name="size">The size of the data exchanged.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int IsoExchangeBuffer(byte[] buffer, ref int size)
    {
        return Cli_IsoExchangeBuffer(this.client, buffer, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsReadArea(IntPtr client, int area, int dbNumber, int start, int amount, int wordLen, byte[] buffer);
    /// <summary>
    /// Asynchronously reads data from a specified area of the PLC.
    /// </summary>
    /// <param name="area">The memory area to read from.</param>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="start">The starting address.</param>
    /// <param name="amount">The number of items to read.</param>
    /// <param name="wordLen">The word length.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsReadArea(int area, int dbNumber, int start, int amount, int wordLen, byte[] buffer)
    {
        return Cli_AsReadArea(this.client, area, dbNumber, start, amount, wordLen, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsWriteArea(IntPtr client, int area, int dbNumber, int start, int amount, int wordLen, byte[] buffer);
    /// <summary>
    /// Asynchronously writes data to a specified area of the PLC.
    /// </summary>
    /// <param name="area">The memory area to write to.</param>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="start">The starting address.</param>
    /// <param name="amount">The number of items to write.</param>
    /// <param name="wordLen">The word length.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsWriteArea(int area, int dbNumber, int start, int amount, int wordLen, byte[] buffer)
    {
        return Cli_AsWriteArea(this.client, area, dbNumber, start, amount, wordLen, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsDBRead(IntPtr client, int dbNumber, int start, int size, byte[] buffer);
    /// <summary>
    /// Asynchronously reads data from a specific data block (DB) in the PLC.
    /// </summary>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="start">The starting byte address within the data block.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsDbRead(int dbNumber, int start, int size, byte[] buffer)
    {
        return Cli_AsDBRead(this.client, dbNumber, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsDBWrite(IntPtr client, int dbNumber, int start, int size, byte[] buffer);
    /// <summary>
    /// Asynchronously writes data to a specific data block (DB) in the PLC.
    /// </summary>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="start">The starting byte address within the data block.</param>
    /// <param name="size">The number of bytes to write.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsDbWrite(int dbNumber, int start, int size, byte[] buffer)
    {
        return Cli_AsDBWrite(this.client, dbNumber, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsMBRead(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Asynchronously reads data from the marker (M) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the marker area.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsMbRead(int start, int size, byte[] buffer)
    {
        return Cli_AsMBRead(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsMBWrite(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Asynchronously writes data to the marker (M) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the marker area.</param>
    /// <param name="size">The number of bytes to write.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsMbWrite(int start, int size, byte[] buffer)
    {
        return Cli_AsMBWrite(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsEBRead(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Asynchronously reads data from the input (E) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the input area.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsEbRead(int start, int size, byte[] buffer)
    {
        return Cli_AsEBRead(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsEBWrite(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Asynchronously writes data to the input (E) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the input area.</param>
    /// <param name="size">The number of bytes to write.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsEbWrite(int start, int size, byte[] buffer)
    {
        return Cli_AsEBWrite(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsABRead(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Asynchronously reads data from the output (A) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the output area.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <param name="buffer">The byte array to store the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsAbRead(int start, int size, byte[] buffer)
    {
        return Cli_AsABRead(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsABWrite(IntPtr client, int start, int size, byte[] buffer);
    /// <summary>
    /// Asynchronously writes data to the output (A) area of the PLC.
    /// </summary>
    /// <param name="start">The starting byte address in the output area.</param>
    /// <param name="size">The number of bytes to write.</param>
    /// <param name="buffer">The byte array containing the data to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsAbWrite(int start, int size, byte[] buffer)
    {
        return Cli_AsABWrite(this.client, start, size, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsTMRead(IntPtr client, int start, int amount, ushort[] buffer);
    /// <summary>
    /// Asynchronously reads timer (T) values from the PLC.
    /// </summary>
    /// <param name="start">The starting timer number.</param>
    /// <param name="amount">The number of timers to read.</param>
    /// <param name="buffer">The ushort array to store the read timer values.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsTmRead(int start, int amount, ushort[] buffer)
    {
        return Cli_AsTMRead(this.client, start, amount, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsTMWrite(IntPtr client, int start, int amount, ushort[] buffer);
    /// <summary>
    /// Asynchronously writes timer (T) values to the PLC.
    /// </summary>
    /// <param name="start">The starting timer number.</param>
    /// <param name="amount">The number of timers to write.</param>
    /// <param name="buffer">The ushort array containing the timer values to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsTmWrite(int start, int amount, ushort[] buffer)
    {
        return Cli_AsTMWrite(this.client, start, amount, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsCTRead(IntPtr client, int start, int amount, ushort[] buffer);
    /// <summary>
    /// Asynchronously reads counter (C) values from the PLC.
    /// </summary>
    /// <param name="start">The starting counter number.</param>
    /// <param name="amount">The number of counters to read.</param>
    /// <param name="buffer">The ushort array to store the read counter values.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsCtRead(int start, int amount, ushort[] buffer)
    {
        return Cli_AsCTRead(this.client, start, amount, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsCTWrite(IntPtr client, int start, int amount, ushort[] buffer);
    /// <summary>
    /// Asynchronously writes counter (C) values to the PLC.
    /// </summary>
    /// <param name="start">The starting counter number.</param>
    /// <param name="amount">The number of counters to write.</param>
    /// <param name="buffer">The ushort array containing the counter values to write.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsCtWrite(int start, int amount, ushort[] buffer)
    {
        return Cli_AsCTWrite(this.client, start, amount, buffer);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsListBlocksOfType(IntPtr client, int blockType, ushort[] list);
    /// <summary>
    /// Asynchronously lists blocks of a specific type in the PLC.
    /// </summary>
    /// <param name="blockType">The type of blocks to list.</param>
    /// <param name="list">The ushort array to populate with block numbers.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsListBlocksOfType(int blockType, ushort[] list)
    {
        return Cli_AsListBlocksOfType(this.client, blockType, list);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsReadSZL(IntPtr client, int id, int index, ref S7Szl data, ref int size);
    /// <summary>
    /// Asynchronously reads SZL (System State List) data from the PLC.
    /// </summary>
    /// <param name="id">The SZL ID.</param>
    /// <param name="index">The index within the SZL.</param>
    /// <param name="data">A <see cref="S7Szl"/> structure to populate with the read data.</param>
    /// <param name="size">The size of the read data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsReadSzl(int id, int index, ref S7Szl data, ref int size)
    {
        return Cli_AsReadSZL(this.client, id, index, ref data, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsReadSZLList(IntPtr client, ref S7SzlList list, ref int itemsCount);
    /// <summary>
    /// Asynchronously reads a list of SZL (System State List) items from the PLC.
    /// </summary>
    /// <param name="list">A <see cref="S7SzlList"/> structure to populate with the list of SZL items.</param>
    /// <param name="itemsCount">The number of items in the list.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsReadSzlList(ref S7SzlList list, ref int itemsCount)
    {
        return Cli_AsReadSZLList(this.client, ref list, ref itemsCount);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsUpload(IntPtr client, int blockType, int blockNum, byte[] usrData, ref int size);
    /// <summary>
    /// Asynchronously uploads a block from the PLC.
    /// </summary>
    /// <param name="blockType">The type of the block.</param>
    /// <param name="blockNum">The number of the block.</param>
    /// <param name="usrData">The byte array to store the uploaded data.</param>
    /// <param name="size">The size of the uploaded data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsUpload(int blockType, int blockNum, byte[] usrData, ref int size)
    {
        return Cli_AsUpload(this.client, blockType, blockNum, usrData, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsFullUpload(IntPtr client, int blockType, int blockNum, byte[] usrData, ref int size);
    /// <summary>
    /// Asynchronously performs a full upload of a block from the PLC.
    /// </summary>
    /// <param name="blockType">The type of the block.</param>
    /// <param name="blockNum">The number of the block.</param>
    /// <param name="usrData">The byte array to store the uploaded data.</param>
    /// <param name="size">The size of the uploaded data.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsFullUpload(int blockType, int blockNum, byte[] usrData, ref int size)
    {
        return Cli_AsFullUpload(this.client, blockType, blockNum, usrData, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsDownload(IntPtr client, int blockNum, byte[] usrData, int size);
    /// <summary>
    /// Asynchronously downloads a block to the PLC.
    /// </summary>
    /// <param name="blockNum">The number of the block.</param>
    /// <param name="usrData">The byte array containing the data to download.</param>
    /// <param name="size">The size of the data to download.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsDownload(int blockNum, byte[] usrData, int size)
    {
        return Cli_AsDownload(this.client, blockNum, usrData, size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsPlcCopyRamToRom(IntPtr client, uint timeout);
    /// <summary>
    /// Asynchronously copies data from RAM to ROM in the PLC.
    /// </summary>
    /// <param name="timeout">The timeout for the operation in milliseconds.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsPlcCopyRamToRom(uint timeout)
    {
        return Cli_AsPlcCopyRamToRom(this.client, timeout);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsPlcCompress(IntPtr client, uint timeout);
    /// <summary>
    /// Asynchronously compresses the memory in the PLC.
    /// </summary>
    /// <param name="timeout">The timeout for the operation in milliseconds.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsPlcCompress(uint timeout)
    {
        return Cli_AsPlcCompress(this.client, timeout);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsDBGet(IntPtr client, int dbNumber, byte[] usrData, ref int size);
    /// <summary>
    /// Asynchronously gets a data block (DB) from the PLC.
    /// </summary>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="usrData">The byte array to store the data block.</param>
    /// <param name="size">The size of the data block.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsDbGet(int dbNumber, byte[] usrData, ref int size)
    {
        return Cli_AsDBGet(this.client, dbNumber, usrData, ref size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_AsDBFill(IntPtr client, int dbNumber, int fillChar);
    /// <summary>
    /// Asynchronously fills a data block (DB) in the PLC with a specified character.
    /// </summary>
    /// <param name="dbNumber">The data block number.</param>
    /// <param name="fillChar">The character to fill the data block with.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsDbFill(int dbNumber, int fillChar)
    {
        return Cli_AsDBFill(this.client, dbNumber, fillChar);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_CheckAsCompletion(IntPtr client, ref int opResult);
    /// <summary>
    /// Checks the completion status of an asynchronous operation.
    /// </summary>
    /// <param name="opResult">The output variable to store the operation result.</param>
    /// <returns><see langword="true"/> if the operation is complete; otherwise, <see langword="false"/>.</returns>
    public bool CheckAsCompletion(ref int opResult)
    {
        return Cli_CheckAsCompletion(this.client, ref opResult) == JobComplete;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_WaitAsCompletion(IntPtr client, int timeout);
    /// <summary>
    /// Waits for the completion of an asynchronous operation.
    /// </summary>
    /// <param name="timeout">The timeout for waiting in milliseconds.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int WaitAsCompletion(int timeout)
    {
        return Cli_WaitAsCompletion(this.client, timeout);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetExecTime(IntPtr client, ref uint time);
    /// <summary>
    /// Gets the execution time of the last operation.
    /// </summary>
    /// <returns>The execution time in milliseconds, or -1 if an error occurs.</returns>
    public int ExecTime()
    {
        uint time = default(uint);
        if (Cli_GetExecTime(this.client, ref time) == 0)
            return (int)time;
        else
            return -1;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetLastError(IntPtr client, ref int lastError);
    /// <summary>
    /// Gets the last error code from the PLC client.
    /// </summary>
    /// <returns>The last error code, or -1 if an error occurs.</returns>
    public int LastError()
    {
        int clientLastError = default(int);
        if (Cli_GetLastError(this.client, ref clientLastError) == 0)
            return (int)clientLastError;
        else
            return -1;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetPduLength(IntPtr client, ref int requested, ref int negotiated);

    /// <summary>
    /// Gets the requested PDU (Protocol Data Unit) length.
    /// </summary>
    /// <returns>The requested PDU length, or -1 if an error occurs.</returns>
    public int RequestedPduLength()
    {
        int requested = default(int);
        int negotiated = default(int);
        if (Cli_GetPduLength(this.client, ref requested, ref negotiated) == 0)
            return requested;
        else
            return -1;
    }

    /// <summary>
    /// Gets the negotiated PDU (Protocol Data Unit) length.
    /// </summary>
    /// <returns>The negotiated PDU length, or -1 if an error occurs.</returns>
    public int NegotiatedPduLength()
    {
        int requested = default(int);
        int negotiated = default(int);
        if (Cli_GetPduLength(this.client, ref requested, ref negotiated) == 0)
            return negotiated;
        else
            return -1;
    }

    [DllImport(S7Consts.Snap7LibName, CharSet = CharSet.Ansi)]
    protected static extern int Cli_ErrorText(int error, StringBuilder errMsg, int textSize);
    /// <summary>
    /// Gets the error text for a given error code.
    /// </summary>
    /// <param name="error">The error code.</param>
    /// <returns>A string containing the error description.</returns>
    public string ErrorText(int error)
    {
        StringBuilder message = new StringBuilder(MsgTextLen);
        Cli_ErrorText(error, message, MsgTextLen);
        return message.ToString();
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Cli_GetConnected(IntPtr client, ref uint isConnected);
    /// <summary>
    /// Checks if the client is connected to the PLC.
    /// </summary>
    /// <returns><see langword="true"/> if connected; otherwise, <see langword="false"/>.</returns>
    public bool Connected()
    {
        uint isConnected = default(uint);
        if (Cli_GetConnected(this.client, ref isConnected) == 0)
            return isConnected != 0;
        else
            return false;
    }
}
