namespace FluentPagination.Models;

using FluentPagination.Models.Filters;

public interface IFluentPaginationRequestModel
{
    public FluentFilterModel FilterModel { get; set; }
}