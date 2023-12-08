namespace Vote.Monitor.Api.Feature.Language;

public record LanguageModel
{
    public Guid Id { get; init; }

    /// <summary>
    /// English language name
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Two-letter language code (ISO 639-1)
    /// </summary>
    public required string Iso1 { get; init; }

    /// <summary>
    /// Three-letter language code (ISO 639-3)
    /// </summary>
    public required string Iso3 { get; init; }
}
