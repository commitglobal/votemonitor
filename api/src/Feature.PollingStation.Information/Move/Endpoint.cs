using Authorization.Policies;
using Feature.PollingStation.Information.Specifications;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Move;

public class Endpoint(
    IRepository<PollingStationInformation> repository,
    IReadRepository<PollingStationAggregate> pollingStationRepository)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict<string>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/information/{id}:move");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Moves polling station information to another polling station (platform admins)";
        });
    }

    public override async Task<Results<NoContent, NotFound, Conflict<string>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var pollingStationInformation = await repository.FirstOrDefaultAsync(
            new GetPollingStationInformationByIdForUpdateSpecification(req.ElectionRoundId, req.Id), ct);
        if (pollingStationInformation is null)
        {
            return TypedResults.NotFound();
        }

        var pollingStation = await pollingStationRepository.FirstOrDefaultAsync(
            new GetPollingStationSpecification(req.ElectionRoundId, req.PollingStationId), ct);
        if (pollingStation is null)
        {
            return TypedResults.NotFound();
        }

        if (pollingStationInformation.PollingStationId == pollingStation.Id)
        {
            return TypedResults.NoContent();
        }

        // Observers can only have one PSI per polling station (unique index, and the mobile app looks PSI up by polling station).
        var hasPsiAtTarget = await repository.AnyAsync(new HasPollingStationInformationAtPollingStationSpecification(
            req.ElectionRoundId, pollingStation.Id, pollingStationInformation.MonitoringObserverId, pollingStationInformation.Id), ct);
        if (hasPsiAtTarget)
        {
            return TypedResults.Conflict("Observer already has polling station information for the target polling station.");
        }

        pollingStationInformation.MoveToPollingStation(pollingStation);
        await repository.UpdateAsync(pollingStationInformation, ct);

        return TypedResults.NoContent();
    }
}
