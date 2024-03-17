using Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Get;

public class Endpoint(IReadRepository<PollingStationInformation> repository) : Endpoint<Request, Results<Ok<PollingStationInformationModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
    }

    public override async Task<Results<Ok<PollingStationInformationModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetPollingStationInformationByIdSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId, req.Id);
        var pollingStationInformation = await repository.FirstOrDefaultAsync(specification, ct);

        if (pollingStationInformation is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(PollingStationInformationModel.FromEntity(pollingStationInformation));
    }
}
