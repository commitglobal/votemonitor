namespace Vote.Monitor.Domain.Entities.SavedViewAggregate;

/// <summary>Who sees a saved view: every member of its NGO, or only its creator.</summary>
[JsonConverter(typeof(SmartEnumValueConverter<SavedViewAccess, string>))]
public sealed class SavedViewAccess : SmartEnum<SavedViewAccess, string>
{
    /// <summary>Every member of the NGO (admins and staff) sees it.</summary>
    public static readonly SavedViewAccess Ngo = new(nameof(Ngo), nameof(Ngo));

    /// <summary>Only its creator sees it.</summary>
    public static readonly SavedViewAccess Private = new(nameof(Private), nameof(Private));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    public static bool TryParse(string value, out SavedViewAccess result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private SavedViewAccess(string name, string value) : base(name, value)
    {
    }
}
