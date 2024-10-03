using Dapper;
using Feature.CitizenReports.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListEntries;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<PagedResponse<CitizenReportEntryModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x => { x.Summary = "Lists citizen report submissions by entry in our system"; });
    }

    public override async Task<Results<Ok<PagedResponse<CitizenReportEntryModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT
                  	COUNT(*) AS COUNT
                  FROM
                  	"CitizenReports" CR
                  	INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                  	INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                    INNER JOIN "Locations" L on L."Id" = CR."LocationId"
                    INNER JOIN "Forms" F on F."Id" = CR."FormId"
                  WHERE
                  	MN."ElectionRoundId" = @electionRoundId
                  	AND MN."NgoId" = @ngoId
                  	AND (
                  		@followUpStatus IS NULL
                  		OR CR."FollowUpStatus" = @followUpStatus
                  	)
                    AND (@level1 IS NULL OR L."Level1" = @level1)
                  	AND (@level2 IS NULL OR L."Level2" = @level2)
                  	AND (@level3 IS NULL OR L."Level3" = @level3)
                  	AND (@level4 IS NULL OR L."Level4" = @level4)
                  	AND (@level5 IS NULL OR L."Level5" = @level5)
                  	AND (@formId IS NULL OR CR."FormId" = @formId)
                  	AND (@questionsAnswered IS NULL 
                  	  OR (@questionsAnswered = 'All' AND F."NumberOfQuestions" = CR."NumberOfQuestionsAnswered")
                  	  OR (@questionsAnswered = 'Some' AND F."NumberOfQuestions" <> CR."NumberOfQuestionsAnswered")
                  	  OR (@questionsAnswered = 'None' AND CR."NumberOfQuestionsAnswered" = 0))
                  	AND (@hasAttachments is NULL
                       OR ((SELECT COUNT(1) FROM "CitizenReportAttachments" WHERE "CitizenReportId" = CR."Id" AND "IsDeleted" = false AND "IsCompleted" = true) = 0 AND @hasAttachments = false) 
                       OR ((SELECT COUNT(1) FROM "CitizenReportAttachments" WHERE "CitizenReportId" = CR."Id" AND "IsDeleted" = false AND "IsCompleted" = true) > 0 AND @hasAttachments = true))
                  	AND (@hasNotes is NULL 
                       OR ((SELECT COUNT(1) FROM "CitizenReportNotes" WHERE "CitizenReportId" = CR."Id") = 0 AND @hasNotes = false) 
                       OR ((SELECT COUNT(1) FROM "CitizenReportNotes" WHERE "CitizenReportId" = CR."Id") > 0 AND @hasNotes = true))
                  	;

                  WITH
                  	CITIZENREPORTS AS (
                  		SELECT
                  			CR."Id" "CitizenReportId",
                  			COALESCE(CR."LastModifiedOn", CR."CreatedOn") "TimeSubmitted",
                  			F."Code" "FormCode",
                  			F."Name" "FormName",
                  			F."DefaultLanguage" "FormDefaultLanguage",
                  			CR."NumberOfQuestionsAnswered",
                  			CR."NumberOfFlaggedAnswers",
                  			(
                  				SELECT
                  					COUNT(1)
                  				FROM
                  					"CitizenReportNotes" CRN
                  				WHERE
                  					CRN."CitizenReportId" = CR."Id"
                  			) AS "NotesCount",
                  			(
                  				SELECT
                  					COUNT(1)
                  				FROM
                  					"CitizenReportAttachments" CRA
                  				WHERE
                  					CRA."CitizenReportId" = CR."Id"
                  					AND CRA."IsCompleted" = TRUE
                  					AND CRA."IsDeleted" = FALSE
                  			) AS "MediaFilesCount",
                  			CR."FollowUpStatus",
                  			L."Level1",
                     		L."Level2",
                     		L."Level3",
                     		L."Level4",
                     		L."Level5"
                  		FROM
                  			"CitizenReports" CR
                  			INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                  			INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                  			INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                  			INNER JOIN "Locations" L on L."Id" = CR."LocationId"
                  		WHERE
                  			CR."ElectionRoundId" = @electionRoundId
                  			AND MN."NgoId" = @ngoId
                  			AND (
                  				@hasFlaggedAnswers IS NULL
                  				OR (
                  					"NumberOfFlaggedAnswers" = 0
                  					AND @hasFlaggedAnswers = FALSE
                  				)
                  				OR (
                  					"NumberOfFlaggedAnswers" > 0
                  					AND @hasFlaggedAnswers = TRUE
                  				)
                  			)
                  			AND (
                  				@followUpStatus IS NULL
                  				OR "FollowUpStatus" = @followUpStatus
                  			)
                            AND (@level1 IS NULL OR L."Level1" = @level1)
                            AND (@level2 IS NULL OR L."Level2" = @level2)
                            AND (@level3 IS NULL OR L."Level3" = @level3)
                            AND (@level4 IS NULL OR L."Level4" = @level4)
                            AND (@level5 IS NULL OR L."Level5" = @level5)
                            AND (@hasFlaggedAnswers is NULL OR @hasFlaggedAnswers = false OR 1 = 2)
                            AND (@formId IS NULL OR CR."FormId" = @formId)
                            AND (@questionsAnswered IS NULL 
                              OR (@questionsAnswered = 'All' AND F."NumberOfQuestions" = CR."NumberOfQuestionsAnswered")
                              OR (@questionsAnswered = 'Some' AND F."NumberOfQuestions" <> CR."NumberOfQuestionsAnswered")
                              OR (@questionsAnswered = 'None' AND CR."NumberOfQuestionsAnswered" = 0))
                            AND (@hasAttachments is NULL
                                OR ((SELECT COUNT(1) FROM "CitizenReportAttachments" WHERE "CitizenReportId" = CR."Id" AND "IsDeleted" = false AND "IsCompleted" = true) = 0 AND @hasAttachments = false) 
                                OR ((SELECT COUNT(1) FROM "CitizenReportAttachments" WHERE "CitizenReportId" = CR."Id" AND "IsDeleted" = false AND "IsCompleted" = true) > 0 AND @hasAttachments = true))
                            AND (@hasNotes is NULL 
                                OR ((SELECT COUNT(1) FROM "CitizenReportNotes" WHERE "CitizenReportId" = CR."Id") = 0 AND @hasNotes = false) 
                                OR ((SELECT COUNT(1) FROM "CitizenReportNotes" WHERE "CitizenReportId" = CR."Id") > 0 AND @hasNotes = true))
                            )
                  SELECT
                  	"CitizenReportId",
                  	"TimeSubmitted",
                  	"FormCode",
                  	"FormName",
                  	"FormDefaultLanguage",
                  	"NumberOfQuestionsAnswered",
                  	"NumberOfFlaggedAnswers",
                  	"NotesCount",
                  	"MediaFilesCount",
                  	"FollowUpStatus",
                  	"Level1",
                  	"Level2",
                  	"Level3",
                  	"Level4",
                  	"Level5"
                  FROM
                  	CITIZENREPORTS
                  ORDER BY
                  	CASE
                  		WHEN @sortExpression = 'TimeSubmitted ASC' THEN "TimeSubmitted"
                  	END ASC,
                  	CASE
                  		WHEN @sortExpression = 'TimeSubmitted DESC' THEN "TimeSubmitted"
                  	END DESC,
                  	CASE
                  		WHEN @sortExpression = 'FormCode ASC' THEN "FormCode"
                  	END ASC,
                  	CASE
                  		WHEN @sortExpression = 'FormCode DESC' THEN "FormCode"
                  	END DESC,
                  	CASE
                  		WHEN @sortExpression = 'NumberOfQuestionsAnswered ASC' THEN "NumberOfQuestionsAnswered"
                  	END ASC,
                  	CASE
                  		WHEN @sortExpression = 'NumberOfQuestionsAnswered DESC' THEN "NumberOfQuestionsAnswered"
                  	END DESC,
                  	CASE
                  		WHEN @sortExpression = 'NumberOfFlaggedAnswers ASC' THEN "NumberOfFlaggedAnswers"
                  	END ASC,
                  	CASE
                  		WHEN @sortExpression = 'NumberOfFlaggedAnswers DESC' THEN "NumberOfFlaggedAnswers"
                  	END DESC,
                  	CASE
                  		WHEN @sortExpression = 'MediaFilesCount ASC' THEN "MediaFilesCount"
                  	END ASC,
                  	CASE
                  		WHEN @sortExpression = 'MediaFilesCount DESC' THEN "MediaFilesCount"
                  	END DESC,
                  	CASE
                  		WHEN @sortExpression = 'NotesCount ASC' THEN "NotesCount"
                  	END ASC,
                  	CASE
                  		WHEN @sortExpression = 'NotesCount DESC' THEN "NotesCount"
                  	END DESC,
                  	CASE WHEN @sortExpression = 'Level1 ASC' THEN "Level1" END ASC,
                  	CASE WHEN @sortExpression = 'Level1 DESC' THEN "Level1" END DESC,
                  
                  	CASE WHEN @sortExpression = 'Level2 ASC' THEN "Level2" END ASC,
                  	CASE WHEN @sortExpression = 'Level2 DESC' THEN "Level2" END DESC,
                  
                  	CASE WHEN @sortExpression = 'Level3 ASC' THEN "Level3" END ASC,
                  	CASE WHEN @sortExpression = 'Level3 DESC' THEN "Level3" END DESC,
                  
                  	CASE WHEN @sortExpression = 'Level4 ASC' THEN "Level4" END ASC,
                  	CASE WHEN @sortExpression = 'Level4 DESC' THEN "Level4" END DESC,
                  
                  	CASE WHEN @sortExpression = 'Level5 ASC' THEN "Level5" END ASC,
                  	CASE WHEN @sortExpression = 'Level5 DESC' THEN "Level5" END DESC
                  OFFSET
                  	@offset
                  	ROWS
                  FETCH NEXT
                  	@pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            followUpStatus = req.FollowUpStatus?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            formId = req.FormId,
            hasAttachments = req.HasAttachments,
            hasNotes = req.HasNotes,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
        };

        int totalRowCount;
        List<CitizenReportEntryModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<CitizenReportEntryModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<CitizenReportEntryModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(CitizenReportEntryModel.TimeSubmitted)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.FormCode)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.NumberOfQuestionsAnswered)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.MediaFilesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.MediaFilesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NotesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.NotesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.TimeSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.TimeSubmitted)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.Level1)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.Level2)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.Level3)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.Level4)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.Level5)} {sortOrder}";
        }

        return $"{nameof(CitizenReportEntryModel.TimeSubmitted)} DESC";
    }
}