// <copyright file="ConfigAppsDto.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Queries.GetConfigAppsList;

/// <summary>
/// Represents the ConfigAppsDto.
/// </summary>
public class ConfigAppsDto
{
    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string ConfigAppId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AppId.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Client { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Factory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Line { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Project { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the VersionDate.
    /// </summary>
    public DateTime VersionDate { get; set; }

    /// <summary>
    /// Gets or sets the ModifiedDate.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<ConfigAppsDto> ToDto(ConfigApp src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<ConfigAppsDto>.WithFailure("ConfigApp source cannot be null");
        }

        return IndQuestResults.Result<ConfigAppsDto>.Success(new ConfigAppsDto
        {
            ConfigAppId = src.ConfigAppId,
            AppId = src.AppId,
            Client = src.Client,
            Factory = src.Factory,
            Line = src.Line,
            MachineId = src.MachineId,
            Project = src.Project,
            Version = src.Version,
            VersionDate = src.CreatedOn ?? DateTime.Now,
            ModifiedDate = src.ModifiedOn ?? DateTime.Now,
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<ConfigApp> ToEntity(ConfigAppsDto src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<ConfigApp>.WithFailure("ConfigAppsDto source cannot be null");
        }

        return IndQuestResults.Result<ConfigApp>.Success(new ConfigApp
        {
            ConfigAppId = src.ConfigAppId,
            AppId = src.AppId,
            Client = src.Client,
            Factory = src.Factory,
            Line = src.Line,
            MachineId = src.MachineId,
            Project = src.Project,
            Version = src.Version,
            CreatedOn = src.VersionDate,
            ModifiedOn = src.ModifiedDate,
        });
    }

    /// <summary>
    /// Executes ToDtoList operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDtoList.</returns>
    public static IndQuestResults.Result<List<ConfigAppsDto>> ToDtoList(IEnumerable<ConfigApp> src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<List<ConfigAppsDto>>.WithFailure("ConfigApp collection cannot be null");
        }

        var list = src.Select(s => new ConfigAppsDto
        {
            ConfigAppId = s.ConfigAppId,
            AppId = s.AppId,
            Client = s.Client,
            Factory = s.Factory,
            Line = s.Line,
            MachineId = s.MachineId,
            Project = s.Project,
            Version = s.Version,
            VersionDate = s.CreatedOn ?? DateTime.Now,
            ModifiedDate = s.ModifiedOn ?? DateTime.Now,
        }).ToList();
        return IndQuestResults.Result<List<ConfigAppsDto>>.Success(list);
    }
}
