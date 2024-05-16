namespace Vote.Monitor.Domain.Entities.NotificationStubAggregate;

public class NotificationStub : BaseEntity
{
    public NotificationStubType StubType { get; private set; }
    public string SerializedData { get; private set; }
    public bool HasBeenProcessed { get; private set; }

    public NotificationStub(NotificationStubType stubType, string serializedData) : base(Guid.NewGuid())
    {
        StubType = stubType;
        SerializedData = serializedData;
        CreatedOn = DateTime.UtcNow;
    }

    public static NotificationStub CreateExpoNotificationStub(string serializedData)
        => new(NotificationStubType.Expo, serializedData);

    public static NotificationStub CreateFirebaseNotificationStub(string serializedData)
        => new(NotificationStubType.Firebase, serializedData);

    public void MarkAsProcessed()
    {
        HasBeenProcessed = true;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    public NotificationStub()
    {
    }
#pragma warning restore CS8618
}
