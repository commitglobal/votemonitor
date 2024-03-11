namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Get;

public class Endpoint(IReadRepository<PollingStationNoteAggregate> repository) : Endpoint<Request, Results<Ok<NoteModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notes"));
    }

    public override async Task<Results<Ok<NoteModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
