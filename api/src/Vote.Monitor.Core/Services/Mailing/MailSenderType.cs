using System.Runtime.Serialization;

namespace Vote.Monitor.Core.Services.Mailing;

public enum MailSenderType
{
    [EnumMember(Value = "SMTP")]
    SMTP,

    [EnumMember(Value = "SES")]
    SES
}
