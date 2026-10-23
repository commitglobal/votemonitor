using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Hangfire.Jobs.Export.IncidentReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.IncidentReports;

public class ExportIncidentReportsJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ILogger<ExportIncidentReportsJob> logger,
    ITimeProvider timeProvider) : IExportIncidentReportsJob
{
    public async Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct)
    {
        var exportedData = await context
            .ExportedData
            .Where(x => x.Id == exportedDataId)
            .FirstOrDefaultAsync(ct);

        if (exportedData == null)
        {
            logger.LogWarning("ExportData was not found for {electionRoundId}  {exportedDataId}",
                electionRoundId, exportedDataId);
            throw new ExportedDataWasNotFoundException(ExportedDataType.IncidentReports, electionRoundId,
                exportedDataId);
        }

        try
        {
            if (exportedData.ExportStatus == ExportedDataStatus.Completed ||
                exportedData.ExportStatus == ExportedDataStatus.Failed)
            {
                logger.LogWarning("ExportData was completed or failed for {electionRoundId} {ngoId} {exportedDataId}",
                    electionRoundId, ngoId, exportedDataId);
                return;
            }

            var utcNow = timeProvider.UtcNow;

            var incidentReportingForm = await context
                .Forms
                .Where(x => x.ElectionRoundId == electionRoundId
                            && x.MonitoringNgo.NgoId == ngoId
                            && x.Status != FormStatus.Drafted
                            && x.FormType == FormType.IncidentReporting)
                .AsNoTracking()
                .ToListAsync(ct);

            var filters = exportedData.IncidentReportsFilters ?? new ExportIncidentReportsFilters();
            var incidentReports = await GetIncidentReports(
                electionRoundId, ngoId, filters, exportedData.FilterConditions, ct);

            foreach (var incidentReport in incidentReports)
            {
                var presignedAttachments = await fileStorageService.PresignAsync(incidentReport.Attachments);
                Array.Copy(presignedAttachments, incidentReport.Attachments, presignedAttachments.Length);
            }

            var excelFileGenerator = ExcelFileGenerator.New();

            for (var index = 0; index < incidentReportingForm.Count; index++)
            {
                var form = incidentReportingForm[index];
                var sheetData = IncidentReportsDataTable
                    .FromForm(form)
                    .WithData()
                    .For(incidentReports)
                    .Please();

                excelFileGenerator.WithSheet((index + 1) + "-" + form.Code, sheetData.header, sheetData.dataTable);
            }

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"incident-reports-{utcNow:yyyyMMdd_HHmmss}.xlsx";
            exportedData.Complete(fileName, base64EncodedData, utcNow);

            await context.SaveChangesAsync(ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when exporting data");
            exportedData.Fail();
            await context.SaveChangesAsync(ct);

            throw;
        }
    }

    private async Task<IncidentReportModel[]> GetIncidentReports(Guid electionRoundId, Guid ngoId,
        ExportIncidentReportsFilters filters, System.Text.Json.JsonDocument? filterConditions, CancellationToken ct)
    {
        var filter = new FilterSqlCompiler(V2ReportFilterFields.IncidentReports)
            .Build(FilterRuleJson.Deserialize(filterConditions));
        var queryParams = new DynamicParameters();
        queryParams.Add("electionRoundId", electionRoundId);
        queryParams.Add("ngoId", ngoId);
        queryParams.Add("dataSource", filters.DataSource.ToString());
        queryParams.Add("coalitionMemberId", filters.CoalitionMemberId);
        queryParams.Add("searchText", $"%{filters.SearchText?.Trim() ?? string.Empty}%");
        queryParams.Add("locationType", filters.LocationType?.ToString());
        queryParams.Add("level1", filters.Level1Filter);
        queryParams.Add("level2", filters.Level2Filter);
        queryParams.Add("level3", filters.Level3Filter);
        queryParams.Add("level4", filters.Level4Filter);
        queryParams.Add("level5", filters.Level5Filter);
        queryParams.Add("pollingStationNumber", filters.PollingStationNumberFilter);
        queryParams.Add("pollingStationId", filters.PollingStationId);
        queryParams.Add("hasFlaggedAnswers", filters.HasFlaggedAnswers);
        queryParams.Add("followUpStatus", filters.FollowUpStatus?.ToString());
        queryParams.Add("tagsFilter", filters.TagsFilter ?? []);
        queryParams.Add("monitoringObserverStatus", filters.MonitoringObserverStatus?.ToString());
        queryParams.Add("formId", filters.FormId);
        queryParams.Add("formStatus", filters.FormStatusFilter?.ToString());
        queryParams.Add("hasNotes", filters.HasNotes);
        queryParams.Add("hasAttachments", filters.HasAttachments);
        queryParams.Add("questionsAnswered", filters.QuestionsAnswered?.ToString());
        queryParams.Add("fromDate", filters.FromDateFilter?.ToString("O"));
        queryParams.Add("toDate", filters.ToDateFilter?.ToString("O"));
        queryParams.Add("isCompleted", filters.IsCompletedFilter);
        filter.AddTo(queryParams);

        var sql = $"""
            SELECT r.*
            FROM "ExportGetIncidentReports"(
                @electionRoundId, @ngoId, @dataSource, @coalitionMemberId, @searchText, @locationType,
                @level1, @level2, @level3, @level4, @level5, @pollingStationNumber, @pollingStationId,
                @hasFlaggedAnswers, @followUpStatus, @tagsFilter, @monitoringObserverStatus, @formId, @formStatus,
                @hasNotes, @hasAttachments, @questionsAnswered, @fromDate, @toDate, @isCompleted) r
            WHERE r."IncidentReportId" IN (
                SELECT s."Id"
                FROM "GetIncidentReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                WHERE s."ElectionRoundId" = @electionRoundId
                  AND {filter.Sql}
            )
            ORDER BY r."TimeSubmitted" DESC;
            """;

        IEnumerable<IncidentReportModel> incidentReports;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            incidentReports = await dbConnection.QueryAsync<IncidentReportModel>(sql, queryParams);
        }

        var incidentReportsData = incidentReports.ToArray();
        return incidentReportsData;
    }
}
