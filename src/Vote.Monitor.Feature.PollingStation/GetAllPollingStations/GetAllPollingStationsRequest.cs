﻿namespace Vote.Monitor.Feature.PollingStation.GetAllPollingStations;
internal class GetAllPollingStationsRequest
{

    public int PageSize { get; set; } = 25;

    public int Page { get; set; } = 1;

    public string? Filter { get; set; }
}
