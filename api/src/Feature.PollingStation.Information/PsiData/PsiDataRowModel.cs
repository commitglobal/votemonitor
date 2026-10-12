using Vote.Monitor.Core.Helpers;

namespace Feature.PollingStation.Information.PsiData;

public record PsiDataRowModel
{
    public Guid Id { get; init; }
    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = string.Empty;
    public string Level2 { get; init; } = string.Empty;
    public string Level3 { get; init; } = string.Empty;
    public string Level4 { get; init; } = string.Empty;
    public string Level5 { get; init; } = string.Empty;
    public string Number { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public Guid MonitoringNgoId { get; init; }
    public string NgoName { get; init; } = string.Empty;
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = string.Empty;
    public string ObserverEmail { get; init; } = string.Empty;
    public string ObserverPhoneNumber { get; init; } = string.Empty;
    public DateTime? ArrivalTime { get; init; }
    public DateTime? DepartureTime { get; init; }
    public double MinutesMonitoring { get; init; }
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime LastUpdatedAt { get; init; }

    public PsiDataRowModel WithMaskedObserverData() => this with
    {
        ObserverName = PersonalDataMasker.MaskName(ObserverName),
        ObserverEmail = PersonalDataMasker.MaskEmail(ObserverEmail),
        ObserverPhoneNumber = PersonalDataMasker.MaskPhoneNumber(ObserverPhoneNumber)
    };
}
