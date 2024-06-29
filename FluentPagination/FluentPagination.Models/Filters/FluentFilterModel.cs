namespace FluentPagination.Models.Filters;

using FluentPagination.Models.Enums;

/// <summary>
/// A model which will represent the filter from the request.
/// </summary>
public class FluentFilterModel
{
    public string? RequestFilterProperty { get; set; }

    public FilteringOperation FilteringOperation { get; set; }

    public string? EntityProperty { get; set; }
}