﻿namespace Vote.Monitor.Api.Feature.PollingStation.FetchAll;

public class Response
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<LocationNode> Nodes { get; set; } = [];
}
