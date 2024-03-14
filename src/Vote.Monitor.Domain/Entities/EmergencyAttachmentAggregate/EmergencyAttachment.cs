using Vote.Monitor.Domain.Entities.EmergencyAggregate;

namespace Vote.Monitor.Domain.Entities.EmergencyAttachmentAggregate;

public class EmergencyAttachment : BaseEntity, IAggregateRoot
{
    public Guid EmergencyId { get; private set; }
    public Emergency Emergency { get; private set; }
    public string Filename { get; private set; }
    public string MimeType { get; private set; }
    public DateTime Timestamp { get; private set; }

    public EmergencyAttachment(Emergency emergency,
        string filename,
        string mimeType,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        Emergency = emergency;
        EmergencyId = emergency.Id;
        Timestamp = timeProvider.UtcNow;
        Filename = filename;
        MimeType = mimeType;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal EmergencyAttachment()
    {
    }
#pragma warning restore CS8618
}
