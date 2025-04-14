using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record MatrixRow
{
    public Guid Id { get; private set; }
    public TranslatedString Text { get; private set; }

    [JsonConstructor]
    internal MatrixRow(Guid id, TranslatedString text)
    {
        Id = id;
        Text = text;
    }

    public static MatrixRow Create(Guid id, TranslatedString text)
        => new(id, text);
    
    public void AddTranslation(string languageCode)
    {
        Text.AddTranslation(languageCode);
    }

    public void RemoveTranslation(string languageCode)
    {
        Text.RemoveTranslation(languageCode);
    }

    public void TrimTranslations(IEnumerable<string> languages)
    {
        Text.TrimTranslations(languages);
    }
}
