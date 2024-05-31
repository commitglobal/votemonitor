namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate;
public class ExportedData : BaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public ExportedDataType ExportedDataType { get; private set; }
    public ExportedDataStatus ExportStatus { get; private set; }
    public string? FileName { get; private set; }
    public string? Base64EncodedData { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private ExportedData(Guid electionRoundId, ExportedDataType exportedDataType, DateTime startedAt) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        ExportStatus = ExportedDataStatus.Started;
        StartedAt = startedAt;
        ExportedDataType = exportedDataType;
    }

    public static ExportedData Create(Guid electionRoundId, ExportedDataType dataType, DateTime startedAt)
    {
        return new ExportedData(electionRoundId, dataType, startedAt);
    }

    public void Fail()
    {
        ExportStatus = ExportedDataStatus.Failed;
    }

    public void Complete(string fileName, string base64EncodedData, DateTime completedAt)
    {
        FileName = fileName;
        Base64EncodedData = base64EncodedData;
        ExportStatus = ExportedDataStatus.Completed;
        CompletedAt = completedAt;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private ExportedData()
    {

    }
#pragma warning restore CS8618

}
