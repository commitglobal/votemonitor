using Vote.Monitor.Core.Constants;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class UserPreferences
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected UserPreferences()
    {
    }
#pragma warning restore CS8618

    protected UserPreferences(string languageCode)
    {
        LanguageCode = languageCode;
    }

    public static UserPreferences Defaults => new(LanguagesList.EN.Iso1);
    public string LanguageCode { get; private set; }

    public void Update(string languageCode)
    {
        LanguageCode = languageCode;
    }
}
