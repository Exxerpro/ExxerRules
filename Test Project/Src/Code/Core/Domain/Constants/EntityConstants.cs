namespace IndTrace.Domain.Constants;

/// <summary>
/// Contains entity-related constants for consistent naming across the application.
/// </summary>
public static class EntityConstants
{
    /// <summary>
    /// Standard primary key suffix for Entity Framework configurations.
    /// Usage: builder.HasKey(e => e.{nameof(ClassName)}Id);
    /// </summary>
    public const string Id = "Id";
}
