using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.IncidentReports.ListEntries;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<IncidentReportEntryModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(x => { x.Summary = "Lists incident reports by entry in our system"; });
    }

    public override async Task<Results<Ok<PagedResponse<IncidentReportEntryModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT "Count" FROM "GetIncidentReportListEntriesCount"(
                      electionRoundId => @electionRoundId,
                      ngoId => @ngoId,
                      monitoringObserverId => @monitoringObserverId,
                      searchText => @searchText,
                      level1 => @level1,
                      level2 => @level2,
                      level3 => @level3,
                      level4 => @level4,
                      level5 => @level5,
                      pollingStationNumber => @pollingStationNumber,
                      pollingStationId => @pollingStationId,
                      hasFlaggedAnswers => @hasFlaggedAnswers,
                      followUpStatus => @followUpStatus,
                      tagsFilter => @tagsFilter,
                      monitoringObserverStatus => @monitoringObserverStatus,
                      formId => @formId,
                      formStatus => @formStatus,
                      questionsAnswered => @questionsAnswered,
                      locationType => @locationType,
                      hasAttachments => @hasAttachments,
                      hasNotes => @hasNotes,
                      hasComments => @hasComments,
                      fromDate => @fromDate,
                      toDate => @toDate,
                      stageId => @stageId,
                      reviewStatus => @reviewStatus,
                      reviewerId => @reviewerId,
                      hasReviewer => @hasReviewer,
                      severities => @severities
                  );

                  SELECT * FROM "GetIncidentReportListEntries"(
                      electionRoundId => @electionRoundId,
                      ngoId => @ngoId,
                      monitoringObserverId => @monitoringObserverId,
                      searchText => @searchText,
                      level1 => @level1,
                      level2 => @level2,
                      level3 => @level3,
                      level4 => @level4,
                      level5 => @level5,
                      pollingStationNumber => @pollingStationNumber,
                      pollingStationId => @pollingStationId,
                      hasFlaggedAnswers => @hasFlaggedAnswers,
                      followUpStatus => @followUpStatus,
                      tagsFilter => @tagsFilter,
                      monitoringObserverStatus => @monitoringObserverStatus,
                      formId => @formId,
                      formStatus => @formStatus,
                      questionsAnswered => @questionsAnswered,
                      locationType => @locationType,
                      hasAttachments => @hasAttachments,
                      hasNotes => @hasNotes,
                      hasComments => @hasComments,
                      fromDate => @fromDate,
                      toDate => @toDate,
                      stageId => @stageId,
                      reviewStatus => @reviewStatus,
                      reviewerId => @reviewerId,
                      hasReviewer => @hasReviewer,
                      severities => @severities,
                      sortExpression => @sortExpression,
                      offsetRows => @offset,
                      pageSize => @pageSize
                  );
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            monitoringObserverId = req.MonitoringObserverId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            LocationType = req.LocationType?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            pollingStationId = req.PollingStationId,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            stageId = req.StageId,
            reviewStatus = req.ReviewStatus?.Value,
            reviewerId = req.ReviewerId,
            hasReviewer = req.HasReviewer,
            severities = req.Severity?.Select(x => x.Value).ToArray() ?? [],
            followUpStatus = req.FollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            formStatus = req.FormStatusFilter?.ToString(),
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            hasComments = req.HasComments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
        };

        int totalRowCount;
        List<IncidentReportEntryModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<IncidentReportEntryModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<IncidentReportEntryModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (SeveritySql.IsSortColumn(sortColumnName))
        {
            // By level (Urgent highest), ties in the list's default order.
            return $"Severity {(isAscendingSorting ? "ASC" : "DESC")}";
        }

        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(IncidentReportEntryModel.TimeSubmitted)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.FormCode)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.NumberOfQuestionsAnswered)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.MediaFilesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.MediaFilesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.NotesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.NotesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.TimeSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.TimeSubmitted)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.Level1)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.Level2)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.Level3)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.Level4)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.Level5)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.PollingStationNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(IncidentReportEntryModel.PollingStationNumber)} {sortOrder}";
        }

        return $"{nameof(IncidentReportEntryModel.TimeSubmitted)} DESC";
    }
}
