namespace IndTrace.Aggregation.BoundedTests.Services;
/// <summary>
/// Represents the TesterUserService.
/// </summary>

public class TesterUserService : IIndTraceUserService
{
    /// <summary>
    /// Gets or sets the CurrentUserId.
    /// </summary>
    public Task<string> CurrentUserId { get; } = Task.FromResult("Admin");
    /// <summary>
    /// Gets or sets the CurrentUserName.
    /// </summary>
    public Task<string> CurrentUserName { get; } = Task.FromResult("Admin");
    /// <summary>
    /// Gets or sets the IsAuthenticated.
    /// </summary>
    public Task<bool> IsAuthenticated { get; } = Task.FromResult(true);
}
