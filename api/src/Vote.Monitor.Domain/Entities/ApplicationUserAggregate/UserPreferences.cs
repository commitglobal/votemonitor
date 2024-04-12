using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class UserPreferences
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected UserPreferences()
    {
    }
#pragma warning restore CS8618

    protected UserPreferences(Guid languageId)
    {
        LanguageId = languageId;
    }

    public static UserPreferences Defaults => new(LanguagesList.EN.Id);
    public Guid LanguageId { get; private set; }

    public void Update(Guid languageId)
    {
        LanguageId = languageId;
    }
}
