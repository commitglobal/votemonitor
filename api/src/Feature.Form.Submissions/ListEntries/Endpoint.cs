using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Queries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListEntries;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<FormSubmissionEntry>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x => { x.Summary = "Lists form submissions by entry in our system"; });
    }

    public override async Task<Results<Ok<PagedResponse<FormSubmissionEntry>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var builder = new SqlBuilder();
        builder.AddParameters(new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString(),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize
        });

        builder.ApplyFilters(ToFilterCriteria(req));
        builder.OrderBy(GetOrderByClause(req.SortColumnName, req.IsAscendingSorting));

        var countTemplate = builder.AddTemplate(
            """
            SELECT COUNT(1)
            FROM "GetFormSubmissionEntries"(@electionRoundId, @ngoId, @dataSource) s
            /**where**/
            """);

        var selectTemplate = builder.AddTemplate(
            """
            SELECT s."SubmissionId",
                   s."TimeSubmitted",
                   s."FormId",
                   s."FormCode",
                   s."FormType",
                   s."DefaultLanguage",
                   s."FormName",
                   s."PollingStationId",
                   s."Level1",
                   s."Level2",
                   s."Level3",
                   s."Level4",
                   s."Level5",
                   s."Number",
                   s."MonitoringObserverId",
                   s."DisplayName" AS "ObserverName",
                   s."Email",
                   s."PhoneNumber",
                   s."MonitoringObserverStatus" AS "Status",
                   s."Tags",
                   s."NgoName",
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
            /**where**/
            /**orderby**/
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY
            """);

        var sql = $"{countTemplate.RawSql};\n{selectTemplate.RawSql}";

        int totalRowCount;
        List<FormSubmissionEntry> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, countTemplate.Parameters);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<FormSubmissionEntry>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<FormSubmissionEntry>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static FormSubmissionEntriesFilterCriteria ToFilterCriteria(Request req) => new()
    {
        CoalitionMemberId = req.CoalitionMemberId,
        MonitoringObserverId = req.MonitoringObserverId,
        SearchText = req.SearchText,
        FormType = req.FormTypeFilter?.ToString(),
        Level1 = req.Level1Filter,
        Level2 = req.Level2Filter,
        Level3 = req.Level3Filter,
        Level4 = req.Level4Filter,
        Level5 = req.Level5Filter,
        PollingStationNumber = req.PollingStationNumberFilter,
        PollingStationId = req.PollingStationId,
        HasFlaggedAnswers = req.HasFlaggedAnswers,
        FollowUpStatus = req.FollowUpStatus?.ToString(),
        Tags = req.TagsFilter,
        MonitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
        FormId = req.FormId,
        HasNotes = req.HasNotes,
        HasAttachments = req.HasAttachments,
        HasComments = req.HasComments,
        QuestionsAnswered = req.QuestionsAnswered?.ToString(),
        FromDate = req.FromDateFilter,
        ToDate = req.ToDateFilter
    };

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"s.""TimeSubmitted"" DESC";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""FormCode"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.FormType),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""FormType"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level1"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level2"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level3"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level4"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level5"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Number),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Number"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""DisplayName"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Email"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""PhoneNumber"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfQuestionsAnswered"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfFlaggedAnswers"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.MediaFilesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""MediaFilesCount"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NotesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NotesCount"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.TimeSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""TimeSubmitted"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.MonitoringObserverStatus),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""MonitoringObserverStatus"" {sortOrder}";
        }

        return @"s.""TimeSubmitted"" DESC";
    }
}
