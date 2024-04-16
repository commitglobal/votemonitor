using System.Runtime.Serialization;

namespace Vote.Monitor.Module.Notifications;

public enum PushNotificationSenderType
{

    [EnumMember(Value = "Noop")]
    Noop,

    [EnumMember(Value = "Firebase")]
    Firebase,

    [EnumMember(Value = "Expo")]
    Expo
}
