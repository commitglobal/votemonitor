namespace Vote.Monitor.Api.Feature.PollingStation.Information.List;

public record Response
{
    public required List<PollingStationInformationModel> Informations { get; init; }
}
