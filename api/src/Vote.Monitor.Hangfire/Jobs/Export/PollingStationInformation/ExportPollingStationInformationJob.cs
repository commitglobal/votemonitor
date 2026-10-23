using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.PollingStationInformation.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.PollingStationInformation;

public class ExportPollingStationInformationJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    ILogger<ExportPollingStationInformationJob> logger,
    ITimeProvider timeProvider) : IExportPollingStationInformationJob
{
    public async Task Run(Guid electionRoundId, Guid exportedDataId, CancellationToken ct)
    {
        var exportedData = await context
            .ExportedData
            .Where(x => x.Id == exportedDataId)
            .FirstOrDefaultAsync(ct);

        if (exportedData == null)
        {
            logger.LogWarning("ExportData was not found for {exportDataType} {electionRoundId} {exportedDataId}",
                ExportedDataType.PollingStationInformation, electionRoundId, exportedDataId);
            throw new ExportedDataWasNotFoundException(ExportedDataType.PollingStationInformation, electionRoundId,
                exportedDataId);
        }

        try
        {
            if (exportedData.ExportStatus == ExportedDataStatus.Completed)
            {
                logger.LogWarning("ExportData was completed for {electionRoundId} {exportedDataId}",
                    electionRoundId, exportedDataId);
                return;
            }

            var utcNow = timeProvider.UtcNow;

            var form = await context.PollingStationInformationForms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ElectionRoundId == electionRoundId, ct);

            var submissions = await GetSubmissionsAsync(electionRoundId, ct);

            var excelFileGenerator = ExcelFileGenerator.New();
            var sheetData = form is null
                ? PsiSubmissionsDataTable.Empty().WithData().ForSubmissions(submissions).Please()
                : PsiSubmissionsDataTable.FromForm(form).WithData().ForSubmissions(submissions).Please();

            excelFileGenerator.WithSheet("psi-submissions", sheetData.header, sheetData.dataTable);

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"psi-submissions-{utcNow:yyyyMMdd_HHmmss}.xlsx";
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

    private async Task<List<PsiSubmissionModel>> GetSubmissionsAsync(Guid electionRoundId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT *
            FROM "ExportGetPollingStationInformation"(@electionRoundId)
            ORDER BY "LastUpdatedAt" DESC, "SubmissionId";
            """;

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        var submissions = await dbConnection.QueryAsync<PsiSubmissionModel>(sql, new { electionRoundId });
        return submissions.ToList();
    }
}
