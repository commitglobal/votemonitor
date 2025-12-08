using Authorization.Policies.Requirements;
using Feature.Notes.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notes.Upsert;

[Obsolete("Will be removed in future version")]
public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringObserver> monitoringObserverRepository,
    IRepository<NoteAggregate> repository,
    ITimeProvider timeProvider)
    : Endpoint<Request, Results<Ok<NoteModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s =>
        {
            s.Summary = "Upserts a note for a question in form";
        });
    }

    public override async Task<Results<Ok<NoteModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var note = await repository.FirstOrDefaultAsync(
            new GetNoteByIdSpecification(req.ElectionRoundId, req.ObserverId, req.Id), ct);
        return note == null ? await AddNoteAsync(req, ct) : await UpdateNoteAsync(note, req, ct);
    }

    private async Task<Results<Ok<NoteModel>, NotFound>> UpdateNoteAsync(NoteAggregate note, Request req,
        CancellationToken ct)
    {
        note.UpdateText(req.Text, req.LastUpdatedAt ?? timeProvider.UtcNow);
        await repository.UpdateAsync(note, ct);

        return TypedResults.Ok(NoteModel.FromEntity(note));
    }

    private async Task<Results<Ok<NoteModel>, NotFound>> AddNoteAsync(Request req, CancellationToken ct)
    {
        var monitoringObserverSpecification =
            new GetMonitoringObserverIdSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserverId =
            await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        var note = new NoteAggregate(req.Id,
            req.ElectionRoundId,
            req.PollingStationId,
            monitoringObserverId,
            req.FormId,
            req.QuestionId,
            req.Text,
            req.LastUpdatedAt ?? timeProvider.UtcNow);

        await repository.AddAsync(note, ct);

        return TypedResults.Ok(NoteModel.FromEntity(note));
    }
}
