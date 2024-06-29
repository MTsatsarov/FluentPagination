namespace FluentPagination.Models;

using FluentPagination.Models.Filters;
using FluentPagination.Models.Sorters;

public interface IFluentPaginationRequestModel
{
    IEnumerable<FluentFilterModel> FilterModel { get; set; }

    IEnumerable<FluentSortingModel> SortingModel { get; set; }

    int ItemsPerPage { get; set; }

    int TotalCount { get; set; }

    int PageNumber { get; set; }

    int TotalPages { get; set; }
}