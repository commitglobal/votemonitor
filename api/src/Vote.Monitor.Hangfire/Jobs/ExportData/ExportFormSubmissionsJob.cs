using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Hangfire.Jobs.ExportData;

public class ExportFormSubmissionsJob(VoteMonitorContext context, ILogger<ExportFormSubmissionsJob> logger) : IExportFormSubmissionsJob
{
    public async Task ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        try
        {
            var exportedData = await context.ExportedData.FirstOrDefaultAsync(x =>
                x.ElectionRoundId == electionRoundId && x.NgoId == ngoId && x.Id == exportedDataId);

            if (exportedData == null)
            {
                logger.LogWarning("ExportData was not found for {electionRoundId} {ngoId} {exportedDataId}", electionRoundId, ngoId, exportedDataId);
                return;
            }

            var psiForm = await context
                 .PollingStationInformationForms
                 .Where(x => x.ElectionRoundId == electionRoundId)
                 .AsNoTracking()
                 .FirstOrDefaultAsync();

            if (psiForm == null)
            {
                logger.LogWarning("ExportData was not found for {electionRoundId} {ngoId} {exportedDataId}", electionRoundId, ngoId, exportedDataId);
                return;
            }

            var utcNow = DateTime.UtcNow;
            var publishedForms = await context
                .Forms
                .Where(x => x.ElectionRoundId == electionRoundId
                            && x.MonitoringNgo.NgoId == ngoId
                            && x.Status == FormStatus.Published)
                .AsNoTracking()
                .ToListAsync();



            var sql = @"
            WITH submissions AS
                (SELECT psi.""Id"" AS ""SubmissionId"",
                        'PSI' AS ""FormType"",
                        'PSI' AS ""FormCode"",
                        psi.""PollingStationId"",
                        psi.""MonitoringObserverId"",
                        psi.""NumberOfQuestionsAnswered"",
                        0 AS ""NumberOfFlaggedAnswers"",
                        0 AS ""MediaFilesCount"",
                        0 AS ""NotesCount"",
                        COALESCE(psi.""LastModifiedOn"", psi.""CreatedOn"") ""TimeSubmitted""
                 FROM ""PollingStationInformation"" psi
                 INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = psi.""MonitoringObserverId""
                 INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
                 WHERE mn.""ElectionRoundId"" = @electionRoundId
                     AND mn.""NgoId"" =@ngoId
                     AND (@monitoringObserverId IS NULL OR mo.""Id"" =@monitoringObserverId)
                 UNION ALL SELECT fs.""Id"" AS ""SubmissionId"",
                                  f.""FormType"" AS ""FormType"",
                                  f.""Code"" AS ""FormCode"",
                                  fs.""PollingStationId"",
                                  fs.""MonitoringObserverId"",
                                  fs.""NumberOfQuestionsAnswered"",
                                  fs.""NumberOfFlaggedAnswers"",
                                  (SELECT COUNT(1) from ""Attachments"" WHERE ""FormId"" = fs.""Id"") AS ""MediaFilesCount"",
                                  (select count(1) from ""Notes"" WHERE ""FormId"" = fs.""Id"") AS ""NotesCount"",
                                  COALESCE(fs.""LastModifiedOn"", fs.""CreatedOn"") ""TimeSubmitted""
                 FROM ""FormSubmissions"" fs
                 INNER JOIN ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
                 INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
                 INNER JOIN ""Forms"" f on f.""Id"" = fs.""FormId""
                 WHERE mn.""ElectionRoundId"" = @electionRoundId
                     AND mn.""NgoId"" =@ngoId
                     AND (@monitoringObserverId IS NULL OR mo.""Id"" =@monitoringObserverId)
                 ORDER BY ""TimeSubmitted"" desc)

            SELECT s.""SubmissionId"",
                   s.""TimeSubmitted"",
                   s.""FormCode"",
                   s.""FormType"",
                   ps.""Id"" as ""PollingStationId"",
                   ps.""Level1"",
                   ps.""Level2"",
                   ps.""Level3"",
                   ps.""Level4"",
                   ps.""Level5"",
                   ps.""Number"",
                   s.""MonitoringObserverId"",
                   u.""FirstName"",
                   u.""LastName"",
                   u.""Email"",
                   u.""PhoneNumber"",
                   mo.""Tags"",
                   s.""NumberOfQuestionsAnswered"",
                   s.""NumberOfFlaggedAnswers"",
                   s.""MediaFilesCount"",
                   s.""NotesCount""
            FROM submissions s
            INNER JOIN ""PollingStations"" ps on ps.""Id"" = s.""PollingStationId""
            INNER JOIN ""MonitoringObservers"" mo on mo.""Id"" = s.""MonitoringObserverId""
            INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
            INNER JOIN ""Observers"" o ON o.""Id"" = mo.""ObserverId""
            INNER JOIN ""AspNetUsers"" u ON u.""Id"" = o.""ApplicationUserId""
            WHERE mn.""ElectionRoundId"" = @electionRoundId
                AND mn.""NgoId"" = @ngoId
            ORDER BY ""TimeSubmitted"" desc";

            var fileName = $"form-submissions-{utcNow:yyyyMMdd_HHmmss}.xlsx";

            var queryParams = new
            {
                electionRoundId = electionRoundId,
                ngoId = ngoId
            };


            var submissions = await context.Connection.QueryAsync<dynamic>(sql, queryParams);


            exportedData.Complete(fileName, "base64", utcNow);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when exporting data");
            throw;
        }
    }
}
