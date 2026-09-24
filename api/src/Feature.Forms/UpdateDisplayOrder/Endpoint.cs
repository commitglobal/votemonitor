using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.UpdateDisplayOrder;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/forms:display-order");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Updates display order for published forms belonging to the current monitoring NGO";
        });
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var formIds = req.Forms.Select(x => x.FormId).ToList();
        var forms = await formsRepository.ListAsync(
            new GetFormsByIdsSpecification(req.ElectionRoundId, req.NgoId, formIds), ct);

        if (forms.Count != formIds.Count)
        {
            return TypedResults.NotFound();
        }

        if (forms.Any(x => x.Status != FormStatus.Published))
        {
            AddError("All forms must be published.");
            return new ProblemDetails(ValidationFailures);
        }

        var displayOrders = req.Forms.ToDictionary(x => x.FormId, x => x.DisplayOrder);
        foreach (var form in forms)
        {
            form.UpdateDisplayOrder(displayOrders[form.Id]);
        }

        await formsRepository.UpdateRangeAsync(forms, ct);

        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(
            new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdateFormVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
