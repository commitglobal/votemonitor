using Authorization.Policies.Requirements;
using Feature.Notes.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Notes.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<NoteAggregate> repository)
    : Endpoint<Request, Results<Ok<List<NoteModel>>, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s => {
            s.Summary = "Lists notes for a polling station";
        });
    }

    public override async Task<Results<Ok<List<NoteModel>>, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetNotesSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId, req.FormId);
        var notes = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(notes
            .Select(NoteModel.FromEntity)
            .ToList()
        );
    }
}
