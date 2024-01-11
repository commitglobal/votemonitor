using Ardalis.Specification;
using Vote.Monitor.Core.Entities;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Extensions;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<T> Paginate<T>(this ISpecificationBuilder<T> builder, BaseFilterRequest filter) where T : class, IAggregateRoot
    {
        return builder.Skip(PaginationHelper.CalculateSkip(filter.PageSize, filter.PageNumber))
             .Take(PaginationHelper.CalculateTake(filter.PageSize));
    }
}
