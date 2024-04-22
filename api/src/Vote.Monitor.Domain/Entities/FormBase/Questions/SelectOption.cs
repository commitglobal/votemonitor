using System.Text.Json.Serialization;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record SelectOption
{
    public Guid Id { get; private set; }
    public TranslatedString Text { get; private set; }
    public bool IsFlagged { get; private set; }
    public bool IsFreeText { get; private set; }

    [JsonConstructor]
    internal SelectOption(Guid id, TranslatedString text, bool isFreeText, bool isFlagged)
    {
        Id = id;
        Text = text;
        IsFlagged = isFlagged;
        IsFreeText = isFreeText;
    }

    public static SelectOption Create(Guid id, TranslatedString text, bool isFreeText, bool isFlagged) =>
        new(id, text, isFreeText, isFlagged);

    public void AddTranslation(string languageCode)
    {
        Text.AddTranslation(languageCode);
    }

    public void RemoveTranslation(string languageCode)
    {
        Text.RemoveTranslation(languageCode);
    }
}
