using System.Linq.Expressions;
using FluentPagination.Models.Enums;
using FluentPagination.Models.Filters;

namespace FluentPagination.Services.Filtering;

public class FilteringService: IFilteringService
{
    private static readonly string NullValue = "null";
    
    public virtual IQueryable<TEntity> Filter<TEntity>(IQueryable<TEntity> query, List<FluentFilterModel> filters)
    {
        if (!filters.Any())
        {
            return query;
        }
        
        var mappedQuery = query;

        foreach (var filter in filters)
        {
            var expression = BuildFilteringExpression<TEntity>(filter);
            mappedQuery = mappedQuery.Where(expression);
        }

        return mappedQuery;
    }

      private static Expression<Func<T, bool>> BuildFilteringExpression<T>(FluentFilterModel filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, filter.Property);

        Expression? expression = null;
        if (filter.Property.PropertyType.IsEnum)
        {
            expression = BuildEnumExpression(filter.Value, filter.Property.PropertyType, property);
        }
        else if (filter.Property.PropertyType == typeof(string))
        {
            expression = BuildStringExpression(filter.FilteringOperation, filter.Value, property);
        }
        else if (filter.Property.PropertyType == typeof(bool))
        {
            expression = BuildBooleanExpression(filter.FilteringOperation, filter.Value, property);
        }
        else if (filter.Property.PropertyType == typeof(int) ||
                 Nullable.GetUnderlyingType(filter.Property.PropertyType) == typeof(int) ||
                 filter.Property.PropertyType == typeof(double) ||
                 Nullable.GetUnderlyingType(filter.Property.PropertyType) == typeof(double))
        {
            var propertyType = filter.Property.PropertyType;
            expression = BuildNumberExpression(filter.FilteringOperation, filter.Value, property, propertyType);
        }
        else if (filter.Property.PropertyType == typeof(DateTime) ||
                 Nullable.GetUnderlyingType(filter.Property.PropertyType) == typeof(DateTime))
        {
            expression = BuildDateTimeExpression(filter.FilteringOperation, filter.Value, property);
        }

        if (expression == null)
        {
            throw new InvalidOperationException("Expression cannot be build");
        }

        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private static Expression BuildEnumExpression(string filterValue, Type propertyType, MemberExpression property)
    {
        if (!Enum.TryParse(propertyType, filterValue, ignoreCase: true, out object? enumValue))
        {
            throw new ArgumentException($"Invalid enum value '{filterValue}' for enum type {propertyType.Name}");
        }

        var constant = Expression.Constant(enumValue, propertyType);
        return Expression.Equal(property, constant);
    }

    private static Expression? BuildNumberExpression(FilteringOperation operatorType, string? value, MemberExpression property, Type? propertyType)
    {
        Expression? expression;

        if (value == null || value.Equals(NullValue, StringComparison.OrdinalIgnoreCase))
        {
            if (!IsNullableType(property.Type))
            {
                throw new ArgumentException($"Cannot assign null to a non-nullable integer property: {property.Member.Name}");
            }

            expression = GetNullableTypesOperation(property, operatorType);
        }
        else if (int.TryParse(value, out var intValue) && (propertyType == typeof(int) || propertyType == typeof(int?)))
        {
            var constant = Expression.Constant(intValue, IsNullableType(property.Type) ? typeof(int?) : typeof(int));
            expression = GetNumberOperation(property, constant, operatorType);
        }
        else if (double.TryParse(value, out var doubleValue) && propertyType == typeof(double))
        {
            var constant = Expression.Constant(doubleValue, IsNullableType(property.Type) ? typeof(double?) : typeof(double));
            expression = GetNumberOperation(property, constant, operatorType);
        }
        else
        {
            throw new ArgumentException($"Invalid value for integer property: {value}");
        }

        return expression;
    }

    private static Expression? BuildDateTimeExpression(FilteringOperation operatorType, string? value, MemberExpression property)
    {
        Expression? expression;

        if (value == null || value.Equals(NullValue, StringComparison.OrdinalIgnoreCase))
        {
            if (!IsNullableType(property.Type))
            {
                throw new ArgumentException($"Cannot assign null to a non-nullable integer property: {property.Member.Name}");
            }

            expression = GetNullableTypesOperation(property, operatorType);
        }
        else if (DateTime.TryParse(value, out var dateTimeValue))
        {
            var constant = Expression.Constant(dateTimeValue, IsNullableType(property.Type) ? typeof(DateTime?) : typeof(DateTime));
            switch (operatorType)
            {
                case FilteringOperation.Equal:
                    expression = Expression.Equal(property, constant);
                    break;
                case FilteringOperation.GreaterThan:
                    expression = Expression.GreaterThan(property, constant);
                    break;
                case FilteringOperation.LessThan:
                    expression = Expression.LessThan(property, constant);
                    break;
                case FilteringOperation.LessThanOrEqual:
                    expression = Expression.LessThanOrEqual(property, constant);
                    break;
                case FilteringOperation.GreaterThanOrEqual:
                    expression = Expression.GreaterThanOrEqual(property, constant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Property of type int cannot have {operatorType} operator");
            }
        }
        else
        {
            throw new ArgumentException($"Invalid value for integer property: {value}");
        }

        return expression;
    }

    private static Expression? BuildBooleanExpression(FilteringOperation operatorType, string value, MemberExpression property)
    {
        Expression? expression;
        if (!bool.TryParse(value, out var boolValue))
        {
            throw new ArgumentException($"Invalid value for boolean property: {value}");
        }

        var constant = Expression.Constant(boolValue);
        switch (operatorType)
        {
            case FilteringOperation.Equal:
                expression = Expression.Equal(property, constant);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"Property of type bool cannot have {operatorType} operator");
        }

        return expression;
    }

    private static Expression? BuildStringExpression(FilteringOperation operatorType, string value, MemberExpression property)
    {
        Expression? expression;
        var constant = Expression.Constant(value, typeof(string));
        switch (operatorType)
        {
            case FilteringOperation.Equal:
                expression = Expression.Equal(property, constant);
                break;
            case FilteringOperation.Contains:
                expression = Expression.Call(
                    property,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    constant);
                break;
            case FilteringOperation.StartsWith:
                expression = Expression.Call(
                    property,
                    typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!,
                    constant);
                break;
            case FilteringOperation.EndsWith:
                expression = Expression.Call(
                    property,
                    typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!,
                    constant);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"Property of type string: cannot have {operatorType} operator");
        }

        return expression;
    }

    private static Expression GetNumberOperation(MemberExpression property,  ConstantExpression constant,  FilteringOperation operatorType)
    {
        switch (operatorType)
        {
            case FilteringOperation.Equal:
              return Expression.Equal(property, constant);
            case FilteringOperation.GreaterThan:
              return Expression.GreaterThan(property, constant);
            case FilteringOperation.LessThan:
              return Expression.LessThan(property, constant);
            case FilteringOperation.LessThanOrEqual:
              return Expression.LessThanOrEqual(property, constant);
            case FilteringOperation.GreaterThanOrEqual:
              return Expression.GreaterThanOrEqual(property, constant);
            default:
              throw new ArgumentOutOfRangeException(
                    $"Property of type int cannot have {operatorType} operator");
        }
    }

    private static Expression GetNullableTypesOperation(MemberExpression property,  FilteringOperation operatorType)
    {
        switch (operatorType)
            {
                case FilteringOperation.Equal:
                    return Expression.Equal(property, Expression.Constant(null, property.Type));
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Property of type int? cannot have {operatorType} operator");
            }
    }

    private static bool IsNullableType(Type type)
        => Nullable.GetUnderlyingType(type) != null || !type.IsValueType;
}