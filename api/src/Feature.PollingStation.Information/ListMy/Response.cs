﻿namespace Feature.PollingStation.Information.ListMy;

public record Response
{
    public required List<PollingStationInformationModel> Informations { get; init; }
}
