using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Notes.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<NoteAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s => {
            s.Summary = "Deletes a note";
        });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        var pollingStationNote = await repository.GetByIdAsync(req.Id, ct);
        
        if (pollingStationNote == null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(pollingStationNote, ct);

        return TypedResults.NoContent();
    }
}
