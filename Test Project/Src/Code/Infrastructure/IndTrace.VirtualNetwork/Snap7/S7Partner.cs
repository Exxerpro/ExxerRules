namespace IndTrace.VirtualNetwork.Snap7;

/// <summary>
/// Represents a Snap7 partner for peer-to-peer communication with other PLCs or devices.
/// </summary>
public class S7Partner
{
    private const int MsgTextLen = 1024;

    // Status
    /// <summary>
    /// Partner is stopped.
    /// </summary>
    public static readonly int ParStopped = 0;
    /// <summary>
    /// Partner is running and actively connecting.
    /// </summary>
    public static readonly int ParConnecting = 1;
    /// <summary>
    /// Partner is running and waiting for a connection.
    /// </summary>
    public static readonly int ParWaiting = 2;
    /// <summary>
    /// Partner is running and connected (linked).
    /// </summary>
    public static readonly int ParLinked = 3;
    /// <summary>
    /// Partner is sending data.
    /// </summary>
    public static readonly int ParSending = 4;
    /// <summary>
    /// Partner is receiving data.
    /// </summary>
    public static readonly int ParReceiving = 5;
    /// <summary>
    /// Error starting passive server.
    /// </summary>
    public static readonly int ParBinderror = 6;

    // Errors
    /// <summary>
    /// Error: Address in use.
    /// </summary>
    public static readonly uint ErrParAddressInUse = 0x00200000;
    /// <summary>
    /// Error: No room.
    /// </summary>
    public static readonly uint ErrParNoRoom = 0x00300000;
    /// <summary>
    /// Error: Server no room.
    /// </summary>
    public static readonly uint ErrServerNoRoom = 0x00400000;
    /// <summary>
    /// Error: Invalid parameters provided.
    /// </summary>
    public static readonly uint ErrParInvalidParams = 0x00500000;
    /// <summary>
    /// Error: Partner is not linked.
    /// </summary>
    public static readonly uint ErrParNotLinked = 0x00600000;
    /// <summary>
    /// Error: Partner is busy.
    /// </summary>
    public static readonly uint ErrParBusy = 0x00700000;
    /// <summary>
    /// Error: Frame timeout.
    /// </summary>
    public static readonly uint ErrParFrameTimeout = 0x00800000;
    /// <summary>
    /// Error: Invalid PDU.
    /// </summary>
    public static readonly uint ErrParInvalidPdu = 0x00900000;
    /// <summary>
    /// Error: Send timeout.
    /// </summary>
    public static readonly uint ErrParSendTimeout = 0x00A00000;
    /// <summary>
    /// Error: Receive timeout.
    /// </summary>
    public static readonly uint ErrParRecvTimeout = 0x00B00000;
    /// <summary>
    /// Error: Send refused.
    /// </summary>
    public static readonly uint ErrParSendRefused = 0x00C00000;
    /// <summary>
    /// Error: Negotiating PDU.
    /// </summary>
    public static readonly uint ErrParNegotiatingPdu = 0x00D00000;
    /// <summary>
    /// Error: Sending block.
    /// </summary>
    public static readonly uint ErrParSendingBlock = 0x00E00000;
    /// <summary>
    /// Error: Receiving block.
    /// </summary>
    public static readonly uint ErrParRecvingBlock = 0x00F00000;
    /// <summary>
    /// Error: Bind error.
    /// </summary>
    public static readonly uint ErrBindError = 0x01000000;
    /// <summary>
    /// Error: Destroying.
    /// </summary>
    public static readonly uint ErrParDestroying = 0x01100000;
    /// <summary>
    /// Error: Invalid parameter number.
    /// </summary>
    public static readonly uint ErrParInvalidParamNumber = 0x01200000;
    /// <summary>
    /// Error: Cannot change parameter.
    /// </summary>
    public static readonly uint ErrParCannotChangeParam = 0x01300000;


    /// <summary>
    /// Generic byte buffer structure, you may need to declare a more
    /// specialistic one in your program.
    //// It's used to cast the input pointer that cames from the callback.
    // See the passive partned demo and the delegate S7ParRecvCallback/    /// Represents a data buffer for S7 operations.
    ///
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S7Buffer
    {
        /// <summary>
        /// Gets or sets the data buffer.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x8000)]
        public byte[] Data;
    }

    // Job status
    private const int JobComplete = 0;
    private const int JobPending = 1;

    private IntPtr partner;

    private int parBytesSent;
    private int parBytesRecv;
    private int parSendErrors;
    private int parRecvErrors;

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern IntPtr Par_Create(int parActive);

    /// <summary>
    /// Initializes a new instance of the <see cref="S7Partner"/> class.
    /// </summary>
    /// <param name="active">Indicates whether this is an active partner (1) or passive partner (0).</param>
    public S7Partner(int active)
    {
        this.partner = Par_Create(active);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_Destroy(ref IntPtr partner);

    /// <summary>
    /// Finalizes an instance of the <see cref="S7Partner"/> class.
    /// </summary>
    ~S7Partner()
    {
        Par_Destroy(ref this.partner);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_StartTo(
        IntPtr partner,
        [MarshalAs(UnmanagedType.LPStr)] string localAddress,
        [MarshalAs(UnmanagedType.LPStr)] string remoteAddress,
        ushort localTsap,
        ushort remoteTsap);

    /// <summary>
    /// Starts a connection to a specific partner.
    /// </summary>
    /// <param name="localAddress">The local IP address.</param>
    /// <param name="remoteAddress">The remote IP address.</param>
    /// <param name="localTsap">The local TSAP.</param>
    /// <param name="remoteTsap">The remote TSAP.</param>
    /// <returns>0 on success, error code otherwise.</returns>
    public int StartTo(
        string localAddress,
        string remoteAddress,
        ushort localTsap,
        ushort remoteTsap)
    {
        return Par_StartTo(this.partner, localAddress, remoteAddress, localTsap, remoteTsap);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_Start(IntPtr partner);
    /// <summary>
    /// Starts the partner connection.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Start()
    {
        return Par_Start(this.partner);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_Stop(IntPtr partner);
    /// <summary>
    /// Stops the partner connection.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Stop()
    {
        return Par_Stop(this.partner);
    }

    // Get/SetParam needs a void* parameter, internally it decides the kind of pointer
    // in accord to ParamNumber.
    // To avoid the use of unsafe code we split the DLL functions and use overloaded methods.

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_GetParam")]
    protected static extern int Par_GetParam_i16(IntPtr partner, int paramNumber, ref short intValue);
    /// <summary>
    /// Gets a 16-bit integer parameter from the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref short intValue)
    {
        return Par_GetParam_i16(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_GetParam")]
    protected static extern int Par_GetParam_u16(IntPtr partner, int paramNumber, ref ushort intValue);
    /// <summary>
    /// Gets an unsigned 16-bit integer parameter from the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref ushort intValue)
    {
        return Par_GetParam_u16(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_GetParam")]
    protected static extern int Par_GetParam_i32(IntPtr partner, int paramNumber, ref int intValue);
    /// <summary>
    /// Gets a 32-bit integer parameter from the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref int intValue)
    {
        return Par_GetParam_i32(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_GetParam")]
    protected static extern int Par_GetParam_u32(IntPtr partner, int paramNumber, ref uint intValue);
    /// <summary>
    /// Gets an unsigned 32-bit integer parameter from the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref uint intValue)
    {
        return Par_GetParam_u32(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_GetParam")]
    protected static extern int Par_GetParam_i64(IntPtr partner, int paramNumber, ref long intValue);
    /// <summary>
    /// Gets a 64-bit integer parameter from the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref long intValue)
    {
        return Par_GetParam_i64(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_GetParam")]
    protected static extern int Par_GetParam_u64(IntPtr partner, int paramNumber, ref ulong intValue);
    /// <summary>
    /// Gets an unsigned 64-bit integer parameter from the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The output variable to store the parameter value.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int GetParam(int paramNumber, ref ulong intValue)
    {
        return Par_GetParam_u64(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_SetParam")]
    protected static extern int Par_SetParam_i16(IntPtr partner, int paramNumber, ref short intValue);
    /// <summary>
    /// Sets a 16-bit integer parameter for the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref short intValue)
    {
        return Par_SetParam_i16(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_SetParam")]
    protected static extern int Par_SetParam_u16(IntPtr partner, int paramNumber, ref ushort intValue);
    /// <summary>
    /// Sets an unsigned 16-bit integer parameter for the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref ushort intValue)
    {
        return Par_SetParam_u16(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_SetParam")]
    protected static extern int Par_SetParam_i32(IntPtr partner, int paramNumber, ref int intValue);
    /// <summary>
    /// Sets a 32-bit integer parameter for the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref int intValue)
    {
        return Par_SetParam_i32(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_SetParam")]
    protected static extern int Par_SetParam_u32(IntPtr partner, int paramNumber, ref uint intValue);
    /// <summary>
    /// Sets an unsigned 32-bit integer parameter for the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref uint intValue)
    {
        return Par_SetParam_u32(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_SetParam")]
    protected static extern int Par_SetParam_i64(IntPtr partner, int paramNumber, ref long intValue);
    /// <summary>
    /// Sets a 64-bit integer parameter for the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref long intValue)
    {
        return Par_SetParam_i64(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName, EntryPoint = "Par_SetParam")]
    protected static extern int Par_SetParam_u64(IntPtr partner, int paramNumber, ref ulong intValue);
    /// <summary>
    /// Sets an unsigned 64-bit integer parameter for the partner.
    /// </summary>
    /// <param name="paramNumber">The parameter number.</param>
    /// <param name="intValue">The value to set.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetParam(int paramNumber, ref ulong intValue)
    {
        return Par_SetParam_u64(this.partner, paramNumber, ref intValue);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_BSend(IntPtr partner, uint rId, byte[] buffer, int size);
    /// <summary>
    /// Sends a block of data to the partner.
    /// </summary>
    /// <param name="rId">The request ID.</param>
    /// <param name="buffer">The byte array containing the data to send.</param>
    /// <param name="size">The size of the data to send.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int BSend(uint rId, byte[] buffer, int size)
    {
        return Par_BSend(this.partner, rId, buffer, size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_AsBSend(IntPtr partner, uint rId, byte[] buffer, int size);
    /// <summary>
    /// Asynchronously sends a block of data to the partner.
    /// </summary>
    /// <param name="rId">The request ID.</param>
    /// <param name="buffer">The byte array containing the data to send.</param>
    /// <param name="size">The size of the data to send.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int AsBSend(uint rId, byte[] buffer, int size)
    {
        return Par_AsBSend(this.partner, rId, buffer, size);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_CheckAsBSendCompletion(IntPtr partner, ref int opResult);
    /// <summary>
    /// Checks the completion status of an asynchronous BSend operation.
    /// </summary>
    /// <param name="opResult">The output variable to store the operation result.</param>
    /// <returns><see langword="true"/> if the operation is complete; otherwise, <see langword="false"/>.</returns>
    public bool CheckAsBSendCompletion(ref int opResult)
    {
        return Par_CheckAsBSendCompletion(this.partner, ref opResult) == JobComplete;
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_WaitAsBSendCompletion(IntPtr partner, int timeout);
    /// <summary>
    /// Waits for the completion of an asynchronous BSend operation.
    /// </summary>
    /// <param name="timeout">The timeout for waiting in milliseconds.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int WaitAsBSendCompletion(int timeout)
    {
        return Par_WaitAsBSendCompletion(this.partner, timeout);
    }

    /// <summary>
    /// Represents the callback for S7 partner send completion.
    /// </summary>
    /// <param name="usrPtr">User-defined pointer.</param>
    /// <param name="opResult">Operation result.</param>
    public delegate void S7ParSendCompletion(IntPtr usrPtr, int opResult);

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_SetSendCallback(IntPtr partner, S7ParSendCompletion completion, IntPtr usrPtr);
    /// <summary>
    /// Sets the send callback function for the partner.
    /// </summary>
    /// <param name="completion">The callback function.</param>
    /// <param name="usrPtr">User-defined pointer to be passed to the callback.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetSendCallBack(S7ParSendCompletion completion, IntPtr usrPtr)
    {
        return Par_SetSendCallback(this.partner, completion, usrPtr);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_BRecv(IntPtr partner, ref uint rId, byte[] buffer, ref int size, uint timeout);
    /// <summary>
    /// Receives a block of data from the partner.
    /// </summary>
    /// <param name="rId">The request ID.</param>
    /// <param name="buffer">The byte array to store the received data.</param>
    /// <param name="size">The size of the received data.</param>
    /// <param name="timeout">The timeout for receiving in milliseconds.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int BRecv(ref uint rId, byte[] buffer, ref int size, uint timeout)
    {
        return Par_BRecv(this.partner, ref rId, buffer, ref size, timeout);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_CheckAsBRecvCompletion(IntPtr partner, ref int opResult, ref uint rId, byte[] buffer, ref int size);
    /// <summary>
    /// Checks the completion status of an asynchronous BRecv operation.
    /// </summary>
    /// <param name="opResult">The output variable to store the operation result.</param>
    /// <param name="rId">The request ID.</param>
    /// <param name="buffer">The byte array containing the received data.</param>
    /// <param name="size">The size of the received data.</param>
    /// <returns><see langword="true"/> if the operation is complete; otherwise, <see langword="false"/>.</returns>
    public bool CheckAsBRecvCompletion(ref int opResult, ref uint rId, byte[] buffer, ref int size)
    {
        Par_CheckAsBRecvCompletion(this.partner, ref opResult, ref rId, buffer, ref size);
        return opResult == JobComplete;
    }

    /// <summary>
    /// Represents the callback for S7 partner receive completion.
    /// </summary>
    /// <param name="usrPtr">User-defined pointer.</param>
    /// <param name="opResult">Operation result.</param>
    /// <param name="rId">Request ID.</param>
    /// <param name="pData">Pointer to the received data.</param>
    /// <param name="size">Size of the received data.</param>
    public delegate void S7ParRecvCallback(IntPtr usrPtr, int opResult, uint rId, IntPtr pData, int size);

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_SetRecvCallback(IntPtr partner, S7ParRecvCallback callback, IntPtr usrPtr);
    /// <summary>
    /// Sets the receive callback function for the partner.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <param name="usrPtr">User-defined pointer to be passed to the callback.</param>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int SetRecvCallback(S7ParRecvCallback callback, IntPtr usrPtr)
    {
        return Par_SetRecvCallback(this.partner, callback, usrPtr);
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_GetLastError(IntPtr partner, ref int lastError);
    /// <summary>
    /// Gets the last error code from the partner.
    /// </summary>
    /// <param name="lastError">The output variable to store the last error code.</param>
    /// <returns>The last error code, or -1 if an error occurs.</returns>
    public int LastError(ref int lastError)
    {
        int partnerLastError = default(int);
        if (Par_GetLastError(this.partner, ref partnerLastError) == 0)
            return (int)partnerLastError;
        else
            return -1;
    }

    [DllImport(S7Consts.Snap7LibName, CharSet = CharSet.Ansi)]
    protected static extern int Par_ErrorText(int error, StringBuilder errMsg, int textSize);
    /// <summary>
    /// Gets the error text for a given error code.
    /// </summary>
    /// <param name="error">The error code.</param>
    /// <returns>A string containing the error description.</returns>
    public string ErrorText(int error)
    {
        StringBuilder message = new StringBuilder(MsgTextLen);
        Par_ErrorText(error, message, MsgTextLen);
        return message.ToString();
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_GetStats(IntPtr partner, ref int bytesSent, ref int bytesRecv,
        ref int sendErrors, ref int recvErrors);

    private void GetStatistics()
    {
        if (Par_GetStats(this.partner, ref this.parBytesSent, ref this.parBytesRecv, ref this.parSendErrors, ref this.parRecvErrors) != 0)
        {
            this.parBytesSent = -1;
            this.parBytesRecv = -1;
            this.parSendErrors = -1;
            this.parRecvErrors = -1;
        }
    }

    /// <summary>
    /// Gets the number of bytes sent by the partner.
    /// </summary>
    public int BytesSent
    {
        get
        {
            this.GetStatistics();
            return this.parBytesSent;
        }
    }

    /// <summary>
    /// Gets the number of bytes received by the partner.
    /// </summary>
    public int BytesRecv
    {
        get
        {
            this.GetStatistics();
            return this.parBytesRecv;
        }
    }

    /// <summary>
    /// Gets the number of send errors encountered by the partner.
    /// </summary>
    public int SendErrors
    {
        get
        {
            this.GetStatistics();
            return this.parSendErrors;
        }
    }

    /// <summary>
    /// Gets the number of receive errors encountered by the partner.
    /// </summary>
    public int RecvErrors
    {
        get
        {
            this.GetStatistics();
            return this.parRecvErrors;
        }
    }

    [DllImport(S7Consts.Snap7LibName)]
    protected static extern int Par_GetStatus(IntPtr partner, ref int status);

    /// <summary>
    /// Gets the current status of the partner.
    /// </summary>
    public int Status
    {
        get
        {
            int parStatus = default(int);
            if (Par_GetStatus(this.partner, ref parStatus) != 0)
                return -1;
            else
                return parStatus;
        }
    }
    /// <summary>
    /// Gets a value indicating whether the partner is linked (connected).
    /// </summary>
    public bool Linked
    {
        get
        {
            return this.Status == ParLinked;
        }
    }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate S7 partner logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    //TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated partner or communication logic. Refactor for maintainability if necessary.
    //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency partner operations, consider optimizing communication and memory usage.

}
