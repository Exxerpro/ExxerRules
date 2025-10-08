using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace IndTrace.Persistence.Helper;

public static class SqlScriptRunner
{
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate SQL script runner logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static void ApplyScriptsFrom(string folderPath, DbContext context)
    {
        var files = Directory.GetFiles(folderPath, "*.sql", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var sql = File.ReadAllText(file);
            context.Database.ExecuteSqlRaw(sql); // or ExecuteSqlInterpolated if needed
            Console.WriteLine($"✔ Executed: {Path.GetFileName(file)}");
        }
    }
}
