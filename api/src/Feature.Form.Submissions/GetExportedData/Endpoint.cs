using System.Data;
using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Form.Submissions.GetExportedData;

public class Endpoint(IAuthorizationService authorizationService, IDbConnection dbConnection) : Endpoint<Request>
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

        var exportedData = await dbConnection.QueryFirstOrDefaultAsync<ExportedDataView>(sql, queryParams);

        if (string.IsNullOrWhiteSpace(exportedData.Base64EncodedData))
        {
            await SendNoContentAsync(ct);
            return;
        }

        var bytes = Convert.FromBase64String(exportedData.Base64EncodedData);
        await SendBytesAsync(bytes, fileName: exportedData.FileName, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", cancellation: ct);
    }
}
