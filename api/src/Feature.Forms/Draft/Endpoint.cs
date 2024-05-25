using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.Draft;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository,
    IRepository<FormSubmission> formSubmissionsRepository) : Endpoint<Request, Results<NoContent, Conflict, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/forms/{id}:draft");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, Conflict, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormByIdSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var form = await formsRepository.FirstOrDefaultAsync(specification, ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var hasSubmittedAnswers = await formSubmissionsRepository
            .AnyAsync(new GetSubmissionsForFormSpecification(req.ElectionRoundId, req.NgoId, req.Id), ct);
        if (hasSubmittedAnswers)
        {
            return TypedResults.Conflict();
        }
        form.Draft();

        await formsRepository.UpdateAsync(form, ct);

        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdatePollingStationsVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
