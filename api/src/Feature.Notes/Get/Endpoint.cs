using Authorization.Policies.Requirements;
using Feature.Notes.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Notes.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<NoteAggregate> repository)
    : Endpoint<Request, Results<Ok<NoteModel>, BadRequest<ProblemDetails>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets a note for a polling station";
        });
    }

    public override async Task<Results<Ok<NoteModel>, BadRequest<ProblemDetails>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.ObserverId, req.Id), ct);
        if (note is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new NoteModel
        {
            Id = note.Id,
            ElectionRoundId = note.ElectionRoundId,
            PollingStationId = note.PollingStationId,
            FormId = note.FormId,
            QuestionId = note.QuestionId,
            Text = note.Text,
            LastUpdatedAt = note.LastUpdatedAt
        });
    }
}
