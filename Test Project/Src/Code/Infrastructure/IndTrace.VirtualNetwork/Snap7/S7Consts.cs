namespace IndTrace.VirtualNetwork.Snap7
{
    /// <summary>
    /// Provides constant values and structures for Snap7 communication.
    /// </summary>
    public class S7Consts
    {
        /// <summary>
        /// The name of the Snap7 library.
        /// </summary>
        public const string Snap7LibName = "snap7";

        //------------------------------------------------------------------------------
        //                                  PARAMS LIST
        //------------------------------------------------------------------------------
        /// <summary>
        /// Parameter for local port (PU16LocalPort).
        /// </summary>
        public static readonly int PU16LocalPort = 1;

        /// <summary>
        /// Parameter for remote port (PU16RemotePort).
        /// </summary>
        public static readonly int PU16RemotePort = 2;

        /// <summary>
        /// Parameter for ping timeout (PI32PingTimeout).
        /// </summary>
        public static readonly int PI32PingTimeout = 3;

        /// <summary>
        /// Parameter for send timeout (PI32SendTimeout).
        /// </summary>
        public static readonly int PI32SendTimeout = 4;

        /// <summary>
        /// Parameter for receive timeout (PI32RecvTimeout).
        /// </summary>
        public static readonly int PI32RecvTimeout = 5;

        /// <summary>
        /// Parameter for work interval (PI32WorkInterval).
        /// </summary>
        public static readonly int PI32WorkInterval = 6;

        /// <summary>
        /// Parameter for source reference (PU16SrcRef).
        /// </summary>
        public static readonly int PU16SrcRef = 7;

        /// <summary>
        /// Parameter for destination reference (PU16DstRef).
        /// </summary>
        public static readonly int PU16DstRef = 8;

        /// <summary>
        /// Parameter for source TSAP (PU16SrcTSap).
        /// </summary>
        public static readonly int PU16SrcTSap = 9;

        /// <summary>
        /// Parameter for PDU request (PI32PduRequest).
        /// </summary>
        public static readonly int PI32PduRequest = 10;

        /// <summary>
        /// Parameter for maximum clients (PI32MaxClients).
        /// </summary>
        public static readonly int PI32MaxClients = 11;

        /// <summary>
        /// Parameter for BSend timeout (PI32BSendTimeout).
        /// </summary>
        public static readonly int PI32BSendTimeout = 12;

        /// <summary>
        /// Parameter for BRecv timeout (PI32BRecvTimeout).
        /// </summary>
        public static readonly int PI32BRecvTimeout = 13;

        /// <summary>
        /// Parameter for recovery time (PU32RecoveryTime).
        /// </summary>
        public static readonly int PU32RecoveryTime = 14;

        /// <summary>
        /// Parameter for keep alive time (PU32KeepAliveTime).
        /// </summary>
        public static readonly int PU32KeepAliveTime = 15;

        /// <summary>
        /// Represents an S7 tag with its area, DB number, start address, size, and word length.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct S7Tag
        {
            /// <summary>
            /// The memory area of the tag.
            /// </summary>
            public int Area;

            /// <summary>
            /// The data block number of the tag.
            /// </summary>
            public int DBNumber;

            /// <summary>
            /// The starting address of the tag.
            /// </summary>
            public int Start;

            /// <summary>
            /// The size of the tag in bytes.
            /// </summary>
            public int Size;

            /// <summary>
            /// The word length of the tag.
            /// </summary>
            public int WordLen;
        }

        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate S7 constants logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    }
}
