using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.Delete;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository,
    IRepository<FormSubmission> formSubmissionsRepository) : Endpoint<Request, Results<NoContent, NotFound, Conflict>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormByIdSpecification(req.ElectionRoundId, req.Id);
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

        await formsRepository.DeleteAsync(form, ct);

        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdatePollingStationsVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
