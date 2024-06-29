namespace FluentPagination.Models.Enums;

/// <summary>
/// Filtering operations.
/// </summary>
public enum FilteringOperation
{
    None = 0,
    Equal = 1,
    NotEqual = 2,
    GreaterThan = 3,
    LessThan = 4,
    GreaterThanOrEqual = 5,
    LessThanOrEqual = 6,
    Contains = 7,
    StartsWith = 8,
    EndsWith = 9,
    IsNull = 10,
    IsTrue = 11,
    IsFalse = 12,
}