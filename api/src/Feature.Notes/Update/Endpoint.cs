using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Notes.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<NoteAggregate> repository)
    : Endpoint<Request, Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s =>
        {
            s.Summary = "Updates a note for a question in form";
        });
    }

    public override async Task<Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var note = await repository.GetByIdAsync(req.Id, ct);

        if (note == null)
        {
            return TypedResults.NotFound();
        }

        note.UpdateText(req.Text);
        await repository.UpdateAsync(note, ct);

        return TypedResults.Ok(new NoteModel
        {
            Id = note.Id,
            ElectionRoundId = note.ElectionRoundId,
            PollingStationId = note.PollingStationId,
            FormId = note.FormId,
            QuestionId = note.QuestionId,
            Text = note.Text,
            CreatedAt = note.CreatedOn,
            UpdatedAt = note.LastModifiedOn
        });
    }
}
