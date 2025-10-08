// <copyright file="EmbeddedResourceTest.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools.Test;

using System.Reflection;
using IndTrace.FilesTools.Services;

/// <summary>
/// Quick test class to verify embedded resources are working.
/// This will be removed once we integrate with the main test suite.
/// </summary>
public static class EmbeddedResourceTest
{
    /// <summary>
    /// Tests if the embedded resources are accessible and working.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<bool> TestEmbeddedResourcesAsync()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            Console.WriteLine($"🔍 Found {resourceNames.Length} embedded resources:");
            foreach (var name in resourceNames)
            {
                Console.WriteLine($"   - {name}");
            }

            // Test the file locator
            var locator = FileLocatorFactory.CreateEmbeddedResourceLocator();
            var embeddedLocator = (Strategies.EmbeddedResourceFileLocator)locator;
            var diagnostics = embeddedLocator.GetDiagnosticInfo();

            Console.WriteLine($"\n🔍 File Locator Diagnostics:");
            foreach (var kvp in diagnostics)
            {
                Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
            }

            // Test loading Rules.json
            var rulesExist = await locator.ExistsAsync("Rules.json");
            Console.WriteLine($"\n🔍 Rules.json exists: {rulesExist}");

            if (rulesExist)
            {
                var rulesContent = await locator.GetFileContentAsync("Rules.json");
                var contentLength = rulesContent?.Length ?? 0;
                Console.WriteLine($"🔍 Rules.json content length: {contentLength} characters");

                if (contentLength > 0)
                {
                    Console.WriteLine($"🔍 Rules.json preview: {rulesContent!.Substring(0, Math.Min(100, contentLength))}...");
                }
            }

            return rulesExist && resourceNames.Length > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test failed: {ex.Message}");
            return false;
        }
    }
}
