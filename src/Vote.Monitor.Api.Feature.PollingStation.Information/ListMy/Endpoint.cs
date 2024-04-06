using Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.ListMy;

public class Endpoint(IReadRepository<PollingStationInformation> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/information:my");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets all polling station information by an observer";
            s.Description = "Allows filtering by polling station";
        });
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetPollingStationInformationForObserverSpecification(req.ElectionRoundId, req.ObserverId, req.PollingStationIds);
        var infos = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(new Response
        {
            Informations = infos
        });
    }
}
