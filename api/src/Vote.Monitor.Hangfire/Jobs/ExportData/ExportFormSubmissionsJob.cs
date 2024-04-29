using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.ExportData;

public class ExportFormSubmissionsJob(VoteMonitorContext context,
    IFileStorageService fileStorageService,
    ILogger<ExportFormSubmissionsJob> logger, 
    ITimeProvider timeProvider) : IExportFormSubmissionsJob
{
    public async Task ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        var exportedData = await context
            .ExportedData
            .FirstOrDefaultAsync(x => x.ElectionRoundId == electionRoundId && x.NgoId == ngoId && x.Id == exportedDataId);

        try
        {
            if (exportedData == null)
            {
                logger.LogWarning("ExportData was not found for {electionRoundId} {ngoId} {exportedDataId}",
                    electionRoundId, ngoId, exportedDataId);
                throw new ExportedDataWasNotFoundException(electionRoundId, ngoId, exportedDataId);
            }

            if (exportedData.ExportStatus == ExportedDataStatus.Completed)
            {
                logger.LogWarning("ExportData was completed for {electionRoundId} {ngoId} {exportedDataId}",
                    electionRoundId, ngoId, exportedDataId);
                return;
            }

            var utcNow = timeProvider.UtcNow;

            var psiForm = await context
                .PollingStationInformationForms
                .Where(x => x.ElectionRoundId == electionRoundId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var publishedForms = await context
                .Forms
                .Where(x => x.ElectionRoundId == electionRoundId
                            && x.MonitoringNgo.NgoId == ngoId
                            && x.Status == FormStatus.Published)
                .AsNoTracking()
                .ToListAsync();

            var submissions = await GetSubmissions(electionRoundId, ngoId);

            foreach (var submission in submissions)
            {
                foreach (var attachment in submission.Attachments)
                {
                    var result = await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);

                    if (result is GetPresignedUrlResult.Ok okResult)
                    {
                        attachment.PresignedUrl = okResult.Url;
                    }
                }
            }

            var excelFileGenerator = ExcelFileGenerator.New();
            if (psiForm != null)
            {
                var psiDataTable = FormSubmissionsDataTable
                    .FromForm(psiForm)
                    .WithData()
                    .ForSubmissions(submissions)
                    .Please();

                excelFileGenerator.WithSheet("PSI", psiDataTable.header, psiDataTable.dataTable);
            }

            foreach (var form in publishedForms)
            {
                var sheetData = FormSubmissionsDataTable
                    .FromForm(form)
                    .WithData()
                    .ForSubmissions(submissions)
                    .Please();

                excelFileGenerator.WithSheet(form.Code, sheetData.header, sheetData.dataTable);
            }

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"form-submissions-{utcNow:yyyyMMdd_HHmmss}.xlsx";
            exportedData.Complete(fileName, base64EncodedData, utcNow);

            await context.SaveChangesAsync();
        }
        catch (ExportedDataWasNotFoundException)
        {
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when exporting data");
            exportedData!.Fail();
            await context.SaveChangesAsync();

            throw;
        }
    }

    private async Task<List<SubmissionModel>> GetSubmissions(Guid electionRoundId, Guid ngoId)
    {
        var sql = @"
            WITH submissions AS
            (SELECT psi.""Id"" as ""SubmissionId"",
            (select ""Id""
            from ""PollingStationInformationForms""
            where ""ElectionRoundId"" = @electionRoundId) as ""FormId"",
            psi.""PollingStationId"",
            psi.""MonitoringObserverId"",
            psi.""Answers"",
            (select ""Questions""
            from ""PollingStationInformationForms""
            where ""ElectionRoundId"" = @electionRoundId) as ""Questions"",
            '[]'::jsonb as ""Attachments"",
            '[]'::jsonb as ""Notes"",
            COALESCE(psi.""LastModifiedOn"", psi.""CreatedOn"") ""TimeSubmitted""
            FROM ""PollingStationInformation"" psi
            inner join ""MonitoringObservers"" mo ON mo.""Id"" = psi.""MonitoringObserverId""
            inner join ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
            WHERE mn.""ElectionRoundId"" = @electionRoundId
                and mn.""NgoId"" = @ngoId
            UNION ALL
            SELECT fs.""Id"" as ""SubmissionId"",
                   f.""Id"" as ""FormId"",
                   fs.""PollingStationId"",
                   fs.""MonitoringObserverId"",
                   fs.""Answers"",
                   f.""Questions"",

                COALESCE((select jsonb_build_array(jsonb_build_object('QuestionId', ""QuestionId"", 'FileName', ""FileName"", 'MimeType', ""MimeType"", 'FilePath', ""FilePath"", 'UploadedFileName', ""UploadedFileName"", 'TimeSubmitted', COALESCE(""LastModifiedOn"", ""CreatedOn"")))
                 from ""Attachments"" a
                 where a.""ElectionRoundId"" = @electionRoundId
                     and a.""FormId"" = fs.""FormId""
                     and a.""MonitoringObserverId"" = fs.""MonitoringObserverId""
                     and fs.""PollingStationId"" = a.""PollingStationId""),'[]'::JSONB) as ""Attachments"",

                COALESCE((select jsonb_build_array(jsonb_build_object('QuestionId', ""QuestionId"", 'Text', ""Text"", 'TimeSubmitted', COALESCE(""LastModifiedOn"", ""CreatedOn"")))
                 from ""Notes"" n
                 where n.""ElectionRoundId"" = @electionRoundId
                     and n.""FormId"" = fs.""FormId""
                     and n.""MonitoringObserverId"" = fs.""MonitoringObserverId""
                     and fs.""PollingStationId"" = n.""PollingStationId""), '[]'::JSONB) as ""Notes"",
                   COALESCE(fs.""LastModifiedOn"", fs.""CreatedOn"") ""TimeSubmitted""
            FROM ""FormSubmissions"" fs
            inner join ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
            inner join ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
            inner join ""Forms"" f on f.""Id"" = fs.""FormId""
            WHERE mn.""ElectionRoundId"" = @electionRoundId
                and mn.""NgoId"" = @ngoId
            order by ""TimeSubmitted"" desc)
            SELECT s.""SubmissionId"",
                   s.""FormId"",
                   s.""TimeSubmitted"",
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
                   s.""Attachments"",
                   s.""Notes"",
                   s.""Answers"",
                   s.""Questions""
            FROM submissions s
            inner join ""PollingStations"" ps on ps.""Id"" = s.""PollingStationId""
            inner join ""MonitoringObservers"" mo on mo.""Id"" = s.""MonitoringObserverId""
            inner join ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
            inner join ""Observers"" o ON o.""Id"" = mo.""ObserverId""
            inner join ""AspNetUsers"" u ON u.""Id"" = o.""ApplicationUserId""
            WHERE mn.""ElectionRoundId"" = @electionRoundId AND mn.""NgoId"" =@ngoId";

        var queryParams = new { electionRoundId, ngoId };

        var submissions = await context.Connection.QueryAsync<SubmissionModel>(sql, queryParams);
        var submissionsData = submissions.ToList();
        return submissionsData;
    }
}
