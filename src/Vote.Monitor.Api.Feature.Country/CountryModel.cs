namespace Vote.Monitor.Api.Feature.Country;

public record CountryModel
{
    public Guid Id { get; init; }

    /// <summary>
    /// Two-letter country code (ISO 3166-1 alpha-2)
    /// </summary>
    public required string Iso2 { get; init; }

    /// <summary>
    /// Three-letter country code (ISO 3166-1 alpha-3)
    /// </summary>
    public required string Iso3 { get; init; }

    /// <summary>
    /// Three-digit country number (ISO 3166-1 numeric)
    /// </summary>
    public required string NumericCode { get; init; }

    /// <summary>
    /// English country name
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Full English country name
    /// </summary>
    public required string FullName { get; init; }
}
