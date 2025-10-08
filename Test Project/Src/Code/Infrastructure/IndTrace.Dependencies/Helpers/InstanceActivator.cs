namespace IndTrace.Dependencies.Helpers
{
    /// <summary>
    /// Provides methods to activate or bring an existing process window to the foreground.
    /// </summary>
    public static class InstanceActivator
    {
        // TODO 1: Refactor InstanceActivator.cs into platform-specific files
        // (InstanceActivator.Windows.cs, InstanceActivator.Linux.cs) for clarity and maintainability.

        /// <summary>
        /// Attempts to bring the window of an existing process with the specified name to the foreground.
        /// </summary>
        /// <param name="processName">The name of the process whose window should be activated.</param>
        public static void TryActivateWindow(string processName)
        {
            try
            {
#if WINDOWS
            var current = ProcessAsync.GetCurrentProcess();
            foreach (var process in ProcessAsync.GetProcessesByName(processName))
            {
                if (process.RecipeId != current.RecipeId && process.MainWindowHandle != IntPtr.Zero)
                {
                    ShowWindow(process.MainWindowHandle, 1); // 1 = SW_SHOWNORMAL
                    SetForegroundWindow(process.MainWindowHandle);
                    return;
                }
            }
#elif LINUX
            if (!string.IsNullOrWhiteSpace(windowTitle))
            {
                ProcessAsync.Start("xdotool", $"search --name \"{processName}\" windowactivate");
            }
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WindowActivator] Could not bring window to front: {ex.Message}");
            }
        }

#if WINDOWS
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
#endif

        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate instance activator logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    }
}
