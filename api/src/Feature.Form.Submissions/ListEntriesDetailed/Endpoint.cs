using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Queries;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListEntriesDetailed;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<PagedResponse<DetailedSubmissionEntry>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byEntryDetailed");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(x => { x.Summary = "Lists form submissions by entry detailed"; });
    }

    public override async Task<Results<Ok<PagedResponse<DetailedSubmissionEntry>>, NotFound>> ExecuteAsync(Request req,
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
                   s."CreatedAt",
                   s."LastUpdatedAt",
                   s."FormId",
                   s."FormCode",
                   s."FormType",
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
                   s."IsOwnObserver",
                   s."MonitoringObserverStatus" AS "Status",
                   s."Tags",
                   s."NgoName",
                   s."NumberOfQuestionsAnswered",
                   s."NumberOfFlaggedAnswers",
                   s."MediaFilesCount",
                   s."NotesCount",
                   s."HasComments",
                   s."HasNotes",
                   s."HasAttachments",
                   s."Answers",
                   s."Notes",
                   s."Attachments",
                   s."ArrivalTime",
                   s."DepartureTime",
                   s."Breaks",
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
        List<DetailedSubmissionEntry> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, countTemplate.Parameters);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<DetailedSubmissionEntry>().ToList();
        }

        entries = (await Task.WhenAll(
            entries.Select(async entry => entry with
            {
                Attachments = await Task.WhenAll(
                    entry.Attachments.Select(async attachment =>
                    {
                        var result =
                            await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
                        return result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds)
                            ? attachment with { PresignedUrl = url, UrlValidityInSeconds = urlValidityInSeconds }
                            : attachment;
                    })
                )
            })
        )).ToList();

        return TypedResults.Ok(
            new PagedResponse<DetailedSubmissionEntry>(entries, totalRowCount, req.PageNumber, req.PageSize));
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
        QuestionsAnswered = req.QuestionsAnswered?.ToString(),
        FromDate = req.FromDateFilter,
        ToDate = req.ToDateFilter
    };

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"s.""CreatedAt"" DESC";
        }

        if (string.Equals(sortColumnName, "FormCode", StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""FormCode"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, "FormType", StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""FormType"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level1"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level2"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level3"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level4"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level5"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Number),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Number"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""DisplayName"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Email"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""PhoneNumber"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfQuestionsAnswered"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfFlaggedAnswers"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, "MediaFilesCount", StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""MediaFilesCount"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, "NotesCount", StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NotesCount"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.CreatedAt),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""CreatedAt"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.LastUpdatedAt),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""LastUpdatedAt"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, "MonitoringObserverStatus",
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""MonitoringObserverStatus"" {sortOrder}";
        }

        return @"s.""CreatedAt"" DESC";
    }
}
