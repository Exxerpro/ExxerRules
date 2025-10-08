namespace IndTrace.Dependencies.Helpers;

internal static class SingleInstanceEnforcer
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    private const int SwRestore = 9;

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate single instance enforcer logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static bool EnsureSingleInstance(string processName, ILogger logger)
    {
        string mutexId = $"Global\\{processName}";
        bool createdNew;

        using var mutex = new Mutex(true, mutexId, out createdNew);

        if (createdNew) return true;

        var currentProcess = Process.GetCurrentProcess();
        foreach (var process in Process.GetProcessesByName(processName))
        {
            if (process.Id != currentProcess.Id)
            {
                IntPtr hWnd = process.MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindowAsync(hWnd, SwRestore);
                    SetForegroundWindow(hWnd);
                    Console.WriteLine("Activated existing instance.");
                }
                break;
            }
        }

        return false;
    }
}
