using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;

namespace Feature.Forms.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<FormAggregate> repository) : Endpoint<Request, Results<Ok<PagedResponse<FormSlimModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgoId}/forms");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<FormSlimModel>>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new ListFormsSpecification(req);
        var formTemplates = await repository.ListAsync(specification, ct);
        var formTemplateCount = await repository.CountAsync(specification, ct);

        return TypedResults.Ok(new PagedResponse<FormSlimModel>(formTemplates, formTemplateCount, req.PageNumber, req.PageSize));
    }
}
