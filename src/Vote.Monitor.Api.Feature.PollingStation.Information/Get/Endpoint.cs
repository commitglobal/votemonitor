﻿using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Get;

public class Endpoint(IReadRepository<PollingStationInfo> repository) : Endpoint<Request, Results<Ok<PollingStationInfoModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
    }

    public override async Task<Results<Ok<PollingStationInfoModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
