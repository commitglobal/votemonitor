using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Update;

public class Endpoint(IReadRepository<PollingStationInformation> repository) : Endpoint<Request, Results<Ok<PollingStationInformationModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
    }

    public override async Task<Results<Ok<PollingStationInformationModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
