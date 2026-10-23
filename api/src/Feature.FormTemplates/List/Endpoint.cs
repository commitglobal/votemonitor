using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.FormTemplates.List;

public class Endpoint(
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<FormTemplateSlimModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/form-templates");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<FormTemplateSlimModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = """
                  SELECT count(*)
                  FROM "GetFormTemplatesList"(@searchText, @type, @status);

                  SELECT *
                  FROM "GetFormTemplatesList"(@searchText, @type, @status)
                  ORDER BY CASE
                               WHEN @sortExpression = 'Code ASC' THEN "Code"
                               END ASC,
                           CASE
                               WHEN @sortExpression = 'Code DESC' THEN "Code"
                               END DESC,
                           CASE
                               WHEN @sortExpression = 'LastModifiedOn ASC' THEN "LastModifiedOn"
                               END ASC,
                           CASE
                               WHEN @sortExpression = 'LastModifiedOn DESC' THEN "LastModifiedOn"
                               END DESC,
                           CASE
                               WHEN @sortExpression = 'FormType ASC' THEN "FormType"
                               END ASC,
                           CASE
                               WHEN @sortExpression = 'FormType DESC' THEN "FormType"
                               END DESC,
                           CASE
                               WHEN @sortExpression = 'Status ASC' THEN "Status"
                               END ASC,
                           CASE
                               WHEN @sortExpression = 'Status DESC' THEN "Status"
                               END DESC
                  OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.FormStatus?.ToString(),
            type = req.FormTemplateType?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount = 0;
        List<FormTemplateSlimModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<FormTemplateSlimModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<FormTemplateSlimModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return "LastModifiedOn ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(FormTemplateSlimModel.Code), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormTemplateSlimModel.Code)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormTemplateSlimModel.LastModifiedOn),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormTemplateSlimModel.LastModifiedOn)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormTemplateSlimModel.FormType), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormTemplateSlimModel.FormType)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormTemplateSlimModel.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormTemplateSlimModel.Status)} {sortOrder}";
        }

        return "LastModifiedOn ASC";
    }
}
