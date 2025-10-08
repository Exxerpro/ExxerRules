// <copyright file="ShiftRepositoryExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repositories;

/// <summary>
/// Provides extension methods for <see cref="IRepository{Shift}"/> to support common shift queries and operations.
/// </summary>
public static class ShiftRepositoryExtensions
{
    /// <summary>
    /// Gets the shift entity for the current date and time using the provided date/time provider.
    /// </summary>
    /// <param name="shiftRepository">The shift repository.</param>
    /// <param name="dateTimeMachine">The date/time provider.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The shift entity if found, or a failure result.</returns>
    public static async Task<Result<Shift>> GetShiftByDateAsync(
        this IRepository<Shift> shiftRepository,
        IDateTimeMachine dateTimeMachine,
        CancellationToken cancellationToken)
    {
        dateTimeMachine ??= new DateTimeMachine();
        var timeShift = dateTimeMachine.Now.ToLocalTime();

        try
        {
            var spec = new Specification<Shift>(b => b.StartBy <= timeShift && b.EndTime >= timeShift);
            var shift = await shiftRepository.FirstOrDefaultAsync(spec, cancellationToken).ConfigureAwait(false);
            if (shift.IsFailure)
            {
                return Result<Shift>.WithFailure(shift.Errors);
            }

            if (shift.Value is null)
            {
                return Result<Shift>.WithFailure("Shift not found for current time");
            }

            return Result<Shift>.Success(shift.Value);
        }
        catch (Exception e)
        {
            return Result<Shift>.WithFailure($"Error while retrieving shift {e}");
        }
    }
}
