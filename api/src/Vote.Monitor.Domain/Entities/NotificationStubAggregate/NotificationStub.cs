namespace Vote.Monitor.Domain.Entities.NotificationStubAggregate;

public class NotificationStub : BaseEntity
{
    public NotificationStubType StubType { get; private set; }
    public string SerializedData { get; private set; }

    public NotificationStub(NotificationStubType stubType, string serializedData) : base(Guid.NewGuid())
    {
        StubType = stubType;
        SerializedData = serializedData;
    }

    public static NotificationStub CreateExpoNotificationStub(string serializedData)
        => new(NotificationStubType.Expo, serializedData);

    public static NotificationStub CreateFirebaseNotificationStub(string serializedData)
        => new(NotificationStubType.Firebase, serializedData);

#pragma warning disable CS8618 // Required by Entity Framework

    public NotificationStub()
    {
    }
#pragma warning restore CS8618
}
