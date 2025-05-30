﻿using Vote.Monitor.Core.Models;

namespace Feature.PollingStations.List;
public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [QueryParam]
    public string? AddressFilter { get; set; }

    [QueryParam]
    public string? Level1Filter { get; set; }

    [QueryParam]
    public string? Level2Filter { get; set; }

    [QueryParam]
    public string? Level3Filter { get; set; }

    [QueryParam]
    public string? Level4Filter { get; set; }

    [QueryParam]
    public string? Level5Filter { get; set; }
    
    [QueryParam]
    public string? PollingStationNumberFilter { get; set; }
}
