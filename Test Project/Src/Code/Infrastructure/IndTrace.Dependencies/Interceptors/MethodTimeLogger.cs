using Serilog;
using ILogger = Serilog.ILogger;

namespace IndTrace.Dependencies.Interceptors
{
    public static class MethodTimeLogger
    {
        public static ILogger? Logger { get; set; }

        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate method time logger logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
        public static void Log(MethodBase methodBase, long milliseconds, string message)
        {
            //Do some logging here

            Logger?.Information(
                "{MethodName} took {Milliseconds} milliseconds. {Message}",
                methodBase.Name,
                milliseconds,
                message);
        }
    }
}
