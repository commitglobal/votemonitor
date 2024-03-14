using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.List;

public class Endpoint(IReadRepository<PollingStationInfo> repository) : Endpoint<Request, Results<Ok<Response>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/information");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
    }

    public override async Task<Results<Ok<Response>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
