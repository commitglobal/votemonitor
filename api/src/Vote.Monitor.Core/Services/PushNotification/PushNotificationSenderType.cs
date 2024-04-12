using System.Runtime.Serialization;

namespace Vote.Monitor.Core.Services.PushNotification;

public enum PushNotificationSenderType
{

    [EnumMember(Value = "Noop")]
    Noop,

    [EnumMember(Value = "Firebase")]
    Firebase
}
