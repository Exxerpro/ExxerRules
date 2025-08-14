using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestRunnerStandalone
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Standalone Test Runner with Timeout");
            Console.WriteLine("===================================");
            
            string command = "dotnet test --filter \"ResultComprehensiveTests\" --verbosity minimal";
            int timeoutSeconds = 600; // 10 minutes
            
            if (args.Length > 0)
            {
                command = args[0];
            }
            
            if (args.Length > 1 && int.TryParse(args[1], out int customTimeout))
            {
                timeoutSeconds = customTimeout;
            }
            
            Console.WriteLine($"Command: {command}");
            Console.WriteLine($"Timeout: {timeoutSeconds} seconds");
            Console.WriteLine($"Starting at: {DateTime.Now}");
            Console.WriteLine();
            
            try
            {
                var result = await RunCommandWithTimeoutAsync(command, timeoutSeconds);
                Console.WriteLine();
                Console.WriteLine($"Completed at: {DateTime.Now}");
                Console.WriteLine($"Exit code: {result}");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }
        
        static async Task<int> RunCommandWithTimeoutAsync(string command, int timeoutSeconds)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            };
            
            using var process = new Process { StartInfo = processInfo };
            
            // Start the process
            process.Start();
            
            // Create a task to wait for the process to complete
            var processTask = Task.Run(() =>
            {
                process.WaitForExit();
                return process.ExitCode;
            });
            
            // Create a timeout task
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            
            // Wait for either the process to complete or timeout
            var completedTask = await Task.WhenAny(processTask, timeoutTask);
            
            if (completedTask == timeoutTask)
            {
                // Timeout occurred
                Console.WriteLine($"Command timed out after {timeoutSeconds} seconds");
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error killing process: {ex.Message}");
                }
                return 1;
            }
            
            // Process completed
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            
            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine("Output:");
                Console.WriteLine(output);
            }
            
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Error output:");
                Console.WriteLine(error);
            }
            
            return await processTask;
        }
    }
}
