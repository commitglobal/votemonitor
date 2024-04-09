using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Forms.Delete;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<FormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgo}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormByIdSpecification(req.ElectionRoundId, req.MonitoringNgoId, req.Id);
        var form = await repository.FirstOrDefaultAsync(specification, ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(form, ct);

        return TypedResults.NoContent();
    }
}
