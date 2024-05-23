using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Form.Submissions.GetExportedData;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:getExportedData");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s =>
        {
            s.Summary = "Gets exported data excel";
        });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));

        if (!result.Succeeded)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var sql = @"SELECT ""Base64EncodedData"", ""FileName""
        FROM ""ExportedData""
        WHERE ""ElectionRoundId"" = @electionRoundId AND ""NgoId"" = @ngoId AND ""Id"" =@exportedDataId";

        var queryParams = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            exportedDataId = req.ExportedDataId
        };

        ExportedDataView exportedData = null;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            exportedData = await dbConnection.QueryFirstOrDefaultAsync<ExportedDataView>(sql, queryParams);
        }

        if (string.IsNullOrWhiteSpace(exportedData?.Base64EncodedData))
        {
            await SendNoContentAsync(ct);
            return;
        }

        var stream = GenerateStreamFromString(exportedData.Base64EncodedData);

        await SendStreamAsync(stream, "import-template.xlsx", stream.Length, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", cancellation: ct);
    }

    private static MemoryStream GenerateStreamFromString(string value)
    {
        return new MemoryStream(Convert.FromBase64String(value));
    }
}
