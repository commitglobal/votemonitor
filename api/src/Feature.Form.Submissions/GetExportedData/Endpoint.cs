using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.GetExportedData;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context) : Endpoint<Request>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:getExportedData");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s =>
        {
            s.Summary = "Enqueues a job to export data and returns job id to poll for results";
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

        var exportedData = await context.Connection.QueryFirstOrDefaultAsync<ExportedDataView>(sql, queryParams);

        if (string.IsNullOrWhiteSpace(exportedData.Base64EncodedData))
        {
            await SendNoContentAsync(ct);
            return;
        }

        var stream = exportedData.Base64EncodedData.ToMemoryStream();
        await SendStreamAsync(stream, exportedData.FileName, stream.Length, cancellation: ct);
    }
}
