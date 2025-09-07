using Authorization.Policies.Requirements;
using Feature.Notes.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Notes.ListV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<NoteAggregate> repository)
    : Endpoint<Request, Results<Ok<List<NoteModelV2>>, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s =>
        {
            s.Summary = "Lists notes for a submission";
        });
    }

    public override async Task<Results<Ok<List<NoteModelV2>>, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetNotesV2Specification(req.ElectionRoundId, req.ObserverId, req.SubmissionId);
        var notes = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(notes
            .Select(note => NoteModelV2.FromEntity(req.ElectionRoundId, note))
            .ToList()
        );
    }
}
