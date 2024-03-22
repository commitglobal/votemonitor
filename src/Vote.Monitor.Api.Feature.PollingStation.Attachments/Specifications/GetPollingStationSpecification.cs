﻿using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;

public sealed class GetPollingStationSpecification : SingleResultSpecification<PollingStationAggregate>
{
    public GetPollingStationSpecification(Guid pollingStationId)
    {
        Query.Where(p => p.Id == pollingStationId);
    }
}
