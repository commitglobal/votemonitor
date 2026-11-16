namespace Feature.ElectionRounds;

/// <summary>A round's English title is optional: left empty, its title stands in (titles already in English).</summary>
public static class EnglishTitle
{
    public static string Or(string? englishTitle, string title) =>
        string.IsNullOrWhiteSpace(englishTitle) ? title : englishTitle;
}
