using Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Create;

public class Endpoint(IRepository<PollingStationInformation> repository,
    IReadRepository<Domain.Entities.PollingStationAggregate.PollingStation> pollingStationRepository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    IReadRepository<PollingStationInformationForm> formRepository,
    ITimeProvider timeProvider) :
        Endpoint<Request, Results<Ok<PollingStationInformationModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
    }

    public override async Task<Results<Ok<PollingStationInformationModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formSpecification = new GetPollingStationInformationFormSpecification(req.ElectionRoundId, req.FormId);
        var form = await formRepository.FirstOrDefaultAsync(formSpecification, ct);
        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetPollingStationInformationSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId);
        var pollingStationInformation = await repository.FirstOrDefaultAsync(specification, ct);

        if (pollingStationInformation is not null)
        {
            return TypedResults.Ok(PollingStationInformationModel.FromEntity(pollingStationInformation));
        }

        var pollingStationSpecification = new GetPollingStationSpecification(req.ElectionRoundId, req.PollingStationId);
        var pollingStation = await pollingStationRepository.FirstOrDefaultAsync(pollingStationSpecification, ct);
        if (pollingStation is null)
        {
            return TypedResults.NotFound();
        }

        var monitoringObserverSpecification = new GetMonitoringObserverSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserver = await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        pollingStationInformation = form.CreatePollingStationInformation(pollingStation, monitoringObserver, timeProvider);

        await repository.AddAsync(pollingStationInformation, ct);

        return TypedResults.Ok(PollingStationInformationModel.FromEntity(pollingStationInformation));
    }
}
