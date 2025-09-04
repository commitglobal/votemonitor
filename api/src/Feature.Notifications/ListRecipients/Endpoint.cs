using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Notifications.ListRecipients;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) :
    Endpoint<Request, PagedResponse<TargetedMonitoringObserverModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications:listRecipients");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<PagedResponse<TargetedMonitoringObserverModel>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = 
            """
            WITH
                "ObserverPSI" AS (
                    SELECT
                        F."Id" AS "FormId",
                        F."FormType" AS "FormType",
                        MO."Id" AS "MonitoringObserverId",
                        FS."PollingStationId" AS "PollingStationId",
                        FS."FollowUpStatus" AS "FollowUpStatus",
                        FS."LastUpdatedAt" AS "LastModifiedOn",
                        FS."IsCompleted" AS "IsCompleted",
                        CAST(NULL AS BIGINT) AS "MediaFilesCount",
                        CAST(NULL AS BIGINT) AS "NotesCount",
                        (
                            CASE
                                WHEN FS."NumberOfQuestionsAnswered" = F."NumberOfQuestions" THEN 'All'
                                WHEN FS."NumberOfQuestionsAnswered" > 0 THEN 'Some'
                                WHEN FS."NumberOfQuestionsAnswered" = 0 THEN 'None'
                                END
                            ) "QuestionsAnswered",
                        (
                            CASE
                                WHEN FS."NumberOfFlaggedAnswers" > 0 THEN TRUE
                                ELSE FALSE
                                END
                            ) "HasFlaggedAnswers",
                        CAST(NULL AS UUID) AS "QuickReportId",
                        NULL AS "IncidentCategory",
                        NULL AS "QuickReportFollowUpStatus"
                    FROM
                        "MonitoringObservers" MO
                            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                            INNER JOIN "PollingStationInformation" FS ON MO."Id" = FS."MonitoringObserverId"
                            INNER JOIN "PollingStationInformationForms" F ON F."ElectionRoundId" = @ELECTIONROUNDID
                    WHERE
                        MN."ElectionRoundId" = @ELECTIONROUNDID
                      AND MN."NgoId" = @NGOID
                ),
                "ObserversFormSubmissions" AS (
                    SELECT
                        AF."FormId" AS "FormId",
                        AF."FormType" AS "FormType",
                        MO."Id" AS "MonitoringObserverId",
                        FS."PollingStationId" AS "PollingStationId",
                        FS."FollowUpStatus" AS "FollowUpStatus",
                        FS."LastUpdatedAt" AS "LastModifiedOn",
                        FS."IsCompleted" AS "IsCompleted",
                        (
                            SELECT
                                COUNT(*)
                            FROM
                                "Attachments" A
                            WHERE
                                A."FormId" = FS."FormId"
                              AND A."MonitoringObserverId" = FS."MonitoringObserverId"
                              AND FS."PollingStationId" = A."PollingStationId"
                              AND A."IsDeleted" = FALSE
                              AND A."IsCompleted" = TRUE
                        ) AS "MediaFilesCount",
                        (
                            SELECT
                                COUNT(*)
                            FROM
                                "Notes" N
                            WHERE
                                (
                                    (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                    OR N."SubmissionId" = FS."Id"
                                )
                              AND N."MonitoringObserverId" = FS."MonitoringObserverId"
                        ) AS "NotesCount",
                        (
                            CASE
                                WHEN FS."NumberOfQuestionsAnswered" = F."NumberOfQuestions" THEN 'All'
                                WHEN FS."NumberOfQuestionsAnswered" > 0 THEN 'Some'
                                WHEN FS."NumberOfQuestionsAnswered" = 0 THEN 'None'
                                END
                            ) "QuestionsAnswered",
                        (
                            CASE
                                WHEN FS."NumberOfFlaggedAnswers" > 0 THEN TRUE
                                ELSE FALSE
                                END
                            ) "HasFlaggedAnswers",
                        CAST(NULL AS UUID) AS "QuickReportId",
                        NULL AS "IncidentCategory",
                        NULL AS "QuickReportFollowUpStatus"
                    FROM
                        "MonitoringObservers" MO
                            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                            INNER JOIN "FormSubmissions" FS ON MO."Id" = FS."MonitoringObserverId"
                            INNER JOIN "GetAvailableForms" (@ELECTIONROUNDID, @NGOID, 'Coalition') AF ON AF."FormId" = FS."FormId"
                            INNER JOIN "Forms" F ON F."Id" = AF."FormId"
                    WHERE
                        MN."ElectionRoundId" = @ELECTIONROUNDID
                      AND MN."NgoId" = @NGOID
                ),
                "ObserversQuickReports" AS (
                    SELECT
                        CAST(NULL AS UUID) AS "FormId",
                        NULL AS "FormType",
                        MO."Id" AS "MonitoringObserverId",
                        QR."PollingStationId" AS "PollingStationId",
                        NULL AS "FollowUpStatus",
                        QR."LastUpdatedAt" AS "LastModifiedOn",
                        CAST(NULL AS BOOLEAN) AS "IsCompleted",
                        CAST(NULL AS BIGINT) AS "MediaFilesCount",
                        CAST(NULL AS BIGINT) AS "NotesCount",
                        NULL AS "QuestionsAnswered",
                        CAST(NULL AS BOOLEAN) AS "HasFlaggedAnswers",
                        QR."Id" AS "QuickReportId",
                        QR."IncidentCategory" AS "IncidentCategory",
                        QR."FollowUpStatus" AS "QuickReportFollowUpStatus"
                    FROM
                        "MonitoringObservers" MO
                            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                            INNER JOIN "QuickReports" QR ON MO."Id" = QR."MonitoringObserverId"
                    WHERE
                        MN."ElectionRoundId" = @ELECTIONROUNDID
                      AND MN."NgoId" = @NGOID
                ),
                "ObserversActivity" AS (
                    SELECT
                        *
                    FROM
                        "ObserversFormSubmissions"
                    UNION ALL
                    SELECT
                        *
                    FROM
                        "ObserversQuickReports"
                    UNION ALL
                    SELECT
                        *
                    FROM
                        "ObserverPSI"
                )
            SELECT
                COUNT(DISTINCT MO."Id") COUNT
            FROM
                "MonitoringObservers" MO
                    INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MO."MonitoringNgoId" = MN."Id"
                    LEFT JOIN "ObserversActivity" OA ON MO."Id" = OA."MonitoringObserverId"
                    LEFT JOIN "PollingStations" PS ON OA."PollingStationId" = PS."Id"
            WHERE
                MN."ElectionRoundId" = @ELECTIONROUNDID
              AND MN."NgoId" = @NGOID
              AND (
                @SEARCHTEXT IS NULL
                    OR @SEARCHTEXT = ''
                    OR U."DisplayName" ILIKE @SEARCHTEXT
                    OR U."Email" ILIKE @SEARCHTEXT
                    OR U."PhoneNumber" ILIKE @SEARCHTEXT
                    OR MO."Id"::TEXT ILIKE @SEARCHTEXT
                )
              AND (
                @TAGSFILTER IS NULL
                    OR CARDINALITY(@TAGSFILTER) = 0
                    OR MO."Tags" && @TAGSFILTER
                )
              AND (
                @MONITORINGOBSERVERSTATUS IS NULL
                    OR MO."Status" = @MONITORINGOBSERVERSTATUS
                )
              AND (
                @FORMTYPE IS NULL
                    OR OA."FormType" = @FORMTYPE
                )
              AND (
                @LEVEL1 IS NULL
                    OR PS."Level1" = @LEVEL1
                )
              AND (
                @LEVEL2 IS NULL
                    OR PS."Level2" = @LEVEL2
                )
              AND (
                @LEVEL3 IS NULL
                    OR PS."Level3" = @LEVEL3
                )
              AND (
                @LEVEL4 IS NULL
                    OR PS."Level4" = @LEVEL4
                )
              AND (
                @LEVEL5 IS NULL
                    OR PS."Level5" = @LEVEL5
                )
              AND (
                @POLLINGSTATIONNUMBER IS NULL
                    OR PS."Number" = @POLLINGSTATIONNUMBER
                )
              AND (
                @HASFLAGGEDANSWERS IS NULL
                    OR OA."HasFlaggedAnswers" = @HASFLAGGEDANSWERS
                )
              AND (
                @SUBMISSIONSFOLLOWUPSTATUS IS NULL
                    OR OA."FollowUpStatus" = @SUBMISSIONSFOLLOWUPSTATUS
                )
              AND (
                @FORMID IS NULL
                    OR OA."FormId" = @FORMID
                )
              AND (
                @QUESTIONSANSWERED IS NULL
                    OR OA."QuestionsAnswered" = @QUESTIONSANSWERED
                )
              AND (
                @HASATTACHMENTS IS NULL
                    OR (
                    @HASATTACHMENTS = TRUE
                        AND OA."MediaFilesCount" > 0
                    )
                    OR (
                    @HASATTACHMENTS = FALSE
                        AND OA."MediaFilesCount" = 0
                    )
                )
              AND (
                @HASNOTES IS NULL
                    OR (
                    OA."NotesCount" = 0
                        AND @HASNOTES = FALSE
                    )
                    OR (
                    OA."NotesCount" > 0
                        AND @HASNOTES = TRUE
                    )
                )
              AND (
                @FROMDATE IS NULL
                    OR OA."LastModifiedOn" >= @FROMDATE::TIMESTAMP
                )
              AND (
                @TODATE IS NULL
                    OR OA."LastModifiedOn" <= @TODATE::TIMESTAMP
                )
              AND (
                @HASQUICKREPORTS IS NULL
                    OR (
                    @HASQUICKREPORTS = TRUE
                        AND OA."QuickReportId" IS NOT NULL
                    )
                    OR (
                    @HASQUICKREPORTS = FALSE
                        AND OA."QuickReportId" IS NULL
                    )
                )
              AND (
                @QUICKREPORTFOLLOWUPSTATUS IS NULL
                    OR OA."QuickReportFollowUpStatus" = @QUICKREPORTFOLLOWUPSTATUS
                )
              AND (
                @QUICKREPORTINCIDENTCATEGORY IS NULL
                    OR OA."IncidentCategory" = @QUICKREPORTINCIDENTCATEGORY
                );
            
            -------------------------------------------------------------------------------------------------
            WITH
                "ObserverPSI" AS (
                    SELECT
                        F."Id" AS "FormId",
                        F."FormType" AS "FormType",
                        MO."Id" AS "MonitoringObserverId",
                        FS."PollingStationId" AS "PollingStationId",
                        FS."FollowUpStatus" AS "FollowUpStatus",
                        FS."LastUpdatedAt" AS "LastModifiedOn",
                        FS."IsCompleted" AS "IsCompleted",
                        CAST(NULL AS BIGINT) AS "MediaFilesCount",
                        CAST(NULL AS BIGINT) AS "NotesCount",
                        (
                            CASE
                                WHEN FS."NumberOfQuestionsAnswered" = F."NumberOfQuestions" THEN 'All'
                                WHEN FS."NumberOfQuestionsAnswered" > 0 THEN 'Some'
                                WHEN FS."NumberOfQuestionsAnswered" = 0 THEN 'None'
                                END
                            ) "QuestionsAnswered",
                        (
                            CASE
                                WHEN FS."NumberOfFlaggedAnswers" > 0 THEN TRUE
                                ELSE FALSE
                                END
                            ) "HasFlaggedAnswers",
                        CAST(NULL AS UUID) AS "QuickReportId",
                        NULL AS "IncidentCategory",
                        NULL AS "QuickReportFollowUpStatus"
                    FROM
                        "MonitoringObservers" MO
                            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                            INNER JOIN "PollingStationInformation" FS ON MO."Id" = FS."MonitoringObserverId"
                            INNER JOIN "PollingStationInformationForms" F ON F."ElectionRoundId" = @ELECTIONROUNDID
                    WHERE
                        MN."ElectionRoundId" = @ELECTIONROUNDID
                      AND MN."NgoId" = @NGOID
                ),
                "ObserversFormSubmissions" AS (
                    SELECT
                        AF."FormId" AS "FormId",
                        AF."FormType" AS "FormType",
                        MO."Id" AS "MonitoringObserverId",
                        FS."PollingStationId" AS "PollingStationId",
                        FS."FollowUpStatus" AS "FollowUpStatus",
                        FS."LastUpdatedAt" AS "LastModifiedOn",
                        FS."IsCompleted" AS "IsCompleted",
                        (
                            SELECT
                                COUNT(*)
                            FROM
                                "Attachments" A
                            WHERE
                                A."FormId" = FS."FormId"
                              AND A."MonitoringObserverId" = FS."MonitoringObserverId"
                              AND FS."PollingStationId" = A."PollingStationId"
                              AND A."IsDeleted" = FALSE
                              AND A."IsCompleted" = TRUE
                        ) AS "MediaFilesCount",
                        (
                            SELECT
                                COUNT(*)
                            FROM
                                "Notes" N
                            WHERE
                                (
                                    (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                    OR N."SubmissionId" = FS."Id"
                                )
                              AND N."MonitoringObserverId" = FS."MonitoringObserverId"
                        ) AS "NotesCount",
                        (
                            CASE
                                WHEN FS."NumberOfQuestionsAnswered" = F."NumberOfQuestions" THEN 'ALL'
                                WHEN FS."NumberOfQuestionsAnswered" > 0 THEN 'SOME'
                                WHEN FS."NumberOfQuestionsAnswered" = 0 THEN 'NONE'
                                END
                            ) "QuestionsAnswered",
                        (
                            CASE
                                WHEN FS."NumberOfFlaggedAnswers" > 0 THEN TRUE
                                ELSE FALSE
                                END
                            ) "HasFlaggedAnswers",
                        CAST(NULL AS UUID) AS "QuickReportId",
                        NULL AS "IncidentCategory",
                        NULL AS "QuickReportFollowUpStatus"
                    FROM
                        "MonitoringObservers" MO
                            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                            INNER JOIN "FormSubmissions" FS ON MO."Id" = FS."MonitoringObserverId"
                            INNER JOIN "GetAvailableForms" (@ELECTIONROUNDID, @NGOID, 'Coalition') AF ON AF."FormId" = FS."FormId"
                            INNER JOIN "Forms" F ON AF."FormId" = F."Id"
                    WHERE
                        MN."ElectionRoundId" = @ELECTIONROUNDID
                      AND MN."NgoId" = @NGOID
                ),
                "ObserversQuickReports" AS (
                    SELECT
                        CAST(NULL AS UUID) AS "FormId",
                        NULL AS "FormType",
                        MO."Id" AS "MonitoringObserverId",
                        QR."PollingStationId" AS "PollingStationId",
                        NULL AS "FollowUpStatus",
                        QR."LastUpdatedAt" AS "LastModifiedOn",
                        CAST(NULL AS BOOLEAN) AS "IsCompleted",
                        CAST(NULL AS BIGINT) AS "MediaFilesCount",
                        CAST(NULL AS BIGINT) AS "NotesCount",
                        NULL AS "QuestionsAnswered",
                        CAST(NULL AS BOOLEAN) AS "HasFlaggedAnswers",
                        QR."Id" AS "QuickReportId",
                        QR."IncidentCategory" AS "IncidentCategory",
                        QR."FollowUpStatus" AS "QuickReportFollowUpStatus"
                    FROM
                        "MonitoringObservers" MO
                            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                            INNER JOIN "QuickReports" QR ON MO."Id" = QR."MonitoringObserverId"
                    WHERE
                        MN."ElectionRoundId" = @ELECTIONROUNDID
                      AND MN."NgoId" = @NGOID
                ),
                "ObserversActivity" AS (
                    SELECT
                        *
                    FROM
                        "ObserversFormSubmissions"
                    UNION ALL
                    SELECT
                        *
                    FROM
                        "ObserversQuickReports"
                    UNION ALL
                    SELECT
                        *
                    FROM
                        "ObserverPSI"
                ),
                "FilteredObservers" AS (
                    SELECT DISTINCT
                        MO."Id" AS "MonitoringObserverId",
                        U."DisplayName" "ObserverName",
                        U."PhoneNumber",
                        U."Email",
                        MO."Tags",
                        MO."Status"
                    FROM
                        "MonitoringObservers" MO
                            INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                            INNER JOIN "MonitoringNgos" MN ON MO."MonitoringNgoId" = MN."Id"
                            LEFT JOIN "ObserversActivity" OA ON MO."Id" = OA."MonitoringObserverId"
                            LEFT JOIN "PollingStations" PS ON OA."PollingStationId" = PS."Id"
                    WHERE
                        MN."ElectionRoundId" = @ELECTIONROUNDID
                      AND MN."NgoId" = @NGOID
                      AND (
                        @SEARCHTEXT IS NULL
                            OR @SEARCHTEXT = ''
                            OR (U."DisplayName") ILIKE @SEARCHTEXT
                            OR U."Email" ILIKE @SEARCHTEXT
                            OR U."PhoneNumber" ILIKE @SEARCHTEXT
                            OR MO."Id"::TEXT ILIKE @SEARCHTEXT
                        )
                      AND (
                        @TAGSFILTER IS NULL
                            OR CARDINALITY(@TAGSFILTER) = 0
                            OR MO."Tags" && @TAGSFILTER
                        )
                      AND (
                        @MONITORINGOBSERVERSTATUS IS NULL
                            OR MO."Status" = @MONITORINGOBSERVERSTATUS
                        )
                      AND (
                        @FORMTYPE IS NULL
                            OR OA."FormType" = @FORMTYPE
                        )
                      AND (
                        @LEVEL1 IS NULL
                            OR PS."Level1" = @LEVEL1
                        )
                      AND (
                        @LEVEL2 IS NULL
                            OR PS."Level2" = @LEVEL2
                        )
                      AND (
                        @LEVEL3 IS NULL
                            OR PS."Level3" = @LEVEL3
                        )
                      AND (
                        @LEVEL4 IS NULL
                            OR PS."Level4" = @LEVEL4
                        )
                      AND (
                        @LEVEL5 IS NULL
                            OR PS."Level5" = @LEVEL5
                        )
                      AND (
                        @POLLINGSTATIONNUMBER IS NULL
                            OR PS."Number" = @POLLINGSTATIONNUMBER
                        )
                      AND (
                        @HASFLAGGEDANSWERS IS NULL
                            OR OA."HasFlaggedAnswers" = @HASFLAGGEDANSWERS
                        )
                      AND (
                        @SUBMISSIONSFOLLOWUPSTATUS IS NULL
                            OR OA."FollowUpStatus" = @SUBMISSIONSFOLLOWUPSTATUS
                        )
                      AND (
                        @FORMID IS NULL
                            OR OA."FormId" = @FORMID
                        )
                      AND (
                        @QUESTIONSANSWERED IS NULL
                            OR OA."QuestionsAnswered" = @QUESTIONSANSWERED
                        )
                      AND (
                        @HASATTACHMENTS IS NULL
                            OR (
                            @HASATTACHMENTS = TRUE
                                AND OA."MediaFilesCount" > 0
                            )
                            OR (
                            @HASATTACHMENTS = FALSE
                                AND OA."MediaFilesCount" = 0
                            )
                        )
                      AND (
                        @HASNOTES IS NULL
                            OR (
                            OA."NotesCount" = 0
                                AND @HASNOTES = FALSE
                            )
                            OR (
                            OA."NotesCount" > 0
                                AND @HASNOTES = TRUE
                            )
                        )
                      AND (
                        @FROMDATE IS NULL
                            OR OA."LastModifiedOn" >= @FROMDATE::TIMESTAMP
                        )
                      AND (
                        @TODATE IS NULL
                            OR OA."LastModifiedOn" <= @TODATE::TIMESTAMP
                        )
                      AND (
                        @HASQUICKREPORTS IS NULL
                            OR (
                            @HASQUICKREPORTS = TRUE
                                AND OA."QuickReportId" IS NOT NULL
                            )
                            OR (
                            @HASQUICKREPORTS = FALSE
                                AND OA."QuickReportId" IS NULL
                            )
                        )
                      AND (
                        @QUICKREPORTFOLLOWUPSTATUS IS NULL
                            OR OA."QuickReportFollowUpStatus" = @QUICKREPORTFOLLOWUPSTATUS
                        )
                      AND (
                        @QUICKREPORTINCIDENTCATEGORY IS NULL
                            OR OA."IncidentCategory" = @QUICKREPORTINCIDENTCATEGORY
                        )
                )
            SELECT
                *
            FROM
                "FilteredObservers"
            ORDER BY
                CASE
                    WHEN @SORTEXPRESSION = 'ObserverName ASC' THEN "ObserverName"
                    END ASC,
                CASE
                    WHEN @SORTEXPRESSION = 'ObserverName DESC' THEN "ObserverName"
                    END DESC,
                CASE
                    WHEN @SORTEXPRESSION = 'PhoneNumber ASC' THEN "PhoneNumber"
                    END ASC,
                CASE
                    WHEN @SORTEXPRESSION = 'PhoneNumber DESC' THEN "PhoneNumber"
                    END DESC,
                CASE
                    WHEN @SORTEXPRESSION = 'Email ASC' THEN "Email"
                    END ASC,
                CASE
                    WHEN @SORTEXPRESSION = 'Email DESC' THEN "Email"
                    END DESC,
                CASE
                    WHEN @SORTEXPRESSION = 'Tags ASC' THEN "Tags"
                    END ASC,
                CASE
                    WHEN @SORTEXPRESSION = 'Tags DESC' THEN "Tags"
                    END DESC,
                CASE
                    WHEN @SORTEXPRESSION = 'Status ASC' THEN "Status"
                    END ASC,
                CASE
                    WHEN @SORTEXPRESSION = 'Status DESC' THEN "Status"
                    END DESC
            OFFSET
                @OFFSET ROWS
                FETCH NEXT
                @PAGESIZE ROWS ONLY;
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            formType = req.FormTypeFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            submissionsFollowUpStatus = req.SubmissionsFollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),

            hasQuickReports = req.HasQuickReports,
            quickReportFollowUpStatus = req.QuickReportFollowUpStatus?.ToString(),
            quickReportIncidentCategory = req.QuickReportIncidentCategory?.ToString(),

            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,

            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<TargetedMonitoringObserverModel> entries;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<TargetedMonitoringObserverModel>().ToList();
        }

        return new PagedResponse<TargetedMonitoringObserverModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.Status),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.Status)} {sortOrder}";
        }

        return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} ASC";
    }
}
