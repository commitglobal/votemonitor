using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record MatrixOption
{
    public Guid Id { get; private set; }
    public TranslatedString Text { get; private set; }
    public bool IsFlagged { get; private set; }

    [JsonConstructor]
    internal MatrixOption(Guid id, TranslatedString text, bool isFlagged)
    {
        Id = id;
        Text = text;
        IsFlagged = isFlagged;
    }

    public static MatrixOption Create(Guid id,
        TranslatedString text, bool isFlagged)
        => new(id, text, isFlagged);
    
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
