namespace FluentPagination.Models;

using FluentPagination.Models.Filters;
using FluentPagination.Models.Sorters;

public class FluentPaginationRequestModel : IFluentPaginationRequestModel
{
    public IEnumerable<FluentFilterModel> FilterModel { get; set; }

    public IEnumerable<FluentSortingModel> SortingModel { get; set; }

    public int ItemsPerPage { get; set; } = 10;

    public int TotalCount { get; set; }

    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }
}