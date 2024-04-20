using Authorization.Policies.Requirements;
using Feature.Notes.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notes.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<NoteAggregate> repository,
    IRepository<MonitoringObserver> monitoringObserverRepository)
    : Endpoint<Request, Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s =>
        {
            s.Summary = "Creates a note for a question in form";
        });
    }

    public override async Task<Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringObserverSpecification = new GetMonitoringObserverSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserver = await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        if (monitoringObserver == null)
        {
            AddError(r => r.ObserverId, "Observer not found");
            return TypedResults.BadRequest(new ProblemDetails(ValidationFailures));
        }

        var note = new NoteAggregate(req.ElectionRoundId,
            req.PollingStationId,
            monitoringObserver.Id,
            req.FormId,
            req.QuestionId,
            req.Text);

        await repository.AddAsync(note, ct);

        return TypedResults.Ok(new NoteModel
        {
            Id = note.Id,
            ElectionRoundId = note.ElectionRoundId,
            PollingStationId = note.PollingStationId,
            FormId = note.FormId,
            QuestionId = note.QuestionId,
            Text = note.Text,
            CreatedAt = note.CreatedOn,
            UpdatedAt = null
        });
    }
}
