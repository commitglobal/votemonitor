using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Queries;

namespace Feature.DataExport.StartV2;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ExportedDataType).NotEmpty();

        RuleFor(x => x.Filter).Custom((filter, context) =>
        {
            if (filter is null)
            {
                return;
            }

            var fields = GetFilterFields(context.InstanceToValidate.ExportedDataType);
            if (fields is null)
            {
                context.AddFailure("Filtering is not supported for this export type.");
                return;
            }

            // Compile up front so an invalid filter is rejected here instead of failing the export job.
            try
            {
                new FilterSqlCompiler(fields).Build(filter);
            }
            catch (ArgumentException e)
            {
                context.AddFailure(e.Message);
            }
        });
    }

    private static IReadOnlyDictionary<string, FilterField>? GetFilterFields(ExportedDataType? exportedDataType)
    {
        if (exportedDataType == ExportedDataType.FormSubmissions
            || exportedDataType == ExportedDataType.FormSubmissionsSimplified)
        {
            return V2ReportFilterFields.FormSubmissions;
        }

        if (exportedDataType == ExportedDataType.QuickReports)
        {
            return V2ReportFilterFields.QuickReports;
        }

        if (exportedDataType == ExportedDataType.CitizenReports)
        {
            return V2ReportFilterFields.CitizenReports;
        }

        if (exportedDataType == ExportedDataType.IncidentReports)
        {
            return V2ReportFilterFields.IncidentReports;
        }

        return null;
    }
}
