namespace Vote.Monitor.Domain.Entities.UserNotificationAggregate;

/// <summary>Why a user received an inbox notification.</summary>
[JsonConverter(typeof(SmartEnumValueConverter<UserNotificationReason, string>))]
public sealed class UserNotificationReason : SmartEnum<UserNotificationReason, string>
{
    public static readonly UserNotificationReason AddedAsReviewer = new(nameof(AddedAsReviewer), nameof(AddedAsReviewer));
    public static readonly UserNotificationReason RemovedAsReviewer = new(nameof(RemovedAsReviewer), nameof(RemovedAsReviewer));
    public static readonly UserNotificationReason ReviewingRecordChanged = new(nameof(ReviewingRecordChanged), nameof(ReviewingRecordChanged));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    public static bool TryParse(string value, out UserNotificationReason result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private UserNotificationReason(string name, string value) : base(name, value)
    {
    }
}
