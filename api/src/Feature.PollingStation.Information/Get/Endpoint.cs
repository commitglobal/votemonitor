﻿using Feature.PollingStation.Information.Specifications;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Get;

public class Endpoint(IReadRepository<PollingStationInformation> repository) : Endpoint<Request, Results<Ok<PollingStationInformationModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information", "mobile"));
        Summary(s => {
            s.Summary = "Gets polling station information submitted for a polling station";
        });
    }

    public override async Task<Results<Ok<PollingStationInformationModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetPollingStationInformationSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId);
        var pollingStationInformation = await repository.FirstOrDefaultAsync(specification, ct);

        if (pollingStationInformation is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(PollingStationInformationModel.FromEntity(pollingStationInformation));
    }
}
