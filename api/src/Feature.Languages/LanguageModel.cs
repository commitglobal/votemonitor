namespace Feature.Languages;

public record LanguageModel
{
    public Guid Id { get; init; }

    /// <summary>
    /// Two-letter language code (ISO 639-1)
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// English language name
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Native language name
    /// </summary>
    public required string NativeName { get; init; }
}
