using Dapper;
using Feature.CitizenReports.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListEntries;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<CitizenReportEntryModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x => { x.Summary = "Lists citizen report submissions by entry in our system"; });
    }

    public override async Task<PagedResponse<CitizenReportEntryModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  SELECT
                  	COUNT(*) AS COUNT
                  FROM
                  	"CitizenReports" CR
                  	INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                  	INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                  WHERE
                  	MN."ElectionRoundId" = @electionRoundId
                  	AND MN."NgoId" = @ngoId
                  	AND (
                  		@followUpStatus IS NULL
                  		OR CR."FollowUpStatus" = @followUpStatus
                  	);

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
                  			CR."FollowUpStatus"
                  		FROM
                  			"CitizenReports" CR
                  			INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                  			INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                  			INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
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
                  	"FollowUpStatus"
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
                  	END DESC
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
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<CitizenReportEntryModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<CitizenReportEntryModel>().ToList();
        }

        return new PagedResponse<CitizenReportEntryModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
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

        return $"{nameof(CitizenReportEntryModel.TimeSubmitted)} DESC";
    }
}