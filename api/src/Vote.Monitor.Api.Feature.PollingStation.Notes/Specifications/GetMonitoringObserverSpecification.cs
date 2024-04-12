﻿using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;
public sealed class GetMonitoringObserverSpecification : Specification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid observerId)
    {
        Query.Where(o => o.ObserverId == observerId);
    }
}
