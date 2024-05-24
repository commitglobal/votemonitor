using Feature.PollingStation.Information.Services;
using Feature.PollingStation.Information.Specifications;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Delete;

public class Endpoint(IRepository<PollingStationInformation> repository, IRelatedDataQueryService queryService) : Endpoint<Request, Results<NoContent, BadRequest, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information", "mobile"));
        Summary(s =>
        {
            s.Summary = "Deletes polling station information submitted for a polling station";
        });
    }

    public override async Task<Results<NoContent, BadRequest, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetPollingStationInformationSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId);
        var pollingStationInformation = await repository.FirstOrDefaultAsync(specification, ct);

        if (pollingStationInformation is null)
        {
            return TypedResults.NotFound();
        }

        var hasData = await queryService.GetHasDataForCurrentPollingStationAsync(req.ElectionRoundId, req.PollingStationId, req.ObserverId, ct);

        if (hasData)
        {
            return TypedResults.BadRequest();
        }

        await repository.DeleteAsync(pollingStationInformation, ct);

        return TypedResults.NoContent();
    }
}
