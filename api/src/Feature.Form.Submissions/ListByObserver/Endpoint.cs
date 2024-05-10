using System.Data;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;
namespace Feature.Form.Submissions.ListByObserver;

public class Endpoint(IDbConnection dbConnection) : Endpoint<Request, PagedResponse<ObserverSubmissionOverview>>
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
                 AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" && @tagsFilter)
             UNION ALL SELECT fs.""Id"" AS ""SubmissionId"",
                              fs.""PollingStationId"",
                              fs.""MonitoringObserverId"",
                              fs.""NumberOfFlaggedAnswers""
             FROM ""FormSubmissions"" fs
             INNER JOIN ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             INNER JOIN ""Forms"" f on f.""Id"" = fs.""FormId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId
                 AND mn.""NgoId"" =@ngoId
                 AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" && @tagsFilter))
        SELECT count(DISTINCT s.""MonitoringObserverId"") count
        FROM submissions s;

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
             AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" && @tagsFilter)
         UNION ALL SELECT fs.""Id"" AS ""SubmissionId"",
                          fs.""PollingStationId"",
                          fs.""MonitoringObserverId"",
                          fs.""NumberOfFlaggedAnswers""
         FROM ""FormSubmissions"" fs
         INNER JOIN ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
         INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
         INNER JOIN ""Forms"" f on f.""Id"" = fs.""FormId""
         WHERE mn.""ElectionRoundId"" = @electionRoundId
             AND mn.""NgoId"" =@ngoId
             AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" && @tagsFilter))

        SELECT
            ""MonitoringObserverId"",
            ""ObserverName"",
            ""PhoneNumber"",
            ""Email"",
            ""Tags"",
            ""NumberOfFlaggedAnswers"",
            ""NumberOfLocations"",
            ""NumberOfFormsSubmitted""
            FROM(
                Select 
                MO.""Id"" AS ""MonitoringObserverId"",
                U.""FirstName"" || ' ' || U.""LastName"" ""ObserverName"",
                U.""PhoneNumber"",
                U.""Email"",
                MO.""Tags"",
                SUM(S.""NumberOfFlaggedAnswers"") AS ""NumberOfFlaggedAnswers"",
                COUNT(DISTINCT S.""PollingStationId"") ""NumberOfLocations"",
                COUNT(DISTINCT S.""SubmissionId"") ""NumberOfFormsSubmitted""
            FROM
                SUBMISSIONS S
                INNER JOIN ""MonitoringObservers"" MO ON MO.""Id"" = S.""MonitoringObserverId""
                INNER JOIN ""AspNetUsers"" U ON U.""Id"" = MO.""ObserverId""
            GROUP BY
                MO.""Id"",
                U.""Id"") t
            ORDER BY
              CASE WHEN @sortExpression = 'ObserverName ASC' THEN ""ObserverName"" END ASC,
              CASE WHEN @sortExpression = 'ObserverName DESC' THEN ""ObserverName"" END DESC,

              CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN ""PhoneNumber"" END ASC,
              CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN ""PhoneNumber"" END DESC,

              CASE WHEN @sortExpression = 'Email ASC' THEN ""Email"" END ASC,
              CASE WHEN @sortExpression = 'Email DESC' THEN ""Email"" END DESC,

              CASE WHEN @sortExpression = 'Tags ASC' THEN ""Tags"" END ASC,
              CASE WHEN @sortExpression = 'Tags DESC' THEN ""Tags"" END DESC,
              
              CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers ASC' THEN ""NumberOfFlaggedAnswers"" END ASC,
              CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers DESC' THEN ""NumberOfFlaggedAnswers"" END DESC,
              
              CASE WHEN @sortExpression = 'NumberOfLocations ASC' THEN ""NumberOfLocations"" END ASC,
              CASE WHEN @sortExpression = 'NumberOfLocations DESC' THEN ""NumberOfLocations"" END DESC,
              
              CASE WHEN @sortExpression = 'NumberOfFormsSubmitted ASC' THEN ""NumberOfFormsSubmitted"" END ASC,
              CASE WHEN @sortExpression = 'NumberOfFormsSubmitted DESC' THEN ""NumberOfFormsSubmitted"" END DESC
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.TagsFilter ?? [],
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
        };

        var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<ObserverSubmissionOverview>().ToList();

        return new PagedResponse<ObserverSubmissionOverview>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(ObserverSubmissionOverview.ObserverName)} ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.Email), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.PhoneNumber), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.Tags), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfFlaggedAnswers), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfLocations), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.NumberOfLocations)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfFormsSubmitted), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.NumberOfFormsSubmitted)} {sortOrder}";
        }

        return $"{nameof(ObserverSubmissionOverview.ObserverName)} ASC";
    }
}
