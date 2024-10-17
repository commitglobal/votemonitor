using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.ValueComparers;

public class ExportQuickReportsFiltersValueComparer() : ValueComparer<ExportQuickReportsFilters>((a, b) =>
        ExportQuickReportsFiltersToJsonConverter.ExportQuickReportsFiltersSerialize(a).Equals(
            ExportQuickReportsFiltersToJsonConverter.ExportQuickReportsFiltersSerialize(b)
        ),
    v => ExportQuickReportsFiltersToJsonConverter.ExportQuickReportsFiltersSerialize(v).GetHashCode());