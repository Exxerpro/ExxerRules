// <copyright file="JsonLoader.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Files;

using System.IO;
using System.Text.Json;

/// <summary>
/// Provides methods to load JSON files from a specified source folder.
/// </summary>
public static class JsonLoader
{
    /// <summary>
    /// Loads a JSON file from the specified source folder and deserializes it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content to.</typeparam>
    /// <param name="folderPath">The name of the folder to search for the JSON file.</param>
    /// <param name="fileName">The name of the JSON file to load.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the JSON file is not found.</exception>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate file name inputs defensively to avoid path traversal or invalid characters. See .NET best practices for file handling.
    public static IndQuestResults.Result<T> LoadJsonFromSrcFolder<T>(string folderPath, string fileName)
    {
        var srcFolderRes = FindSrcFolder(folderPath);
        if (srcFolderRes.IsFailure)
        {
            return IndQuestResults.Result<T>.WithFailure(srcFolderRes.Errors);
        }

        if (srcFolderRes.Value is null)
        {
            return IndQuestResults.Result<T>.WithFailure("Source folder path is null");
        }

        var filePathRes = FindJsonFile(srcFolderRes.Value, fileName);

        if (filePathRes.IsFailure)
        {
            return IndQuestResults.Result<T>.WithFailure(filePathRes.Errors);
        }

        if (filePathRes.Value == null)
        {
            return IndQuestResults.Result<T>.WithFailure($"JSON file '{fileName}' not found in the '{folderPath}' folder or its subdirectories.");
        }

        var jsonContent = File.ReadAllText(filePathRes.Value);
        var obj = JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        return obj is null
            ? IndQuestResults.Result<T>.WithFailure("Failed to deserialize JSON content")
            : IndQuestResults.Result<T>.Success(obj);
    }

    private static IndQuestResults.Result<string> FindSrcFolder(string folderPath)
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var directoryInfo = new DirectoryInfo(currentDirectory);

        while (directoryInfo != null && !directoryInfo.Name.Equals(folderPath, StringComparison.OrdinalIgnoreCase))
        {
            directoryInfo = directoryInfo.Parent;
        }

        if (directoryInfo == null)
        {
            return IndQuestResults.Result<string>.WithFailure($"Could not find the {folderPath} folder in the path.");
        }

        return IndQuestResults.Result<string>.Success(directoryInfo.FullName);
    }

    private static IndQuestResults.Result<string?> FindJsonFile(string folderPath, string fileName)
    {
        try
        {
            var jsonFiles = Directory.GetFiles(folderPath, "*.json", SearchOption.AllDirectories);
            var foundFile = jsonFiles.FirstOrDefault(file => Path.GetFileName(file).Equals(fileName, StringComparison.OrdinalIgnoreCase));
            return IndQuestResults.Result<string?>.Success(foundFile);
        }
        catch (Exception ex)
        {
            return IndQuestResults.Result<string?>.WithFailure($"Error searching for JSON file '{fileName}' in folder '{folderPath}': {ex.Message}");
        }
    }

    // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated file name parsing or validation logic. Refactor for maintainability if necessary.
}
