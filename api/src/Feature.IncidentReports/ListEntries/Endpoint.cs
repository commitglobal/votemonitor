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
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x => { x.Summary = "Lists incident reports by entry in our system"; });
    }

    public override async Task<Results<Ok<PagedResponse<IncidentReportEntryModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT
                      COUNT(*) AS COUNT
                  FROM
                      "IncidentReports" IR
                          INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                          INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                          INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                          INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                          INNER JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                          LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                  WHERE
                      MN."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                    AND (@monitoringObserverId IS NULL OR MO."Id" = @monitoringObserverId)
                    AND (@searchText IS NULL
                          OR @searchText = ''
                          OR U."FirstName" ILIKE @searchText
                          OR U."LastName" ILIKE @searchText
                          OR U."Email" ILIKE @searchText
                          OR U."PhoneNumber" ILIKE @searchText
                      )
                    AND (@level1 IS NULL OR PS."Level1" = @level1)
                    AND (@level2 IS NULL OR PS."Level2" = @level2)
                    AND (@level3 IS NULL OR PS."Level3" = @level3)  
                    AND (@level4 IS NULL OR PS."Level4" = @level4)
                    AND (@level5 IS NULL OR PS."Level5" = @level5)
                    AND (@pollingStationNumber IS NULL OR PS."Number" = @pollingStationNumber)
                    AND (@hasFlaggedAnswers IS NULL
                          OR (IR."NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = FALSE)
                          OR ("NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = TRUE)
                      )
                    AND (@followUpStatus IS NULL OR IR."FollowUpStatus" = @followUpStatus)
                    AND (@tagsFilter IS NULL OR CARDINALITY(@tagsFilter) = 0 OR MO."Tags" && @tagsFilter)
                    AND (@monitoringObserverStatus IS NULL OR MO."Status" = @monitoringObserverStatus)
                    AND (@formId IS NULL OR IR."FormId" = @formId)
                    AND (@questionsAnswered IS NULL
                          OR (@questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                          OR (@questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                          OR (@questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                      )
                    AND (@locationType IS NULL OR IR."LocationType" = @locationType)
                    AND (@hasAttachments IS NULL
                          OR (@hasAttachments = FALSE AND
                          (
                              SELECT
                                  COUNT(1)
                              FROM
                                  "IncidentReportAttachments" A
                              WHERE
                                  A."IncidentReportId" = IR."Id"
                                AND IR."PollingStationId" = "PollingStationId"
                                AND A."IsDeleted" = FALSE
                                AND A."IsCompleted" = TRUE
                          ) = 0)
                          OR (
                          (@hasAttachments = TRUE AND(
                              SELECT
                                  COUNT(1)
                              FROM
                                  "IncidentReportAttachments" A
                              WHERE
                                  A."IncidentReportId" = IR."Id"
                                AND A."IsDeleted" = FALSE
                                AND A."IsCompleted" = TRUE
                          ) > 0)
                      )
                    AND (@hasNotes IS NULL
                          OR (@hasNotes = FALSE AND (SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") = 0)
                          OR (@hasNotes = TRUE AND (SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") > 0 )
                      ));
                  WITH
                      INCIDENT_REPORTS AS (
                          SELECT
                              IR."Id" AS "IncidentReportId",
                              F."Code" AS "FormCode",
                              F."Name" AS "FormName",
                              F."DefaultLanguage" "FormDefaultLanguage",
                              IR."LocationType",
                              IR."LocationDescription",
                              IR."PollingStationId",
                              IR."MonitoringObserverId",
                              IR."NumberOfQuestionsAnswered",
                              IR."NumberOfFlaggedAnswers",
                              (
                                  SELECT
                                      COUNT(1)
                                  FROM
                                      "IncidentReportAttachments" A
                                  WHERE
                                      A."IncidentReportId" = IR."Id"
                                    AND "IsDeleted" = FALSE
                                    AND "IsCompleted" = TRUE
                              ) AS "MediaFilesCount",
                              ( SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") AS "NotesCount",
                              COALESCE(IR."LastModifiedOn", IR."CreatedOn") AS "TimeSubmitted",
                              IR."FollowUpStatus"
                          FROM
                              "IncidentReports" IR
                                  INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                                  INNER JOIN "MonitoringObservers" MO ON IR."MonitoringObserverId" = MO."Id"
                                  INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                          WHERE
                              MN."ElectionRoundId" = @electionRoundId
                            AND MN."NgoId" = @ngoId
                            AND (@monitoringObserverId IS NULL OR MO."Id" = @monitoringObserverId)
                            AND (@monitoringObserverStatus IS NULL OR MO."Status" = @monitoringObserverStatus)
                            AND (@formId IS NULL OR IR."FormId" = @formId)
                            AND (@locationType IS NULL OR IR."LocationType" = @locationType)
                            AND (@questionsAnswered IS NULL
                              OR (@questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                              OR (@questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                              OR (@questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                              )
                      )
                  SELECT
                      IR."IncidentReportId",
                      IR."TimeSubmitted",
                      IR."FormCode",
                      IR."FormName",
                      IR."FormDefaultLanguage",
                      IR."LocationType",
                      IR."LocationDescription",
                      PS."Id" AS "PollingStationId",
                      PS."Level1",
                      PS."Level2",
                      PS."Level3",
                      PS."Level4",
                      PS."Level5",
                      PS."Number" "PollingStationNumber",
                      IR."MonitoringObserverId",
                      U."FirstName" || ' ' || U."LastName" AS "ObserverName",
                      U."Email",
                      U."PhoneNumber",
                      MO."Status",
                      MO."Tags",
                      IR."NumberOfQuestionsAnswered",
                      IR."NumberOfFlaggedAnswers",
                      IR."MediaFilesCount",
                      IR."NotesCount",
                      IR."FollowUpStatus"
                  FROM
                      INCIDENT_REPORTS IR
                          INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                          INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                          INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                          INNER JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                          LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                  WHERE
                      MN."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                    AND (@monitoringObserverId IS NULL OR MO."Id" = @monitoringObserverId)
                    AND (
                      @searchText IS NULL
                          OR @searchText = ''
                          OR U."FirstName" ILIKE @searchText
                          OR U."LastName" ILIKE @searchText
                          OR U."Email" ILIKE @searchText
                          OR U."PhoneNumber" ILIKE @searchText
                      )
                    AND (@level1 IS NULL OR PS."Level1" = @level1)
                    AND (@level2 IS NULL OR PS."Level2" = @level2)
                    AND (@level3 IS NULL OR PS."Level3" = @level3)
                    AND (@level4 IS NULL OR PS."Level4" = @level4)
                    AND (@level5 IS NULL OR PS."Level5" = @level5)
                    AND (@pollingStationNumber IS NULL OR PS."Number" = @pollingStationNumber)
                    AND (@hasFlaggedAnswers IS NULL
                      OR (IR."NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = FALSE)
                      OR (IR."NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = TRUE)
                      )
                    AND (@followUpStatus IS NULL OR IR."FollowUpStatus" = @followUpStatus)
                    AND (@tagsFilter IS NULL OR CARDINALITY(@tagsFilter) = 0 OR MO."Tags" && @tagsFilter)
                    AND (@hasNotes IS NULL
                      OR (IR."NotesCount" = 0 AND @hasNotes = FALSE)
                      OR (IR."NotesCount" > 0 AND @hasNotes = TRUE)
                      )
                    AND (@hasAttachments IS NULL
                      OR (IR."MediaFilesCount" = 0 AND @hasAttachments = FALSE)
                      OR (IR."MediaFilesCount" > 0 AND @hasAttachments = TRUE)
                      )
                  ORDER BY
                      CASE WHEN @sortExpression = 'TimeSubmitted ASC' THEN IR."TimeSubmitted" END ASC,
                      CASE WHEN @sortExpression = 'TimeSubmitted DESC' THEN IR."TimeSubmitted" END DESC,
                      CASE WHEN @sortExpression = 'FormCode ASC' THEN IR."FormCode" END ASC,
                      CASE WHEN @sortExpression = 'FormCode DESC' THEN IR."FormCode" END DESC,
                      CASE WHEN @sortExpression = 'Level1 ASC' THEN PS."Level1" END ASC,
                      CASE WHEN @sortExpression = 'Level1 DESC' THEN PS."Level1" END DESC,
                      CASE WHEN @sortExpression = 'Level2 ASC' THEN PS."Level2" END ASC,
                      CASE WHEN @sortExpression = 'Level2 DESC' THEN PS."Level2" END DESC,
                      CASE WHEN @sortExpression = 'Level3 ASC' THEN PS."Level3" END ASC,
                      CASE WHEN @sortExpression = 'Level3 DESC' THEN PS."Level3" END DESC,
                      CASE WHEN @sortExpression = 'Level4 ASC' THEN PS."Level4" END ASC,
                      CASE WHEN @sortExpression = 'Level4 DESC' THEN PS."Level4" END DESC,
                      CASE WHEN @sortExpression = 'Level5 ASC' THEN PS."Level5" END ASC,
                      CASE WHEN @sortExpression = 'Level5 DESC' THEN PS."Level5" END DESC,
                      CASE WHEN @sortExpression = 'PollingStationNumber ASC' THEN PS."Number" END ASC,
                      CASE WHEN @sortExpression = 'PollingStationNumber DESC' THEN PS."Number" END DESC,
                      CASE WHEN @sortExpression = 'ObserverName ASC' THEN U."FirstName" || ' ' || U."LastName" END ASC,
                      CASE WHEN @sortExpression = 'ObserverName DESC' THEN U."FirstName" || ' ' || U."LastName" END DESC
                  OFFSET @offset ROWS
                      FETCH NEXT @pageSize ROWS ONLY;
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
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            followUpStatus = req.FollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            
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