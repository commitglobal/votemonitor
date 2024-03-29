﻿namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public sealed class GetPollingStationByIdSpecification : Specification<PollingStationAggregate>,
    ISingleResultSpecification<PollingStationAggregate>
{
    public GetPollingStationByIdSpecification(Guid id)
    {
        Query.Where(pollingStation => pollingStation.Id == id);
    }
}
