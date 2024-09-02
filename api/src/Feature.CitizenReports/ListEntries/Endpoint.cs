using Dapper;
using Feature.CitizenReports.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListEntries;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, PagedResponse<CitizenReportEntryModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x =>
        {
            x.Summary = "Lists form submissions by entry in our system";
        });
    }

    public override async Task<PagedResponse<CitizenReportEntryModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """

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

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.FormCode), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.FormCode)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NumberOfQuestionsAnswered), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.NumberOfQuestionsAnswered)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NumberOfFlaggedAnswers), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.MediaFilesCount), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.MediaFilesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NotesCount), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.NotesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.TimeSubmitted), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CitizenReportEntryModel.TimeSubmitted)} {sortOrder}";
        }

        return $"{nameof(CitizenReportEntryModel.TimeSubmitted)} DESC";
    }
}
