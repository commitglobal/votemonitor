using Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.List;

public class Endpoint(IReadRepository<PollingStationInformation> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/information/");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetPollingStationInformationForNgoSpecification(req);
        var infos = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(new Response
        {
            Informations = infos
        });
    }
}
