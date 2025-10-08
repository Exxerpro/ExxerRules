// <copyright file="ShiftDetailVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Queries.GetShftDetail;

/// <summary>
/// Represents the ShiftDetailVm.
/// </summary>
public class ShiftDetailVm
{
    /// <summary>
    /// Gets or sets the ShiftId.
    /// </summary>
    public int ShiftId { get; set; }

    /// <summary>
    /// Gets or sets the PartNumber.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: Pattern 17 Fix - Constructor property should be initialized with = null!, not string.Empty to match test expectations
    public string PartNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ShiftName.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: Pattern 17 Fix - Constructor property should be initialized with = null!, not string.Empty to match test expectations
    public string ShiftName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the IsActive.
    /// </summary>
    public int IsActive { get; set; }

    /// <summary>
    /// Gets or sets the Version.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the CustomerPartNumber.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: Pattern 17 Fix - Constructor property should be initialized with = null!, not string.Empty to match test expectations
    public string CustomerPartNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the AliasPartNumber.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: Pattern 17 Fix - Constructor property should be initialized with = null!, not string.Empty to match test expectations
    public string AliasPartNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Description.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: Pattern 17 Fix - Constructor property should be initialized with = null!, not string.Empty to match test expectations
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CreatedBy.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: Pattern 17 Fix - Constructor property should be initialized with = null!, not string.Empty to match test expectations
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ModifiedBy.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 22/08/2025
    // Reason: Pattern 17 Fix - Constructor property should be initialized with = null!, not string.Empty to match test expectations
    public string ModifiedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CreatedOn.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the ModifiedOn.
    /// </summary>
    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// Converts a Shift entity to ShiftDetailVm.
    /// </summary>
    /// <param name="src">The source Shift entity.</param>
    /// <returns>A Result containing the ShiftDetailVm or failure information.</returns>
    public static Result<ShiftDetailVm> ToDto(Shift src)
    {
        if (src == null)
        {
            return Result<ShiftDetailVm>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        return Result<ShiftDetailVm>.Success(new ShiftDetailVm
        {
            ShiftId = src.ShiftId,
            ShiftName = src.ShiftType ?? string.Empty,
            CreatedOn = src.CreatedOn ?? DateTime.MinValue,
            ModifiedOn = src.ModifiedOn ?? DateTime.MinValue,

            // Only map properties that exist in both Shift and ShiftDetailVm
        });
    }
}
