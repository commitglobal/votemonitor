using Authorization.Policies.Requirements;
using Feature.Forms.Models;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;
using GetCoalitionFormSpecification = Feature.Forms.Specifications.GetCoalitionFormSpecification;

namespace Feature.Forms.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<FormAggregate> formRepository,
    IReadRepository<Coalition> coalitionRepository) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<FormFullModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var coalitionFormSpecification =
            new GetCoalitionFormSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var ngoFormSpecification =
            new GetFormByIdSpecification(req.ElectionRoundId, req.NgoId, req.Id);

        var form = (await coalitionRepository.FirstOrDefaultAsync(coalitionFormSpecification, ct)) ??
                   (await formRepository.FirstOrDefaultAsync(ngoFormSpecification, ct));

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(FormFullModel.FromEntity(form));
    }
}
