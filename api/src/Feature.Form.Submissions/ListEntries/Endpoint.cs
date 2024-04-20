using Authorization.Policies;
using Dapper;
using Feature.Form.Submissions.ListByObserver;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListEntries;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, PagedResponse<FormSubmissionEntry>>
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
        var sql = @$"
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
             WHERE mn.""ElectionRoundId"" = @electionRoundId AND mn.""NgoId"" =@ngoId) c


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
        ORDER BY ""TimeSubmitted"" desc
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
        };

        var multi = await context.Connection.QueryMultipleAsync(sql, queryArgs);
        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<FormSubmissionEntry>().ToList();

        return new PagedResponse<FormSubmissionEntry>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }
}
