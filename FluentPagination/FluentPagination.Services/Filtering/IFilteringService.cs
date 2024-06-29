using FluentPagination.Models.Filters;

namespace FluentPagination.Services.Filtering;

public interface IFilteringService
{
    IQueryable<TEntity> Filter<TEntity>(IQueryable<TEntity> query, List<FluentFilterModel> filters);
}