namespace Feature.PollingStationCorrections.GetRecord;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public CorrectableRecordType RecordType { get; set; } = null!;

    public Guid RecordId { get; set; }
}
