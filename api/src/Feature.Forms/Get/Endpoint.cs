using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;

namespace Feature.Forms.Get;

public class Endpoint(
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider roleProvider,
    IAuthorizationService authorizationService,
    IReadRepository<FormAggregate> repository) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<FormFullModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminOrObserverRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        FormAggregate? form = null;

        var specification = new GetFormByIdSpecification(req.ElectionRoundId, req.Id);
        form = await repository.FirstOrDefaultAsync(specification, ct);
        
        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(FormFullModel.FromEntity(form));
    }
}