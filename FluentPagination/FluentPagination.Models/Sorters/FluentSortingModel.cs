namespace FluentPagination.Models.Sorters;

using FluentPagination.Models.Enums;

public class FluentSortingModel
{
    public string SortingProperty { get; set; }

    public SortingOperation SortingOperation { get; set; }
}