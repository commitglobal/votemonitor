using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Domain.Entities.LanguageAggregate;

public class Language : BaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private Language()
    {

    }

    /// <summary>
    /// English language name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Two-letter language code (ISO 639-1)
    /// </summary>
    public string Iso1 { get; }

    /// <summary>
    /// Three-letter language code (ISO 639-3)
    /// </summary>
    public string Iso3 { get; }

    /// <summary>
    /// Language type: Living, Extinct, Ancient, Historical, Constructed (only the initial character)
    /// </summary>
    public char LanguageType { get; }

    public Language(string name, string iso1, string iso3, char languageType)
    {
        Id = iso3.ToGuid();
        Name = name;
        Iso1 = iso1;
        Iso3 = iso3;
        LanguageType = languageType;
    }
}
