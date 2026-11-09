using Feature.Labels.Models;
using Feature.Labels.Services;

namespace Feature.Labels.List;

public class Endpoint(IAuthorizationService authorizationService, ILabelService labelService)
    : Endpoint<Request, Results<Ok<LabelsModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/labels");
        DontAutoTag();
        Options(x => x.WithTags("labels"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists every label on the caller's NGO's records in the round, A to Z"; });
    }

    public override async Task<Results<Ok<LabelsModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorization = await authorizationService.AuthorizeAsync(User, null,
            new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorization.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var labels = await labelService.ListAsync(req.ElectionRoundId, req.NgoId, ct);
        return TypedResults.Ok(new LabelsModel(labels));
    }
}
