using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListByFormV2;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions:byFormV2");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminOrStaff);

        Summary(x => { x.Summary = "Form submissions aggregated by form (v2)"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var compiler = new FilterSqlCompiler(SubmissionFilterFields.All);
        var filter = compiler.Build(req.FilterConditions);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("dataSource", req.DataSource.ToString());
        parameters.Add("offset", PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber));
        parameters.Add("pageSize", req.PageSize);

        filter.AddTo(parameters);

        var orderBy = GetOrderByClause(req.SortColumnName, req.IsAscendingSorting);

        var sql = $"""
                   WITH
                     FILTERED_SUBMISSIONS AS (
                       SELECT 
                         s."SubmissionId",
                         s."FormId",
                         s."FormCode",
                         s."FormType",
                         s."FormStatus",
                         s."DefaultLanguage" as "FormDefaultLanguage",
                         s."FormName",
                         s."NumberOfQuestionsAnswered",
                         s."NumberOfFlaggedAnswers",
                         s."MediaFilesCount",
                         s."NotesCount",
                         s."CommentsCount",
                         s."HasComments",
                         s."HasNotes",
                         s."HasAttachments",
                         s."FollowUpStatus",
                         s."IsCompleted",
                         s."MonitoringObserverStatus"
                   FROM "GetFormSubmissionEntries"(@electionRoundId, @ngoId, @dataSource) s
                   WHERE {filter.Sql})
                   SELECT
                     FS."FormId",
                     FS."FormCode",
                     FS."FormType",
                     FS."FormName",
                     FS."FormDefaultLanguage",
                     COUNT(FS."SubmissionId") AS "NumberOfSubmissions",
                     COALESCE(SUM(FS."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                     COALESCE(SUM(FS."MediaFilesCount"), 0) AS "NumberOfMediaFiles",
                     COALESCE(SUM(FS."NotesCount"), 0) AS "NumberOfNotes",
                     COALESCE(SUM(FS."CommentsCount"), 0) AS "CommentsCount"
                   FROM
                     FILTERED_SUBMISSIONS FS
                   WHERE
                     FS."FormStatus" <> 'Drafted'
                     AND FS."FormType" NOT IN ('CitizenReporting')
                     
                   GROUP BY
                     FS."FormId",
                     FS."FormCode",
                     FS."FormType",
                     FS."FormName",
                     FS."FormDefaultLanguage"
                   ORDER BY {orderBy};
                   """;

        IEnumerable<AggregatedFormOverview> aggregatedFormOverviews;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregatedFormOverviews = await dbConnection.QueryAsync<AggregatedFormOverview>(sql, parameters);
        }

        return TypedResults.Ok(new Response { AggregatedForms = aggregatedFormOverviews.ToList() });
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"""FormCode"" ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""FormCode"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.FormType),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""FormType"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfSubmissions),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfSubmissions"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfFlaggedAnswers"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfNotes),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfNotes"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfMediaFiles),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfMediaFiles"" {sortOrder}";
        }

        return "FormCode ASC";
    }
}
