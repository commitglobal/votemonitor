using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Forms.Draft;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<FormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgo}/forms/{id}:draft");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        form.Draft();

        await repository.UpdateAsync(form, ct);

        return TypedResults.NoContent();
    }
}
