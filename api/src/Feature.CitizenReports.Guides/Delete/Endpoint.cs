using Authorization.Policies.Requirements;
using Feature.CitizenReports.Guides.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.CitizenReports.Guides.Delete;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<CitizenReportGuideAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/citizen-reports-guides/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports-guides"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetObserverGuideByIdSpecification(req.ElectionRoundId, req.Id);
        var observerGuide = await repository.FirstOrDefaultAsync(specification, ct);

        if (observerGuide == null)
        {
            return TypedResults.NotFound();
        }

        observerGuide.Delete();

        await repository.UpdateAsync(observerGuide, ct);

        return TypedResults.NoContent();
    }
}
