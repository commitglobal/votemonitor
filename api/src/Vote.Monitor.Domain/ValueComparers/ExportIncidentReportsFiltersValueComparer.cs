using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.ValueComparers;

public class ExportIncidentReportsFiltersValueComparer() : ValueComparer<ExportIncidentReportsFilters>((a, b) =>
        ExportIncidentReportsFiltersToJsonConverter.ExportIncidentReportsFiltersSerialize(a).Equals(
            ExportIncidentReportsFiltersToJsonConverter.ExportIncidentReportsFiltersSerialize(b)
        ),
    v => ExportIncidentReportsFiltersToJsonConverter.ExportIncidentReportsFiltersSerialize(v).GetHashCode());