namespace Vote.Monitor.Api.Feature.PollingStation.Notes.List;

public class Endpoint(IReadRepository<PollingStationNoteAggregate> repository) : Endpoint<Request, Results<Ok<Response>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes"));
    }

    public override async Task<Results<Ok<Response>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
