using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Specifications;
namespace Feature.Form.Submissions.ListByObserver;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, PagedResponse<ObserverSubmissionsOverview>>
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

    public override async Task<PagedResponse<ObserverSubmissionsOverview>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var orderBySql = BuildOrderBySql(req.SortColumnName, req.IsAscendingSorting);

        string sql = @$"
        SELECT COUNT( distinct mo.""Id"")::int as ""totalRowCount""
        FROM ""MonitoringObservers"" mo
        inner join ""AspNetUsers"" o on mo.""ObserverId"" = o.""Id""
        INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
        LEFT JOIN ""FormSubmissions"" fs ON fs.""MonitoringObserverId"" = mo.""Id""
        LEFT JOIN ""PollingStationAttachments"" psa on psa.""MonitoringObserverId"" = mo.""Id""
        LEFT JOIN ""PollingStationNotes"" psn ON psn.""MonitoringObserverId"" = mo.""Id""
        LEFT JOIN ""PollingStationInformation"" psi ON psi.""MonitoringObserverId"" = mo.""Id""
        left join ""PollingStations"" ps on ps.""Id"" = fs.""PollingStationId""
        OR ps.""Id"" = psa.""PollingStationId""
        or ps.""Id"" = psn.""PollingStationId""
        or ps.""Id"" = psi.""PollingStationId""
        where ps.""ElectionRoundId"" = @electionRoundId
            AND mn.""NgoId"" = @ngoId
            AND (cardinality(@tagsFilter) = 0 OR mo.""Tags"" && @tagsFilter)
            AND (fs.""ElectionRoundId"" IS NULL OR fs.""ElectionRoundId"" = @electionRoundId)
            AND (psn.""ElectionRoundId"" IS NULL OR psn.""ElectionRoundId"" = @electionRoundId)
            AND (psi.""ElectionRoundId"" IS NULL OR psi.""ElectionRoundId"" = @electionRoundId)
            AND (psa.""ElectionRoundId"" IS NULL OR psa.""ElectionRoundId"" = @electionRoundId)
            AND (psa.""IsDeleted"" IS NULL OR psa.""IsDeleted"" = FALSE);

        SELECT mo.""Id"" as ""MonitoringObserverId"",
               o.""FirstName"",
               o.""LastName"",
               mo.""Tags"",
               coalesce(COUNT(fs.""Id""), 0) as ""NumberOfFormsSubmitted"",
               coalesce(SUM(fs.""NumberOfFlaggedAnswers""), 0) as ""NumberOfFlaggedAnswers"",
               coalesce(SUM(fs.""NumberOfFlaggedAnswers""), 0) as ""NumberOfFlaggedAnswers"",
               coalesce(COUNT(psa.""Id""), 0) as ""NumberOfUpload"",
               coalesce(COUNT(psn.""Id""), 0) as ""NumberOfNotes"",
               coalesce(COUNT(psi.""Id""), 0) as ""NumberOfPollingStationInformation"",
               coalesce(COUNT(DISTINCT ps.""Id""), 0) as ""NumberOfPollingStationsVisited"",
            (SELECT ""Timestamp""
             FROM ""AuditTrails""
             WHERE ""UserId"" = mo.""ObserverId""
             ORDER BY ""Timestamp"" DESC
             LIMIT 1) AS ""LastActivity""
        FROM ""MonitoringObservers"" mo
        inner join ""AspNetUsers"" o on mo.""ObserverId"" = o.""Id""
        INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
        LEFT JOIN ""FormSubmissions"" fs ON fs.""MonitoringObserverId"" = mo.""Id""
        LEFT JOIN ""PollingStationAttachments"" psa on psa.""MonitoringObserverId"" = mo.""Id""
        LEFT JOIN ""PollingStationNotes"" psn ON psn.""MonitoringObserverId"" = mo.""Id""
        LEFT JOIN ""PollingStationInformation"" psi ON psi.""MonitoringObserverId"" = mo.""Id""
        left join ""PollingStations"" ps on ps.""Id"" = fs.""PollingStationId""
        OR ps.""Id"" = psa.""PollingStationId""
        or ps.""Id"" = psn.""PollingStationId""
        or ps.""Id"" = psi.""PollingStationId""
        WHERE ps.""ElectionRoundId"" = @electionRoundId
            AND mn.""NgoId"" = @ngoId
            AND (cardinality(@tagsFilter) = 0 OR mo.""Tags"" && @tagsFilter)
            AND (fs.""ElectionRoundId"" IS NULL OR fs.""ElectionRoundId"" = @electionRoundId)
            AND (psn.""ElectionRoundId"" IS NULL OR psn.""ElectionRoundId"" = @electionRoundId)
            AND (psi.""ElectionRoundId"" IS NULL OR psi.""ElectionRoundId"" = @electionRoundId)
            AND (psa.""ElectionRoundId"" IS NULL OR psa.""ElectionRoundId"" = @electionRoundId)
            AND (psa.""IsDeleted"" IS NULL OR psa.""IsDeleted"" = FALSE)
        GROUP BY mo.""Id"",
                 o.""FirstName"",
                 o.""LastName"",
                 mo.""Tags""
        ORDER BY {orderBySql}
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.TagsFilter
        };
       
        var multi = await context.Connection.QueryMultipleAsync(sql, queryArgs);
        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<ObserverSubmissionsOverview>().ToList();

        return new PagedResponse<ObserverSubmissionsOverview>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string BuildOrderBySql(string sortOrderColumn, bool isAscendingSorting)
    {
        if (string.Equals(sortOrderColumn, nameof(ObserverSubmissionsOverview.FirstName), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? @"o.""FirstName"", mo.""Id"""
                : @"o.""FirstName"" desc, mo.""Id""";
        }

        if (string.Equals(sortOrderColumn, nameof(ObserverSubmissionsOverview.LastName), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? @"o.""LastName"", ""LastActivity"" desc"
                : @"o.""LastName"" desc, ""LastActivity"" desc";
        }

        if (string.Equals(sortOrderColumn, nameof(ObserverSubmissionsOverview.LastActivity), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? @"""LastActivity"", ""LastActivity"" desc"
                : @"""LastActivity"" desc, ""LastActivity"" desc";
        }

        if (string.Equals(sortOrderColumn, nameof(ObserverSubmissionsOverview.NumberOfFormsSubmitted), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? @"""NumberOfFormsSubmitted"", ""LastActivity"" desc"
                : @"""NumberOfFormsSubmitted"" desc, ""LastActivity"" desc";
        }

        if (string.Equals(sortOrderColumn, nameof(ObserverSubmissionsOverview.NumberOfQuestionAnswered), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? @"""NumberOfQuestionAnswered"", ""LastActivity"" desc"
                : @"""NumberOfQuestionAnswered"" desc, ""LastActivity"" desc";
        }

        if (string.Equals(sortOrderColumn, nameof(ObserverSubmissionsOverview.NumberOfFlaggedAnswers), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? @"""NumberOfFlaggedAnswers"", ""LastActivity"" desc"
                : @"""NumberOfFlaggedAnswers"" desc, ""LastActivity"" desc";
        }

        if (string.Equals(sortOrderColumn, nameof(ObserverSubmissionsOverview.NumberOfUploads), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? @"""NumberOfUploads"", ""LastActivity"" desc"
                : @"""NumberOfUploads"" desc, ""LastActivity"" desc";
        }

        if (string.Equals(sortOrderColumn, nameof(ObserverSubmissionsOverview.NumberOfNotes), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? @"""NumberOfNotes"", ""LastActivity"" desc"
                : @"""NumberOfNotes"" desc, ""LastActivity"" desc" ;
        }

        return @"""LastActivity"" desc";
    }

}
