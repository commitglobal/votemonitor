using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.Publish;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<FormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgo}/forms/{id}:publish");
        Description(x => x.Accepts<Request>());
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

        var result = form.Publish();

        if (result is PublishResult.InvalidForm validationResult)
        {
            validationResult.Problems.Errors.ForEach(AddError);
            return new ProblemDetails(ValidationFailures);
        }

        await repository.UpdateAsync(form, ct);
        return TypedResults.NoContent();
    }
}
