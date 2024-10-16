using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.ValueComparers;

public class ExportFormSubmissionsFiltersValueComparer() : ValueComparer<ExportFormSubmissionsFilters>((a, b) =>
        ExportFormSubmissionsFiltersToJsonConverter.ExportFormSubmissionsFiltersSerialize(a).Equals(
            ExportFormSubmissionsFiltersToJsonConverter.ExportFormSubmissionsFiltersSerialize(b)
        ),
    v => ExportFormSubmissionsFiltersToJsonConverter.ExportFormSubmissionsFiltersSerialize(v).GetHashCode());