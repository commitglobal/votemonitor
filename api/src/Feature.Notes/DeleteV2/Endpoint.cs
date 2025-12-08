using Authorization.Policies.Requirements;
using Feature.Notes.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Notes.DeleteV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<NoteAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/notes/{id}");
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

        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.SubmissionId, req.ObserverId, req.Id), ct);
        if (note == null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(note, ct);

        return TypedResults.NoContent();
    }
}
