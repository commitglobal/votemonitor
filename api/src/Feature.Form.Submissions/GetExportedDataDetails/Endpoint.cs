using Authorization.Policies;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.Form.Submissions.GetExportedDataDetails;

public class Endpoint(IAuthorizationService authorizationService, IReadRepository<ExportedData> repository) : Endpoint<Request, Results<Ok<ExportedDataDetails>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:getExportedDataDetails");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s =>
        {
            s.Summary = "Gets details about an exported data";
        });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<ExportedDataDetails>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));

        if (!result.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetExportedDataDetailsSpecification(req.ElectionRoundId, req.NgoId, req.ExportedDataId);

        var exportedDataDetails = await repository.SingleOrDefaultAsync(specification, ct);

        if (exportedDataDetails != null)
        {
            return TypedResults.Ok(exportedDataDetails);
        }

        return TypedResults.NotFound();

    }
}
