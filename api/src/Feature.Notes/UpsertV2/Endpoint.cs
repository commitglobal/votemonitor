using Authorization.Policies.Requirements;
using Feature.Notes.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notes.UpsertV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringObserver> monitoringObserverRepository,
    IRepository<NoteAggregate> repository)
    : Endpoint<Request, Results<Ok<NoteModelV2>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s =>
        {
            s.Summary = "Upserts a note for a question in a form submission";
        });
    }

    public override async Task<Results<Ok<NoteModelV2>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var note = await repository.FirstOrDefaultAsync(
            new GetNoteByIdSpecification(req.ElectionRoundId, req.Submissionid,req.ObserverId, req.Id), ct);
        return note == null ? await AddNoteAsync(req, ct) : await UpdateNoteAsync(note, req, ct);
    }

    private async Task<Results<Ok<NoteModelV2>, NotFound>> UpdateNoteAsync(NoteAggregate note, Request req,
        CancellationToken ct)
    {
        note.UpdateText(req.Text, req.LastUpdatedAt);
        await repository.UpdateAsync(note, ct);

        return TypedResults.Ok(NoteModelV2.FromEntity(note));
    }

    private async Task<Results<Ok<NoteModelV2>, NotFound>> AddNoteAsync(Request req, CancellationToken ct)
    {
        var monitoringObserverSpecification =
            new GetMonitoringObserverIdSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserverId =
            await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        var note = NoteAggregate.Create(req.Id,
            monitoringObserverId,
            req.Submissionid,
            req.QuestionId,
            req.Text,
            req.LastUpdatedAt);

        await repository.AddAsync(note, ct);

        return TypedResults.Ok(NoteModelV2.FromEntity(note));
    }
}
