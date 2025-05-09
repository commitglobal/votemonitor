﻿using Vote.Monitor.Core.Models;

namespace Feature.PollingStations.Services;

public class PollingStationImportModel
{
    public int DisplayOrder { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }
    public string Number { get; set; }
    public string Address { get; set; }

    public List<TagImportModel> Tags { get; set; }
}