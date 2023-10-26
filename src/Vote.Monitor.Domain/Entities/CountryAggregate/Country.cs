using Vote.Monitor.Core;

namespace Vote.Monitor.Domain.Entities.CountryAggregate;

public class Country : BaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private Country()
    {

    }

    /// <summary>
    /// Two-letter country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string Iso2 { get; private set; }

    /// <summary>
    /// Three-letter country code (ISO 3166-1 alpha-3)
    /// </summary>
    public string Iso3 { get; private set; }

    /// <summary>
    /// Three-digit country number (ISO 3166-1 numeric)
    /// </summary>
    public string NumericCode { get; private set; }

    /// <summary>
    /// English country name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Full English country name
    /// </summary>
    public string FullName { get; private set; }
    

    public Country(string iso2, string name, string iso3, string numericCode, string fullName)
    {
        Id = iso2.ToGuid();
        Iso2 = iso2;
        Name = name;
        FullName = fullName;
        Iso3 = iso3;
        NumericCode = numericCode;
    }

    public void UpdateDetails(string iso2, string name, string fullName, string iso3, string numericCode)
    {
        Iso2 = iso2;
        Name = name;
        FullName = fullName;
        Iso3 = iso3;
        NumericCode = numericCode;
    }
}
