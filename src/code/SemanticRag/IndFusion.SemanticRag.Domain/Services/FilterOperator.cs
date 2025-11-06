namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Filter operators for advanced search.
/// </summary>
public enum FilterOperator
{
    /// <summary>
    /// Equals.
    /// </summary>
    Equals,

    /// <summary>
    /// Not equals.
    /// </summary>
    NotEquals,

    /// <summary>
    /// Contains.
    /// </summary>
    Contains,

    /// <summary>
    /// Greater than.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Less than.
    /// </summary>
    LessThan,

    /// <summary>
    /// In list.
    /// </summary>
    In,

    /// <summary>
    /// Not in list.
    /// </summary>
    NotIn
}