﻿namespace Vote.Monitor.Api.Feature.PollingStation.Import;

public class Request
{
    public required Guid ElectionRoundId { get; set; }
    public required IFormFile File { get; set; }
}
