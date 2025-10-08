namespace IndTrace.VirtualNetwork.Snap7;

/// <summary>
/// Provides functionality for reading and writing multiple variables to and from an S7 PLC in a single request.
/// </summary>
public class S7MultiVar
{
    private S7Client fClient;
    private GCHandle[] handles = new GCHandle[S7Client.MaxVars];
    private int count = 0;
    private S7Client.S7DataItem[] items = new S7Client.S7DataItem[S7Client.MaxVars];

    /// <summary>
    /// Gets the results of the multi-variable operation.
    /// </summary>
    public int[] Results = new int[S7Client.MaxVars];

    /// <summary>
    /// Initializes a new instance of the <see cref="S7MultiVar"/> class.
    /// </summary>
    /// <param name="client">The S7 client instance to use for communication.</param>
    public S7MultiVar(S7Client client)
    {
        this.fClient = client;
        for (int c = 0; c < S7Client.MaxVars; c++)
            this.Results[c] = (int)S7Client.ErrCliItemNotAvailable;
    }
    /// <summary>
    /// Finalizes an instance of the <see cref="S7MultiVar"/> class.
    /// </summary>
    ~S7MultiVar()
    {
        this.Clear();
    }

    /// <summary>
    /// Adds a variable to the multi-variable read/write request.
    /// </summary>
    /// <typeparam name="T">The type of the buffer elements.</typeparam>
    /// <param name="area">The memory area of the variable.</param>
    /// <param name="wordLen">The word length of the variable.</param>
    /// <param name="dbNumber">The DB number of the variable (if applicable).</param>
    /// <param name="start">The starting address of the variable.</param>
    /// <param name="amount">The amount of data to read/write.</param>
    /// <param name="buffer">The buffer containing the data.</param>
    /// <returns><see langword="true"/> if the variable was successfully added; otherwise, <see langword="false"/>.</returns>
    public bool Add<T>(int area, int wordLen, int dbNumber, int start, int amount, ref T[] buffer)
    {
        return this.Add(area, wordLen, dbNumber, start, amount, ref buffer, 0);
    }

    /// <summary>
    /// Adds a variable to the multi-variable read/write request with an offset.
    /// </summary>
    /// <typeparam name="T">The type of the buffer elements.</typeparam>
    /// <param name="area">The memory area of the variable.</param>
    /// <param name="wordLen">The word length of the variable.</param>
    /// <param name="dbNumber">The DB number of the variable (if applicable).</param>
    /// <param name="start">The starting address of the variable.</param>
    /// <param name="amount">The amount of data to read/write.</param>
    /// <param name="buffer">The buffer containing the data.</param>
    /// <param name="offset">The offset within the buffer.</param>
    /// <returns><see langword="true"/> if the variable was successfully added; otherwise, <see langword="false"/>.</returns>
    public bool Add<T>(int area, int wordLen, int dbNumber, int start, int amount, ref T[] buffer, int offset)
    {
        if (this.count < S7Client.MaxVars)
        {
            this.items[this.count].Area = area;
            this.items[this.count].WordLen = wordLen;
            this.items[this.count].Result = 0;
            this.items[this.count].DBNumber = dbNumber;
            this.items[this.count].Start = start;
            this.items[this.count].Amount = amount;
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            if (IntPtr.Size == 4)
                this.items[this.count].PData = (IntPtr)(handle.AddrOfPinnedObject().ToInt32() + (offset * Marshal.SizeOf(typeof(T))));
            else
                this.items[this.count].PData = (IntPtr)(handle.AddrOfPinnedObject().ToInt64() + (offset * Marshal.SizeOf(typeof(T))));

            this.handles[this.count] = handle;
            this.count++;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Executes the multi-variable read request.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Read()
    {
        int functionResult;
        if (this.count > 0)
        {
            functionResult = this.fClient.ReadMultiVars(this.items, this.count);
            if (functionResult == 0)
            {
                for (int c = 0; c < S7Client.MaxVars; c++)
                    this.Results[c] = this.items[c].Result;
            }

            return functionResult;
        }
        else
        {
            return (int)S7Client.ErrCliFunctionRefused;
        }
    }
    /// <summary>
    /// Executes the multi-variable write request.
    /// </summary>
    /// <returns>An integer representing the result of the operation (0 for success).</returns>
    public int Write()
    {
        int functionResult;
        if (this.count > 0)
        {
            functionResult = this.fClient.WriteMultiVars(this.items, this.count);
            if (functionResult == 0)
            {
                for (int c = 0; c < S7Client.MaxVars; c++)
                    this.Results[c] = this.items[c].Result;
            }

            return functionResult;
        }
        else
        {
            return (int)S7Client.ErrCliFunctionRefused;
        }
    }
    /// <summary>
    /// Clears all added variables from the multi-variable request.
    /// </summary>
    public void Clear()
    {
        for (int c = 0; c < this.count; c++)
        {
            if (this.handles[c].IsAllocated)
            {
                this.handles[c].Free();
            }
        }
        for (int c = 0; c < S7Client.MaxVars; c++)
            this.Results[c] = (int)S7Client.ErrCliItemNotAvailable;
        this.count = 0;
    }
}
