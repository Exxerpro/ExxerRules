using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Nuke.Common.Git;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using Nuke.Common.Tooling;


class Build : NukeBuild
{

    //dotnet tool install --global Nuke.GlobalTool --version 8.1.0
    //nuke --verbosity quiet --all --StartAll -msg "commit message"

    public static int Main() => Execute<Build>(
        x => x.RestartProcess);

    //public static IndTraceOptions TargetsOptions =IndTraceOptions.Communications | IndTraceOptions.VirtualNetwork;
    [Parameter("Select projects to build. Use '--all' to select all projects or list specific projects like '--project Communications --project VirtualNetwork'")]
    readonly IndTraceOptions TargetsOptions = IndTraceOptions.All;

    // Define the message parameter
    [Parameter("Commit message for GitHub push")]
    readonly string Msg;

    //nuke --restart
    [Parameter("Restart closed process")]
    readonly bool Restart;

    //nuke --startAll
    [Parameter("Restart closed process")]
    readonly bool StartAll;


    // Set the verbosity level for the Nuke build
    public new static Verbosity Verbosity => Verbosity.Quiet;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;


    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    // Favicon paths that cause integration test conflicts
    string[] FaviconPaths => new[]
    {
        "Code/Presentation/IndTrace.Components/wwwroot/favicon.ico",
        "Code/Presentation/IndTrace.Intelligence/wwwroot/favicon.ico",
        "Code/Presentation/IndTrace.Monitor/wwwroot/favicon.ico",
        "Code/Presentation/IndTrace.Monitor/wwwroot/img/favicon.ico",
        "Code/Presentation/IndTrace.OEE/wwwroot/favicon.ico"
    };

    DotNetTestSettings settings => new DotNetTestSettings()
        .SetProjectFile(Solution)
        .SetConfiguration(Configuration)
        .SetNoBuild(true)
        .SetNoRestore(true)
        .SetVerbosity(DotNetVerbosity.quiet);

    Target HideFavicons => _ => _
        .Executes(() =>
        {
            Log.Information("🔒 Hiding favicon assets to prevent integration test conflicts...");

            var hiddenCount = 0;
            foreach (var faviconPath in FaviconPaths)
            {
                var fullPath = RootDirectory / faviconPath;
                var backupPath = fullPath + ".bak";

                if (File.Exists(fullPath))
                {
                    if (File.Exists(backupPath))
                    {
                        Log.Debug("  Already hidden: {Path}", faviconPath);
                    }
                    else
                    {
                        Log.Debug("  Hiding: {Path}", faviconPath);
                        File.Move(fullPath, backupPath);
                        hiddenCount++;
                    }
                }
            }

            Log.Information("✅ Successfully hidden {Count} favicon files", hiddenCount);
            Log.Information("Integration tests can now run without static asset conflicts!");
        });

    Target RestoreFavicons => _ => _
        .Executes(() =>
        {
            Log.Information("🔓 Restoring favicon assets for production use...");

            var restoredCount = 0;
            foreach (var faviconPath in FaviconPaths)
            {
                var fullPath = RootDirectory / faviconPath;
                var backupPath = fullPath + ".bak";

                if (File.Exists(backupPath))
                {
                    Log.Debug("  Restoring: {Path}", faviconPath);
                    File.Move(backupPath, fullPath);
                    restoredCount++;
                }
            }

            Log.Information("✅ Successfully restored {Count} favicon files", restoredCount);
            Log.Information("Production favicon assets are ready!");
        });

    Target StopServices => target => target
        .Executes(() =>
        {

            Log.Information("target.Description:{Description}", target.Description);
            Log.Information("Stopping services for solution: {Solution}", Solution.Name);

            string[] serviceNames; // Declare the string array without initializing

            if (TargetsOptions == IndTraceOptions.All)
            {
                serviceNames = new[]
                {
                    "IndTrace.Communications",
                    "IndTrace.VirtualNetwork",
                    "IndTrace.S7Monitor",
                    "IndTrace.Monitor",
                    "IndTrace.Hub.Client",
                    "IndTrace.Hub.Server",
                };
            }
            else
            {
                serviceNames = new[]
                {
                    "IndTrace.Communications",
                    "IndTrace.Monitor",
                };
            }



            foreach (var serviceName in serviceNames)
            {
                try
                {
                    var processes = Process.GetProcessesByName(serviceName);
                    foreach (var process in processes)
                    {
                        Log.Information("Stopping service: {ServiceName} (PID: {Id})", serviceName, process.Id);
                        process.Kill();
                        process.WaitForExit();
                        Log.Information("Service {ServiceName} stopped successfully.", serviceName);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to stop service {serviceName}: {Message}", serviceName, ex.Message);
                }
            }
        });

    Target Clean => target => target
        .DependsOn(StopServices) // Ensure StopServices runs before Clean
        .Executes(() =>
        {
            Log.Information("target.Description: {Target}", target.Description);
            Log.Information("Cleaning packages for solution: {Solution}", Solution.Name);
        });


    Target Restore => target => target
        .DependsOn(Clean)
        .Executes(() =>

        {
            Log.Information("target.Description: {Target}", target.Description);
            Log.Information("Cleaning packages for solution: {Solution}", Solution.Name);

            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .SetVerbosity(DotNetVerbosity.quiet)
            );
        });


    Target Compile => target => target
        .DependsOn(Restore)
        .Executes(() =>
        {

            Log.Information("target.Description: {Target}", target.Description);
            Log.Information("Cleaning packages for solution: {Solution}", Solution.Name);

            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Release)
                .SetVerbosity(DotNetVerbosity.quiet)
            );
        });



    Target Publish => target => target
        .DependsOn(Compile)
        .Executes(() =>
        {

            Log.Information("target.Description: {Target}", target.Description);
            Log.Information("Cleaning packages for solution: {Solution}", Solution.Name);
            var projects = Solution.AllProjects;
            foreach (var project in projects)
            {
                Log.Information("Project Path: {Path}", project.Path);
                Log.Information("Project Name: {Name}", project.Name);
                Log.Information("Project Directory: {Directory}", project.Directory);

            }
            // Check if projects are found
            // Find projects dynamically
            var IndTraceCommunications = projects.FirstOrDefault(p => p.Name.Equals("IndTrace.Communications", StringComparison.OrdinalIgnoreCase));
            var IndTraceVirtualNetwork = projects.FirstOrDefault(p => p.Name.Equals("IndTrace.VirtualNetwork", StringComparison.OrdinalIgnoreCase));
            var IndTraceS7Monitor = projects.FirstOrDefault(p => p.Name.Equals("IndTrace.S7Monitor", StringComparison.OrdinalIgnoreCase));
            var IndTraceMonitor = projects.FirstOrDefault(p => p.Name.Equals("IndTrace.Monitor", StringComparison.OrdinalIgnoreCase));

            var IndTraceHubClient = projects.FirstOrDefault(p => p.Name.Equals("IndTrace.Hub.Client", StringComparison.OrdinalIgnoreCase));
            var IndTraceHubServer = projects.FirstOrDefault(p => p.Name.Equals("IndTrace.Hub.Server", StringComparison.OrdinalIgnoreCase));
            var IndTraceIntelligence = projects.FirstOrDefault(p => p.Name.Equals("IndTrace.Intelligence", StringComparison.OrdinalIgnoreCase));
            var IndTraceOEE = projects.FirstOrDefault(p => p.Name.Equals("IndTrace.OEE", StringComparison.OrdinalIgnoreCase));


            //// Log project paths to ensure correct references
            Log.Information("Project 1 Path: {IndTraceCommunications}", IndTraceCommunications?.Path);
            Log.Information("Project 2 Path: {IndTraceVirtualNetwork}", IndTraceVirtualNetwork?.Path);
            Log.Information("Project 3 Path: {IndTraceS7Monitor}", IndTraceS7Monitor?.Path);
            Log.Information("Project 4 Path: {IndTraceMonitor}", IndTraceMonitor?.Path);
            Log.Information("Project 5 Path: {IndTraceHubClient}", IndTraceHubClient?.Path);
            Log.Information("Project 6 Path: {IndTraceHubServer}", IndTraceHubServer?.Path);
            Log.Information("Project 7 Path: {IndTraceIntelligence}", IndTraceIntelligence?.Path);
            Log.Information("Project 8 Path: {IndTraceOEE}", IndTraceOEE?.Path);

            if (IndTraceCommunications == null)
            {
                Log.Fatal(" IndTrace.Communications  could not be found in the solution.");
                Console.WriteLine(" IndTrace.Communications  could not be found in the solution.");
                throw new NullReferenceException(" IndTrace.Communications  could not be found in the solution.");
            }

            if (IndTraceVirtualNetwork == null)
            {
                Log.Fatal("IndTrace.VirtualNetwork could not be found in the solution.");
                Console.WriteLine("IndTrace.VirtualNetwork could not be found in the solution.");
                throw new NullReferenceException("IndTrace.VirtualNetwork could not be found in the solution.");
            }

            if (IndTraceS7Monitor == null)
            {
                Log.Fatal("IndTrace.S7Monitor  could not be found in the solution.");
                Console.WriteLine("IndTrace.S7Monitor  could not be found in the solution.");
                throw new NullReferenceException("IndTrace.S7Monitor  could not be found in the solution.");

            }

            if (IndTraceMonitor == null)
            {
                Log.Fatal("IndTrace.Monitor  could not be found in the solution.");
                Console.WriteLine("IndTrace.Monitor  could not be found in the solution.");
                throw new NullReferenceException("IndTrace.Monitor  could not be found in the solution.");
            }

            if (IndTraceHubClient == null)
            {
                Log.Fatal("IndTrace.Monitor  could not be found in the solution.");
                Console.WriteLine("IndTrace.Monitor  could not be found in the solution.");
                throw new NullReferenceException("IndTrace.Hub Client  could not be found in the solution.");

            }

            if (IndTraceHubServer == null)
            {
                Log.Fatal("IndTrace.Monitor  could not be found in the solution.");
                Console.WriteLine("IndTrace.Monitor  could not be found in the solution.");
                throw new NullReferenceException("IndTrace.Hub Server  could not be found in the solution.");
            }

            if (IndTraceIntelligence == null)
            {
                Log.Fatal("IndTrace.Intelligence could not be found in the solution.");
                Console.WriteLine("IndTrace.Intelligence could not be found in the solution.");
                throw new NullReferenceException("IndTrace.Intelligence could not be found in the solution.");
            }

            if (IndTraceOEE == null)
            {
                Log.Fatal("IndTrace.OEE could not be found in the solution.");
                Console.WriteLine("IndTrace.OEE could not be found in the solution.");
                throw new NullReferenceException("IndTrace.OEE could not be found in the solution.");
            }


            if (TargetsOptions == IndTraceOptions.All)
            {
                // Publish Project 2
                DotNetPublish(s => s
                    .SetProject(IndTraceVirtualNetwork)
                    .SetConfiguration(Configuration.Release)
                    .SetOutput("publish/IndTrace.VirtualNetwork")
                    .SetVerbosity(DotNetVerbosity.quiet));
                Log.Warning("Published Project: {IndTraceVirtualNetwork} at {Path}", IndTraceVirtualNetwork.Name, IndTraceVirtualNetwork.Path);


                //// Publish Project 3
                DotNetPublish(s => s
                    .SetProject(IndTraceS7Monitor)
                    .SetConfiguration(Configuration.Release)
                    .SetOutput("publish/IndTrace.S7Monitor")
                    .SetVerbosity(DotNetVerbosity.quiet));
                Log.Warning("Published Project: {IndTraceS7Monitor} at {Path}", IndTraceS7Monitor.Name, IndTraceS7Monitor.Path);

                // Publish Intelligence Project
                DotNetPublish(s => s
                    .SetProject(IndTraceIntelligence)
                    .SetConfiguration(Configuration.Release)
                    .SetOutput("publish/IndTrace.Intelligence")
                    .SetVerbosity(DotNetVerbosity.quiet));
                Log.Warning("Published Project: {IndTraceIntelligence} at {Path}", IndTraceIntelligence.Name, IndTraceIntelligence.Path);

                // Publish OEE Project
                DotNetPublish(s => s
                    .SetProject(IndTraceOEE)
                    .SetConfiguration(Configuration.Release)
                    .SetOutput("publish/IndTrace.OEE")
                    .SetVerbosity(DotNetVerbosity.quiet));
                Log.Warning("Published Project: {IndTraceOEE} at {Path}", IndTraceOEE.Name, IndTraceOEE.Path);

            }

            // Publish Project 1
            DotNetPublish(s => s
                .SetProject(IndTraceCommunications)
                .SetConfiguration(Configuration.Release)
                .SetOutput("publish/IndTrace.Communications")
                .SetVerbosity(DotNetVerbosity.quiet));
            Log.Warning("Published Project: {IndTraceCommunications} at {Path}", IndTraceCommunications.Name, IndTraceCommunications.Path);


            // Publish Project 4
            DotNetPublish(s => s
                .SetProject(IndTraceMonitor)
                .SetConfiguration(Configuration.Release)
                .SetOutput("publish/IndTrace.Monitor")
                .SetVerbosity(DotNetVerbosity.quiet));
            Log.Warning("Published Project: {IndTraceS7Monitor} at {Path}", IndTraceS7Monitor.Name, IndTraceS7Monitor.Path);


            // Publish Project 5
            DotNetPublish(s => s
                .SetProject(IndTraceHubClient)
                .SetConfiguration(Configuration.Release)
                .SetOutput("publish/IndTrace.HubClient")
                .SetVerbosity(DotNetVerbosity.quiet));
            Log.Warning("Published Project: {IndTraceHubClient} at {Path}", IndTraceHubClient.Name, IndTraceHubClient.Path);



            // Publish Project 6
            DotNetPublish(s => s
                .SetProject(IndTraceHubServer)
                .SetConfiguration(Configuration.Release)
                .SetOutput("publish/IndTrace.HubServer")
                .SetVerbosity(DotNetVerbosity.quiet));
            Log.Warning("Published Project: {IndTraceHubServer} at {Path}", IndTraceHubServer.Name, IndTraceHubServer.Path);




        });

    Target Push => _ => _
        .DependsOn(Publish) // Ensure the Publish target runs before Push
        .OnlyWhenStatic(() => IsLocalBuild) // Only run this on local builds
        .Executes(() =>
        {
            if (string.IsNullOrWhiteSpace(Msg))
            {
                Log.Information("Src code not committed");
                return;
            }

            var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            var finalCommitMessage = GenerateCommitMessage(Msg, timestamp);

            try
            {
                // Add and commit changes
                Log.Information("Adding changes to git...");
                Git("add .", logOutput: true);

                Log.Information("Committing changes to git with message: {Message}", finalCommitMessage);
                Git($"commit -m \"{finalCommitMessage}\"", logOutput: true);

                Log.Information("Pushing changes to branch {Branch}...", GitRepository.Branch);
                Git($"push origin {GitRepository.Branch}", logOutput: true);
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while executing git commands: {Exception}", ex.Message);
                if (ex.InnerException != null)
                    Log.Error("Inner Exception: {InnerException}", ex.InnerException.Message);
                Log.Error("See above for additional details.");
            }
        });

    private string GenerateCommitMessage(string userMessage, string timestamp)
    {
        try
        {
            Log.Information("🤖 Attempting to generate commit message with GitHub Copilot...");

            // Try GitHub CLI with Copilot extension first
            var copilotMessage = TryGenerateCopilotMessage(userMessage);
            if (!string.IsNullOrEmpty(copilotMessage))
            {
                Log.Information("✅ Generated Copilot commit message successfully");
                return $"{copilotMessage} | nuke {timestamp}";
            }

            // Fallback to user message
            Log.Information("⚠️ Copilot unavailable, using provided message");
            return $"{userMessage} nuke {timestamp}";
        }
        catch (Exception ex)
        {
            Log.Warning("Failed to generate Copilot message: {Exception}", ex.Message);
            return $"{userMessage} nuke {timestamp}";
        }
    }

    private string TryGenerateCopilotMessage(string context)
    {
        try
        {
            // Method 1: Try GitHub CLI with Copilot extension
            Log.Debug("Trying GitHub CLI Copilot...");
            var ghCopilotResult = TryGitHubCliCopilot(context);
            if (!string.IsNullOrEmpty(ghCopilotResult))
                return ghCopilotResult;

            // Method 2: Try VS Code CLI integration
            Log.Debug("Trying VS Code Copilot integration...");
            var vscodeResult = TryVSCodeCopilot(context);
            if (!string.IsNullOrEmpty(vscodeResult))
                return vscodeResult;

            return null;
        }
        catch (Exception ex)
        {
            Log.Debug("Copilot generation failed: {Exception}", ex.Message);
            return null;
        }
    }

    private string TryGitHubCliCopilot(string context)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "gh",
                    Arguments = $"copilot suggest -t git \"commit message for: {context}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            // Timeout after 30 seconds
            if (process.WaitForExit(30000))
            {
                var output = process.StandardOutput.ReadToEnd().Trim();
                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    // Extract actual commit message from Copilot response
                    var lines = output.Split('\n');
                    var commitLine = lines.FirstOrDefault(l => l.StartsWith("git commit -m"));
                    if (commitLine != null)
                    {
                        // Extract message between quotes
                        var match = System.Text.RegularExpressions.Regex.Match(commitLine, @"git commit -m [""'](.*?)[""']");
                        if (match.Success)
                            return match.Groups[1].Value;
                    }
                    return output.Split('\n').FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
                }
            }
            else
            {
                Log.Debug("GitHub CLI Copilot timed out after 30 seconds");
                process.Kill();
            }
        }
        catch (Exception ex)
        {
            Log.Debug("GitHub CLI Copilot failed: {Exception}", ex.Message);
        }
        return null;
    }

    private string TryVSCodeCopilot(string context)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "code",
                    Arguments = "--version", // Test if VS Code CLI is available
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            // Quick timeout for availability check
            if (process.WaitForExit(5000) && process.ExitCode == 0)
            {
                Log.Debug("VS Code CLI available, but commit message generation via CLI is limited");
                // VS Code Copilot commit message generation typically requires interactive UI
                // Could potentially create a temp script or use VS extensions API
                return null;
            }
        }
        catch (Exception ex)
        {
            Log.Debug("VS Code Copilot check failed: {Exception}", ex.Message);
        }
        return null;
    }



    Target RestartProcess => _ => _
        .DependsOn(Push) // Ensure the Publish target runs before Push
        .OnlyWhenStatic(() => IsLocalBuild) // Only run this on local builds
        .Executes(async () =>
        {
            if (!Restart && !StartAll) return;

            Log.Information("Restarting the process...");

            // Execute terminal command at the end of the build
            string command = string.Empty;
            try
            {
                command = "start publish\\IndTrace.VirtualNetwork.exe.lnk";
                Log.Information("Executing terminal command: {Command}", command);
                ProcessTasks.StartProcess("cmd.exe", $"/c {command}").AssertZeroExitCode();

                await Task.Delay(10_000);

                command = "start publish\\IndTrace.Hub.Server.exe.lnk";
                Log.Information("Executing terminal command: {Command}", command);
                ProcessTasks.StartProcess("cmd.exe", $"/c {command}").AssertZeroExitCode();

                if (!StartAll) return;

                await Task.Delay(30_000);

                command = "start publish\\IndTrace.Monitor.exe.lnk";
                Log.Information("Executing terminal command: {Command}", command);
                ProcessTasks.StartProcess("cmd.exe", $"/c {command}").AssertZeroExitCode();


                await Task.Delay(30_000);

                command = "start publish\\IndTrace.Communications.exe.lnk";
                Log.Information("Executing terminal command: {Command}", command);
                ProcessTasks.StartProcess("cmd.exe", $"/c {command}").AssertZeroExitCode();


            }
            catch (Exception ex)
            {

                Log.Error("An error occurred while executing git the terminal command: {Exception}", ex.Message);
                if (ex.InnerException != null)
                    Log.Error("Inner Exception: {InnerException}", ex.InnerException.Message);
                Log.Error("See above for additional details.");

            }
        });


}
