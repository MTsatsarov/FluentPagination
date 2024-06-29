namespace FluentPagination.Models.Filters;

using System.Reflection;
using FluentPagination.Models.Enums;

/// <summary>
/// A model which will represent the filter from the request.
/// </summary>
public class FluentFilterModel
{
    public PropertyInfo Property { get; set; }

    public FilteringOperation FilteringOperation { get; set; }

    public string Value { get; set; }
}