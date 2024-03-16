using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.InformationForm.Delete;

public class Endpoint(IRepository<PollingStationInformationForm> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-station-information-form/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information-form"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
