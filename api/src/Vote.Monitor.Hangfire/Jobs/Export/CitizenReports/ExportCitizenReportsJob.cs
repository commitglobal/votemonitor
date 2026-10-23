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
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Hangfire.Jobs.Export.CitizenReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.CitizenReports;

public class ExportCitizenReportsJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ILogger<ExportCitizenReportsJob> logger,
    ITimeProvider timeProvider) : IExportCitizenReportsJob
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
            throw new ExportedDataWasNotFoundException(ExportedDataType.CitizenReports, electionRoundId,
                exportedDataId);
        }

        try
        {
            if (exportedData.ExportStatus == ExportedDataStatus.Completed)
            {
                logger.LogWarning("ExportData was completed for {electionRoundId} {ngoId} {exportedDataId}",
                    electionRoundId, ngoId, exportedDataId);
                return;
            }

            var utcNow = timeProvider.UtcNow;

            var citizenReportingForm = await context
                .Forms
                .Where(x => x.ElectionRoundId == electionRoundId
                            && x.MonitoringNgo.NgoId == ngoId
                            && x.Status != FormStatus.Drafted
                            && x.FormType == FormType.CitizenReporting
                            && x.ElectionRound.CitizenReportingEnabled
                            && x.ElectionRound.MonitoringNgoForCitizenReporting.NgoId == ngoId)
                .AsNoTracking()
                .ToListAsync(ct);

            var citizenReports = await GetCitizenReports(
                electionRoundId, ngoId, exportedData.CitizenReportsFilers, exportedData.FilterConditions, ct);

            foreach (var citizenReport in citizenReports)
            {
                var presignedAttachments = await fileStorageService.PresignAsync(citizenReport.Attachments);
                Array.Copy(presignedAttachments, citizenReport.Attachments, presignedAttachments.Length);
            }

            var excelFileGenerator = ExcelFileGenerator.New();

            for (var index = 0; index < citizenReportingForm.Count; index++)
            {
                var form = citizenReportingForm[index];
                var sheetData = CitizenReportsDataTable
                    .FromForm(form)
                    .WithData()
                    .For(citizenReports)
                    .Please();

                excelFileGenerator.WithSheet((index + 1) + "-" + form.Code, sheetData.header, sheetData.dataTable);
            }

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"citizen-reports-{utcNow:yyyyMMdd_HHmmss}.xlsx";
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

    private async Task<CitizenReportModel[]> GetCitizenReports(Guid electionRoundId, Guid ngoId,
        ExportCitizenReportsFilers? filters, System.Text.Json.JsonDocument? filterConditions, CancellationToken ct)
    {
        var filter = new FilterSqlCompiler(V2ReportFilterFields.CitizenReports)
            .Build(FilterRuleJson.Deserialize(filterConditions));
        var queryParams = new DynamicParameters();
        queryParams.Add("electionRoundId", electionRoundId);
        queryParams.Add("ngoId", ngoId);
        queryParams.Add("hasFlaggedAnswers", filters?.HasFlaggedAnswers);
        queryParams.Add("followUpStatus", filters?.FollowUpStatus?.ToString());
        queryParams.Add("level1", filters?.Level1Filter);
        queryParams.Add("level2", filters?.Level2Filter);
        queryParams.Add("level3", filters?.Level3Filter);
        queryParams.Add("level4", filters?.Level4Filter);
        queryParams.Add("level5", filters?.Level5Filter);
        queryParams.Add("locationId", filters?.LocationId);
        queryParams.Add("formId", filters?.FormId);
        queryParams.Add("formStatus", filters?.FormStatusFilter?.ToString());
        queryParams.Add("hasAttachments", filters?.HasAttachments);
        queryParams.Add("hasNotes", filters?.HasNotes);
        queryParams.Add("questionsAnswered", filters?.QuestionsAnswered?.ToString());
        filter.AddTo(queryParams);

        var sql = $"""
                  SELECT r.*
                  FROM "ExportGetCitizenReports"(
                      @electionRoundId, @ngoId, @hasFlaggedAnswers, @followUpStatus,
                      @level1, @level2, @level3, @level4, @level5, @locationId,
                      @formId, @formStatus, @hasAttachments, @hasNotes, @questionsAnswered) r
                  WHERE r."CitizenReportId" IN (
                      SELECT s."Id"
                      FROM "GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                      WHERE s."ElectionRoundId" = @electionRoundId
                        AND s."NgoId" = @ngoId
                        AND s."CitizenReportingEnabled" = TRUE
                        AND {filter.Sql}
                  )
                  ORDER BY
                      r."TimeSubmitted" DESC
                  """;

        IEnumerable<CitizenReportModel> citizenReports;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            citizenReports = await dbConnection.QueryAsync<CitizenReportModel>(sql, queryParams);
        }

        var citizenReportsData = citizenReports.ToArray();
        return citizenReportsData;
    }
}
