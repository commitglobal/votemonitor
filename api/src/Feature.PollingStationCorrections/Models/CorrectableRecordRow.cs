using Vote.Monitor.Core.Helpers;

namespace Feature.PollingStationCorrections.Models;

internal record CorrectableRecordRow
{
    public Guid Id { get; init; }
    public string RecordType { get; init; } = string.Empty;
    public string? ObserverName { get; init; }
    public string NgoName { get; init; } = string.Empty;
    public string? FormCode { get; init; }
    public string? FormName { get; init; }
    public DateTime SubmittedAt { get; init; }
    public Guid PollingStationId { get; init; }
    public string Number { get; init; } = string.Empty;
    public string Level1 { get; init; } = string.Empty;
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }
    public string Address { get; init; } = string.Empty;
    public PollingStationCorrectionModel[] Corrections { get; init; } = [];

    public CorrectableRecordModel ToModel() => new()
    {
        Id = Id,
        RecordType = RecordType,
        ObserverName = PersonalDataMasker.MaskName(ObserverName),
        NgoName = NgoName,
        FormCode = FormCode,
        FormName = FormName,
        SubmittedAt = SubmittedAt,
        PollingStation = new StationRefModel
        {
            Id = PollingStationId,
            Number = Number,
            Level1 = Level1,
            Level2 = Level2,
            Level3 = Level3,
            Level4 = Level4,
            Level5 = Level5,
            Address = Address
        },
        Corrections = Corrections
    };
}
