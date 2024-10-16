using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.ValueComparers;

public class ExportCitizenReportsFilersValueComparer() : ValueComparer<ExportCitizenReportsFilers>((a, b) =>
        ExportCitizenReportsFilersToJsonConverter.ExportCitizenReportsFilersSerialize(a).Equals(
            ExportCitizenReportsFilersToJsonConverter.ExportCitizenReportsFilersSerialize(b)
        ),
    v => ExportCitizenReportsFilersToJsonConverter.ExportCitizenReportsFilersSerialize(v).GetHashCode());