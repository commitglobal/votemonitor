using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;
using Vote.Monitor.Hangfire.Jobs.Export.PollingStations.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.PollingStations;

public class ExportPollingStationsJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    ILogger<ExportFormSubmissionsJob> logger,
    ITimeProvider timeProvider) : IExportPollingStationsJob
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
                ExportedDataType.PollingStations, electionRoundId, exportedDataId);
            throw new ExportedDataWasNotFoundException(ExportedDataType.PollingStations, electionRoundId,
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

            var pollingStations = await GetPollingStationsAsync(electionRoundId, ct);

            var excelFileGenerator = ExcelFileGenerator.New();

            var sheetData = PollingStationsDataTable
                .New()
                .WithData()
                .ForPollingStations(pollingStations)
                .Please();

            excelFileGenerator.WithSheet("polling-stations", sheetData.header, sheetData.dataTable);

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"polling-stations-{utcNow:yyyyMMdd_HHmmss}.xlsx";
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

    private async Task<List<PollingStationModel>> GetPollingStationsAsync(Guid electionRoundId, CancellationToken ct)
    {
        var sql = """
                  SELECT
                      PS."Id",
                      PS."Level1",
                      PS."Level2",
                      PS."Level3",
                      PS."Level4",
                      PS."Level5",
                      PS."Number",
                      PS."Address",
                      ps."DisplayOrder",
                      PS."Tags"
                  FROM
                      "PollingStations" PS
                  WHERE
                      "ElectionRoundId" = @electionRoundId
                  ORDER BY
                      "DisplayOrder" ASC;
                  """;

        var queryArgs = new { electionRoundId, };

        IEnumerable<PollingStationModel> pollingStations;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            pollingStations = await dbConnection.QueryAsync<PollingStationModel>(sql, queryArgs);
        }

        var pollingStationsData = pollingStations.ToList();
        return pollingStationsData;
    }
}
