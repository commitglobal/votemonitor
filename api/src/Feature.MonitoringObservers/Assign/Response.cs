﻿namespace Feature.MonitoringObservers.Assign;
public record Response
{
    public Guid Id { get; init; }
    public Guid InviterNgoId { get; init; }
    public Guid ObserverId { get; init; }
}
