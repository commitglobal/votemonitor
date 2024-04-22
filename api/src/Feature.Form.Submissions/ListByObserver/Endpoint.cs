using Authorization.Policies;
using Dapper;
using Feature.Form.Submissions.ListEntries;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Specifications;
namespace Feature.Form.Submissions.ListByObserver;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, PagedResponse<ObserverSubmissionOverview>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byObserver");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x =>
        {
            x.Summary = "Form submissions aggregated by observer";
        });
    }

    public override async Task<PagedResponse<ObserverSubmissionOverview>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        WITH submissions AS
            (SELECT psi.""Id"" AS ""SubmissionId"",
                    psi.""PollingStationId"",
                    psi.""MonitoringObserverId"",
                    0 AS ""NumberOfFlaggedAnswers""
             FROM ""PollingStationInformation"" psi
             INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = psi.""MonitoringObserverId""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId
                 AND mn.""NgoId"" =@ngoId
             UNION ALL SELECT fs.""Id"" AS ""SubmissionId"",
                              fs.""PollingStationId"",
                              fs.""MonitoringObserverId"",
                              fs.""NumberOfFlaggedAnswers""
             FROM ""FormSubmissions"" fs
             INNER JOIN ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             INNER JOIN ""Forms"" f on f.""Id"" = fs.""FormId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId
                 AND mn.""NgoId"" =@ngoId)
        select count(DISTINCT s.""MonitoringObserverId"") count
        from submissions s;

        WITH submissions AS
        (SELECT psi.""Id"" AS ""SubmissionId"",
                psi.""PollingStationId"",
                psi.""MonitoringObserverId"",
                0 AS ""NumberOfFlaggedAnswers""
         FROM ""PollingStationInformation"" psi
         INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = psi.""MonitoringObserverId""
         INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
         WHERE mn.""ElectionRoundId"" = @electionRoundId
             AND mn.""NgoId"" =@ngoId
         UNION ALL SELECT fs.""Id"" AS ""SubmissionId"",
                          fs.""PollingStationId"",
                          fs.""MonitoringObserverId"",
                          fs.""NumberOfFlaggedAnswers""
         FROM ""FormSubmissions"" fs
         INNER JOIN ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
         INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
         INNER JOIN ""Forms"" f on f.""Id"" = fs.""FormId""
         WHERE mn.""ElectionRoundId"" = @electionRoundId
             AND mn.""NgoId"" =@ngoId)
        select mo.""Id"" as ""MonitoringObserverId"",
               u.""FirstName"",
               u.""LastName"",
               u.""PhoneNumber"",
               u.""Email"",
               mo.""Tags"",
               sum(s.""NumberOfFlaggedAnswers"") as ""NumberOfFlaggedAnswers"",
               count(distinct s.""PollingStationId"") ""NumberOfLocations"",
               count(distinct s.""SubmissionId"") ""NumberOfFormsSubmitted""
        from submissions s
        inner join ""MonitoringObservers"" mo on mo.""Id"" = s.""MonitoringObserverId""
        inner join ""AspNetUsers"" u on u.""Id"" = mo.""ObserverId""
        group by mo.""Id"",
                 u.""FirstName"",
                 u.""LastName"",
                 u.""PhoneNumber"",
                 u.""Email""
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
        var entries = multi.Read<ObserverSubmissionOverview>().ToList();

        return new PagedResponse<ObserverSubmissionOverview>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }
}
