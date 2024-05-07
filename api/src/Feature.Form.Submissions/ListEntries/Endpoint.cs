using System.Data;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListEntries;

public class Endpoint(IDbConnection dbConnection) : Endpoint<Request, PagedResponse<FormSubmissionEntry>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x =>
        {
            x.Summary = "Lists form submissions by entry in our system";
        });
    }

    public override async Task<PagedResponse<FormSubmissionEntry>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT SUM(count)
        FROM
            (SELECT count(1) as count
             FROM ""PollingStationInformation"" psi
             INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = psi.""MonitoringObserverId""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId
                 AND mn.""NgoId"" =@ngoId
             UNION ALL SELECT count(1) as count
             FROM ""FormSubmissions"" fs
             INNER JOIN ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId AND mn.""NgoId"" =@ngoId
             AND (@monitoringObserverId IS NULL OR mo.""Id"" =@monitoringObserverId)) c;

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
            AND (@monitoringObserverId IS NULL OR mo.""Id"" =@monitoringObserverId)
            AND (@formCode is null OR s.""FormCode"" = @formCode) 
            AND (@formType is null OR s.""FormType"" = @formType)
            AND (@level1 is null OR ps.""Level1"" = @level1)
            AND (@level2 is null OR ps.""Level2"" = @level2)
            AND (@level3 is null OR ps.""Level3"" = @level3)
            AND (@level4 is null OR ps.""Level4"" = @level4)
            AND (@level5 is null OR ps.""Level5"" = @level5)
            AND (@pollingStationNumber is null OR ps.""Number"" = @pollingStationNumber)
            AND ((@hasFlaggedAnswers = false OR @hasFlaggedAnswers IS NULL) OR s.""NumberOfFlaggedAnswers"" > 0 )
        ORDER BY ""TimeSubmitted"" desc
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            monitoringObserverId = req.MonitoringObserverId,
            formCode = string.IsNullOrWhiteSpace(req.FormCodeFilter) ? null : req.FormCodeFilter,
            formType = req.FormTypeFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
        };

        var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<FormSubmissionEntry>().ToList();

        return new PagedResponse<FormSubmissionEntry>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }
}
