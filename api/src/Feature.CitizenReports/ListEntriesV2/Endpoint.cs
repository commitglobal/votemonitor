using Dapper;
using Feature.CitizenReports.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListEntriesV2;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<PagedResponse<CitizenReportEntryModel>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports:byEntryV2");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(x => { x.Summary = "Lists citizen report submissions by entry (v2)"; });
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

        var compiler = new FilterSqlCompiler(V2ReportFilterFields.CitizenReports);
        var filter = compiler.Build(req.FilterConditions);
        var queryArgs = new DynamicParameters();
        queryArgs.Add("electionRoundId", req.ElectionRoundId);
        queryArgs.Add("ngoId", req.NgoId);
        queryArgs.Add("offset", PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber));
        queryArgs.Add("pageSize", req.PageSize);
        queryArgs.Add("sortExpression", GetSortExpression(req.SortColumnName, req.IsAscendingSorting));
        filter.AddTo(queryArgs);

        var sql = $"""
                  SELECT
                  	COUNT(*) AS COUNT
                  FROM
                  	"GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                  WHERE {filter.Sql};

                  WITH
                  	CITIZENREPORTS AS (
                  		SELECT
                  			s."Id" "CitizenReportId",
                  			COALESCE(s."LastModifiedOn", s."CreatedOn") "TimeSubmitted",
                  			*
                  		FROM
                  			"GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                  		WHERE {filter.Sql}
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
                  OFFSET @offset ROWS
                  FETCH NEXT @pageSize ROWS ONLY;
                  """;

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
