namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Create;

public class Endpoint(IRepository<PollingStationNoteAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Results<Ok<NoteModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes"));
    }

    public override async Task<Results<Ok<NoteModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
