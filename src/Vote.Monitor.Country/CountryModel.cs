namespace Vote.Monitor.Country;

public record CountryModel
{
    public Guid Id { get; init; }

    /// <summary>
    /// Two-letter country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string Iso2 { get; init; }

    /// <summary>
    /// Three-letter country code (ISO 3166-1 alpha-3)
    /// </summary>
    public string Iso3 { get; init; }

    /// <summary>
    /// Three-digit country number (ISO 3166-1 numeric)
    /// </summary>
    public string NumericCode { get; init; }

    /// <summary>
    /// English country name
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Full English country name
    /// </summary>
    public string FullName { get; init; }
}
