using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Hangfire.Jobs.Export.IncidentReports;

/// <summary>
/// Incident reports with one column per question (see <see cref="SimplifiedAnswerWriter"/>), one sheet per form.
/// Same filters and query as <see cref="ExportIncidentReportsJob"/>.
/// </summary>
public class ExportIncidentReportsSimplifiedJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ILogger<ExportIncidentReportsSimplifiedJob> logger,
    ITimeProvider timeProvider) : IExportIncidentReportsSimplifiedJob
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
            throw new ExportedDataWasNotFoundException(ExportedDataType.IncidentReportsSimplified, electionRoundId,
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

            var incidentReportingForms = await context
                .Forms
                .Where(x => x.ElectionRoundId == electionRoundId
                            && x.MonitoringNgo.NgoId == ngoId
                            && x.Status != FormStatus.Drafted
                            && x.FormType == FormType.IncidentReporting)
                .AsNoTracking()
                .ToListAsync(ct);

            var filters = exportedData.IncidentReportsFilters ?? new ExportIncidentReportsFilters();
            var incidentReports = await ExportIncidentReportsJob.GetIncidentReports(dbConnectionFactory,
                electionRoundId, ngoId, filters, exportedData.FilterConditions, ct);

            foreach (var incidentReport in incidentReports)
            {
                var presignedAttachments = await fileStorageService.PresignAsync(incidentReport.Attachments);
                Array.Copy(presignedAttachments, incidentReport.Attachments, presignedAttachments.Length);
            }

            var excelFileGenerator = ExcelFileGenerator.New();

            for (var index = 0; index < incidentReportingForms.Count; index++)
            {
                var form = incidentReportingForms[index];
                var sheetData = IncidentReportsSimplifiedDataTable
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
}
