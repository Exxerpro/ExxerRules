namespace IndTrace.VirtualNetwork.Snap7;

/// <summary>
/// Provides functionality for implementing an S7 server, handling PLC communication and data areas.
/// </summary>
public class S7Server
{
    private const int MsgTextLen = 1024;
    private const int MkEvent = 0;
    private const int MkLog = 1;

    // S7 Area ID
    /// <summary>ProcessAsync inputs area ID.</summary>
    public const byte S7AreaPe = 0x81;
    /// <summary>ProcessAsync outputs area ID.</summary>
    public const byte S7AreaPa = 0x82;
    /// <summary>Merkers area ID.</summary>
    public const byte S7AreaMk = 0x83;
    /// <summary>DB (Data Block) area ID.</summary>
    public const byte S7AreaDb = 0x84;
    /// <summary>Counters area ID.</summary>
    public const byte S7AreaCt = 0x1C;
    /// <summary>Timers area ID.</summary>
    public const byte S7AreaTm = 0x1D;
    // S7 Word Length
    /// <summary>Bit data type length.</summary>
    public const int S7WlBit = 0x01;
    /// <summary>Byte data type length.</summary>
    public const int S7WlByte = 0x02;
    /// <summary>Word data type length.</summary>
    public const int S7WlWord = 0x04;
    /// <summary>Double word data type length.</summary>
    public const int S7WldWord = 0x06;
    /// <summary>Real (floating point) data type length.</summary>
    public const int S7WlReal = 0x08;
    /// <summary>Counter data type length.</summary>
    public const int S7WlCounter = 0x1C;
    /// <summary>Timer data type length.</summary>
    public const int S7WlTimer = 0x1D;

    // Server Area ID  (use with Register/unregister - Lock/unlock Area)
    public static readonly int SrvAreaPe = 0;
    public static readonly int SrvAreaPa = 1;
    public static readonly int SrvAreaMk = 2;
    public static readonly int SrvAreaCt = 3;
    public static readonly int SrvAreaTm = 4;
    public static readonly int SrvAreaDb = 5;
    // Errors
    public static readonly uint ErrSrvCannotStart = 0x00100000; // Server cannot start
    public static readonly uint ErrSrvDbNullPointer = 0x00200000; // Passed null as PData
    public static readonly uint ErrSrvAreaAlreadyExists = 0x00300000; // Area Re-registration
    public static readonly uint ErrSrvUnknownArea = 0x00400000; // Unknown area
    public static readonly uint ErrSrvInvalidParams = 0x00500000; // Invalid param(s) supplied
    public static readonly uint ErrSrvTooManyDb = 0x00600000; // Cannot register DB
    public static readonly uint ErrSrvInvalidParamNumber = 0x00700000; // Invalid param (srv_get/set_param)
    public static readonly uint ErrSrvCannotChangeParam = 0x00800000; // Cannot change because running
    // TCP Server Event codes
    public static readonly uint EvcServerStarted = 0x00000001;
    public static readonly uint EvcServerStopped = 0x00000002;
    public static readonly uint EvcListenerCannotStart = 0x00000004;
    public static readonly uint EvcClientAdded = 0x00000008;
    public static readonly uint EvcClientRejected = 0x00000010;
    public static readonly uint EvcClientNoRoom = 0x00000020;
    public static readonly uint EvcClientException = 0x00000040;
    public static readonly uint EvcClientDisconnected = 0x00000080;
    public static readonly uint EvcClientTerminated = 0x00000100;
    public static readonly uint EvcClientsDropped = 0x00000200;
    public static readonly uint EvcReserved00000400 = 0x00000400; // actually unused
    public static readonly uint EvcReserved00000800 = 0x00000800; // actually unused
    public static readonly uint EvcReserved00001000 = 0x00001000; // actually unused
    public static readonly uint EvcReserved00002000 = 0x00002000; // actually unused
    public static readonly uint EvcReserved00004000 = 0x00004000; // actually unused
    public static readonly uint EvcReserved00008000 = 0x00008000; // actually unused
    // S7 Server Event Code
    public static readonly uint EvcPdUincoming = 0x00010000;
    public static readonly uint EvcDataRead = 0x00020000;
    public static readonly uint EvcDataWrite = 0x00040000;
    public static readonly uint EvcNegotiatePdu = 0x00080000;
    public static readonly uint EvcReadSzl = 0x00100000;
    public static readonly uint EvcClock = 0x00200000;
    public static readonly uint EvcUpload = 0x00400000;
    public static readonly uint EvcDownload = 0x00800000;
    public static readonly uint EvcDirectory = 0x01000000;
    public static readonly uint EvcSecurity = 0x02000000;
    public static readonly uint EvcControl = 0x04000000;
    public static readonly uint EvcReserved08000000 = 0x08000000; // actually unused
    public static readonly uint EvcReserved10000000 = 0x10000000; // actually unused
    public static readonly uint EvcReserved20000000 = 0x20000000; // actually unused
    public static readonly uint EvcReserved40000000 = 0x40000000; // actually unused
    public static readonly uint EvcReserved80000000 = 0x80000000; // actually unused
    // Masks to enable/disable all events
    public static readonly uint EvcAll = 0xFFFFFFFF;
    public static readonly uint EvcNone = 0x00000000;
    // Event SubCodes
    public static readonly ushort EvsUnknown = 0x0000;
    public static readonly ushort EvsStartUpload = 0x0001;
    public static readonly ushort EvsStartDownload = 0x0001;
    public static readonly ushort EvsGetBlockList = 0x0001;
    public static readonly ushort EvsStartListBoT = 0x0002;
    public static readonly ushort EvsListBoT = 0x0003;
    public static readonly ushort EvsGetBlockInfo = 0x0004;
    public static readonly ushort EvsGetClock = 0x0001;
    public static readonly ushort EvsSetClock = 0x0002;
    public static readonly ushort EvsSetPassword = 0x0001;
    public static readonly ushort EvsClrPassword = 0x0002;
    // Event Params : functions group
    public static readonly ushort GrProgrammer = 0x0041;
    public static readonly ushort GrCyclicData = 0x0042;
    public static readonly ushort GrBlocksInfo = 0x0043;
    public static readonly ushort GrSzl = 0x0044;
    public static readonly ushort GrPassword = 0x0045;
    public static readonly ushort GrBSend = 0x0046;
    public static readonly ushort GrClock = 0x0047;
    public static readonly ushort GrSecurity = 0x0045;
    // Event Params : control codes
    public static readonly ushort CodeControlUnknown = 0x0000;
    public static readonly ushort CodeControlColdStart = 0x0001;
    public static readonly ushort CodeControlWarmStart = 0x0002;
    public static readonly ushort CodeControlStop = 0x0003;
    public static readonly ushort CodeControlCompress = 0x0004;
    public static readonly ushort CodeControlCpyRamRom = 0x0005;
    public static readonly ushort CodeControlInsDel = 0x0006;
    // Event Result
    public static readonly ushort EvrNoError = 0x0000;
    public static readonly ushort EvrFragmentRejected = 0x0001;
    public static readonly ushort EvrMalformedPdu = 0x0002;
    public static readonly ushort EvrSparseBytes = 0x0003;
    public static readonly ushort EvrCannotHandlePdu = 0x0004;
    public static readonly ushort EvrNotImplemented = 0x0005;
    public static readonly ushort EvrErrException = 0x0006;
    public static readonly ushort EvrErrAreaNotFound = 0x0007;
    public static readonly ushort EvrErrOutOfRange = 0x0008;
    public static readonly ushort EvrErrOverPdu = 0x0009;
    public static readonly ushort EvrErrTransportSize = 0x000A;
    public static readonly ushort EvrInvalidGroupUData = 0x000B;
    public static readonly ushort EvrInvalidSzl = 0x000C;
    public static readonly ushort EvrDataSizeMismatch = 0x000D;
    public static readonly ushort EvrCannotUpload = 0x000E;
    public static readonly ushort EvrCannotDownload = 0x000F;
    public static readonly ushort EvrUploadInvalidId = 0x0010;
    public static readonly ushort EvrResNotFound = 0x0011;

    // Read/Write Operation (to be used into RWCallback)
    public static readonly int OperationRead = 0;
    public static readonly int OperationWrite = 1;
    /// <summary>
    /// Represents the USrvEvent.
    /// </summary>

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct USrvEvent
    {
        public IntPtr EvtTime;   // It's platform dependent (32 or 64 bit)
        public int EvtSender;
        public uint EvtCode;
        public ushort EvtRetCode;
        public ushort EvtParam1;
        public ushort EvtParam2;
        public ushort EvtParam3;
        public ushort EvtParam4;
    }
    /// <summary>
    /// Represents the SrvEvent.
    /// </summary>

    public struct SrvEvent
    {
        public DateTime EvtTime;
        public int EvtSender;
        public uint EvtCode;
        public ushort EvtRetCode;
        public ushort EvtParam1;
        public ushort EvtParam2;
        public ushort EvtParam3;
        public ushort EvtParam4;
    }

    private Dictionary<int, GCHandle> hArea;

    private IntPtr server;

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern IntPtr Srv_Create();
    /// <summary>
    /// Initializes a new instance of the <see cref="S7Server"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public S7Server()
    {
        this.server = Srv_Create();
        this.hArea = [];
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_Destroy(ref IntPtr server);

    ~S7Server()
    {
        foreach (var item in this.hArea)
        {
            GCHandle handle = item.Value;
            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }
        Srv_Destroy(ref this.server);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_StartTo(IntPtr server, [MarshalAs(UnmanagedType.LPStr)] string address);
    /// <summary>
    /// Executes StartTo operation.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>The result of StartTo.</returns>
    public int StartTo(string address)
    {
        return Srv_StartTo(this.server, address);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_Start(IntPtr server);
    /// <summary>
    /// Executes Start operation.
    /// </summary>
    /// <returns>The result of Start.</returns>
    public int Start()
    {
        return Srv_Start(this.server);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_Stop(IntPtr server);
    /// <summary>
    /// Executes Stop operation.
    /// </summary>
    /// <returns>The result of Stop.</returns>
    public int Stop()
    {
        return Srv_Stop(this.server);
    }

    // Get/SetParam needs a void* parameter, internally it decides the kind of pointer
    // in accord to ParamNumber.
    // To avoid the use of unsafe code we split the DLL functions and use overloaded methods.

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_GetParam")]
    protected static extern int Srv_GetParam_i16(IntPtr server, int paramNumber, ref short intValue);
    /// <summary>
    /// Executes GetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of GetParam.</returns>
    public int GetParam(int paramNumber, ref short intValue)
    {
        return Srv_GetParam_i16(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_GetParam")]
    protected static extern int Srv_GetParam_u16(IntPtr server, int paramNumber, ref ushort intValue);
    /// <summary>
    /// Executes GetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of GetParam.</returns>
    public int GetParam(int paramNumber, ref ushort intValue)
    {
        return Srv_GetParam_u16(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_GetParam")]
    protected static extern int Srv_GetParam_i32(IntPtr server, int paramNumber, ref int intValue);
    /// <summary>
    /// Executes GetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of GetParam.</returns>
    public int GetParam(int paramNumber, ref int intValue)
    {
        return Srv_GetParam_i32(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_GetParam")]
    protected static extern int Srv_GetParam_u32(IntPtr server, int paramNumber, ref uint intValue);
    /// <summary>
    /// Executes GetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of GetParam.</returns>
    public int GetParam(int paramNumber, ref uint intValue)
    {
        return Srv_GetParam_u32(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_GetParam")]
    protected static extern int Srv_GetParam_i64(IntPtr server, int paramNumber, ref long intValue);
    /// <summary>
    /// Executes GetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of GetParam.</returns>
    public int GetParam(int paramNumber, ref long intValue)
    {
        return Srv_GetParam_i64(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_GetParam")]
    protected static extern int Srv_GetParam_u64(IntPtr server, int paramNumber, ref ulong intValue);
    /// <summary>
    /// Executes GetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of GetParam.</returns>
    public int GetParam(int paramNumber, ref ulong intValue)
    {
        return Srv_GetParam_u64(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_SetParam")]
    protected static extern int Srv_SetParam_i16(IntPtr server, int paramNumber, ref short intValue);
    /// <summary>
    /// Executes SetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of SetParam.</returns>
    public int SetParam(int paramNumber, ref short intValue)
    {
        return Srv_SetParam_i16(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_SetParam")]
    protected static extern int Srv_SetParam_u16(IntPtr server, int paramNumber, ref ushort intValue);
    /// <summary>
    /// Executes SetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of SetParam.</returns>
    public int SetParam(int paramNumber, ref ushort intValue)
    {
        return Srv_SetParam_u16(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_SetParam")]
    protected static extern int Srv_SetParam_i32(IntPtr server, int paramNumber, ref int intValue);
    /// <summary>
    /// Executes SetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of SetParam.</returns>
    public int SetParam(int paramNumber, ref int intValue)
    {
        return Srv_SetParam_i32(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_SetParam")]
    protected static extern int Srv_SetParam_u32(IntPtr server, int paramNumber, ref uint intValue);
    /// <summary>
    /// Executes SetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of SetParam.</returns>
    public int SetParam(int paramNumber, ref uint intValue)
    {
        return Srv_SetParam_u32(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_SetParam")]
    protected static extern int Srv_SetParam_i64(IntPtr server, int paramNumber, ref long intValue);
    /// <summary>
    /// Executes SetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of SetParam.</returns>
    public int SetParam(int paramNumber, ref long intValue)
    {
        return Srv_SetParam_i64(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Srv_SetParam")]
    protected static extern int Srv_SetParam_u64(IntPtr server, int paramNumber, ref ulong intValue);
    /// <summary>
    /// Executes SetParam operation.
    /// </summary>
    /// <param name="paramNumber">The paramNumber.</param>
    /// <param name="intValue">The intValue.</param>
    /// <returns>The result of SetParam.</returns>
    public int SetParam(int paramNumber, ref ulong intValue)
    {
        return Srv_SetParam_u64(this.server, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_RegisterArea(IntPtr server, int areaCode, int index, IntPtr pUsrData, int size);

    public int RegisterArea<T>(int areaCode, int index, ref T pUsrData, int size)
    {
        int areaUid = (areaCode << 16) + index;
        GCHandle handle = GCHandle.Alloc(pUsrData, GCHandleType.Pinned);
        int result = Srv_RegisterArea(this.server, areaCode, index, handle.AddrOfPinnedObject(), size);
        if (result == 0)
            this.hArea.Add(areaUid, handle);
        else
            handle.Free();
        return result;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_UnregisterArea(IntPtr server, int areaCode, int index);
    /// <summary>
    /// Executes UnregisterArea operation.
    /// </summary>
    /// <param name="areaCode">The areaCode.</param>
    /// <param name="index">The index.</param>
    /// <returns>The result of UnregisterArea.</returns>
    public int UnregisterArea(int areaCode, int index)
    {
        int result = Srv_UnregisterArea(this.server, areaCode, index);
        if (result == 0)
        {
            int areaUid = (areaCode << 16) + index;
            if (this.hArea.ContainsKey(areaUid)) // should be always true
            {
                GCHandle handle = this.hArea[areaUid];
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
                this.hArea.Remove(areaUid);
            }
        }
        return result;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_LockArea(IntPtr server, int areaCode, int index);
    /// <summary>
    /// Executes LockArea operation.
    /// </summary>
    /// <param name="areaCode">The areaCode.</param>
    /// <param name="index">The index.</param>
    /// <returns>The result of LockArea.</returns>
    public int LockArea(int areaCode, int index)
    {
        return Srv_LockArea(this.server, areaCode, index);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_UnlockArea(IntPtr server, int areaCode, int index);
    /// <summary>
    /// Executes UnlockArea operation.
    /// </summary>
    /// <param name="areaCode">The areaCode.</param>
    /// <param name="index">The index.</param>
    /// <returns>The result of UnlockArea.</returns>
    public int UnlockArea(int areaCode, int index)
    {
        return Srv_UnlockArea(this.server, areaCode, index);
    }


    /// <summary>
    /// Represents the RwBuffer.
    /// </summary>

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RwBuffer
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)] // A telegram cannot exceed PDU size (960 bytes)
        public byte[] Data;
    }

    public delegate void SrvCallback(IntPtr usrPtr, ref USrvEvent @event, int size);

    public delegate int SrvRwAreaCallback(IntPtr usrPtr, int sender, int operation, ref S7Consts.S7Tag tag, ref RwBuffer buffer);

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_SetEventsCallback(IntPtr server, SrvCallback callback, IntPtr usrPtr);
    /// <summary>
    /// Executes SetEventsCallBack operation.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="usrPtr">The usrPtr.</param>
    /// <returns>The result of SetEventsCallBack.</returns>
    public int SetEventsCallBack(SrvCallback callback, IntPtr usrPtr)
    {
        return Srv_SetEventsCallback(this.server, callback, usrPtr);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_SetReadEventsCallback(IntPtr server, SrvCallback callback, IntPtr usrPtr);
    /// <summary>
    /// Executes SetReadEventsCallBack operation.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="usrPtr">The usrPtr.</param>
    /// <returns>The result of SetReadEventsCallBack.</returns>
    public int SetReadEventsCallBack(SrvCallback callback, IntPtr usrPtr)
    {
        return Srv_SetReadEventsCallback(this.server, callback, usrPtr);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_SetRWAreaCallback(IntPtr server, SrvRwAreaCallback callback, IntPtr usrPtr);
    /// <summary>
    /// Executes SetRwAreaCallBack operation.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="usrPtr">The usrPtr.</param>
    /// <returns>The result of SetRwAreaCallBack.</returns>
    public int SetRwAreaCallBack(SrvRwAreaCallback callback, IntPtr usrPtr)
    {
        return Srv_SetRWAreaCallback(this.server, callback, usrPtr);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_PickEvent(IntPtr server, ref USrvEvent @event, ref int evtReady);
    /// <summary>
    /// Executes PickEvent operation.
    /// </summary>
    /// <param name="@event">The @event.</param>
    /// <returns>The result of PickEvent.</returns>
    public bool PickEvent(ref USrvEvent @event)
    {
        int evtReady = default(int);
        if (Srv_PickEvent(this.server, ref @event, ref evtReady) == 0)
            return evtReady != 0;
        else
            return false;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_ClearEvents(IntPtr server);
    /// <summary>
    /// Executes ClearEvents operation.
    /// </summary>
    /// <returns>The result of ClearEvents.</returns>
    public int ClearEvents()
    {
        return Srv_ClearEvents(this.server);
    }

    [DllImport(S7Consts.Snap7LibName, CharSet = CharSet.Ansi)]
    protected static extern int Srv_EventText(ref USrvEvent @event, StringBuilder evtMsg, int textSize);
    /// <summary>
    /// Executes EventText operation.
    /// </summary>
    /// <param name="@event">The @event.</param>
    /// <returns>The result of EventText.</returns>
    public string EventText(ref USrvEvent @event)
    {
        StringBuilder message = new StringBuilder(MsgTextLen);
        Srv_EventText(ref @event, message, MsgTextLen);
        return message.ToString();
    }
    /// <summary>
    /// Executes EvtTimeToDateTime operation.
    /// </summary>
    /// <param name="timeStamp">The timeStamp.</param>
    /// <returns>The result of EvtTimeToDateTime.</returns>

    public DateTime EvtTimeToDateTime(IntPtr timeStamp)
    {
        DateTime unixStartEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return unixStartEpoch.AddSeconds(Convert.ToDouble(timeStamp));
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_GetMask(IntPtr server, int maskKind, ref uint mask);

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_SetMask(IntPtr server, int maskKind, uint mask);

    // Property LogMask R/W
    public uint LogMask
    {
        get
        {
            uint mask = default(uint);
            if (Srv_GetMask(this.server, S7Server.MkLog, ref mask) == 0)
                return mask;
            else
                return 0;
        }

        set
        {
            Srv_SetMask(this.server, S7Server.MkLog, value);
        }
    }

    // Property EventMask R/W
    public uint EventMask
    {
        get
        {
            uint mask = default(uint);
            if (Srv_GetMask(this.server, S7Server.MkEvent, ref mask) == 0)
                return mask;
            else
                return 0;
        }

        set
        {
            Srv_SetMask(this.server, S7Server.MkEvent, value);
        }
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_GetStatus(IntPtr server, ref int serverStatus, ref int cpuStatus, ref int clientsCount);

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Srv_SetCpuStatus(IntPtr server, int cpuStatus);

    // Property Virtual CPU status R/W
    public int CpuStatus
    {
        get
        {
            int cStatus = default(int);
            int sStatus = default(int);
            int cCount = default(int);

            if (Srv_GetStatus(this.server, ref sStatus, ref cStatus, ref cCount) == 0)
                return cStatus;
            else
                return -1;
        }

        set
        {
            Srv_SetCpuStatus(this.server, value);
        }
    }

    // Property Server Status Read Only
    public int ServerStatus
    {
        get
        {
            int cStatus = default(int);
            int sStatus = default(int);
            int cCount = default(int);
            if (Srv_GetStatus(this.server, ref sStatus, ref cStatus, ref cCount) == 0)
                return sStatus;
            else
                return -1;
        }
    }

    // Property Clients Count Read Only
    public int ClientsCount
    {
        get
        {
            int cStatus = default(int);
            int sStatus = default(int);
            int cCount = default(int);
            if (Srv_GetStatus(this.server, ref cStatus, ref sStatus, ref cCount) == 0)
                return cCount;
            else
                return -1;
        }
    }

    [DllImport(S7Consts.Snap7LibName, CharSet = CharSet.Ansi)]
    protected static extern int Srv_ErrorText(int error, StringBuilder errMsg, int textSize);
    /// <summary>
    /// Executes ErrorText operation.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>The result of ErrorText.</returns>
    public string ErrorText(int error)
    {
        StringBuilder message = new StringBuilder(MsgTextLen);
        Srv_ErrorText(error, message, MsgTextLen);
        return message.ToString();
    }
}
