using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record SingleSelectQuestion : BaseQuestion
{
    public IReadOnlyList<SelectOption> Options { get; private set; }

    [JsonConstructor]
    internal SingleSelectQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options,
        DisplayLogic? displayLogic) : base(id, code, text, helptext, displayLogic)
    {
        Options = options.ToList().AsReadOnly();
    }

    public static SingleSelectQuestion Create(Guid id,
        string code,
        TranslatedString text,
        IReadOnlyList<SelectOption> options,
        TranslatedString? helptext = null,
        DisplayLogic? displayLogic = null) =>
        new(id, code, text, helptext, options, displayLogic);

    public virtual bool Equals(SingleSelectQuestion? other)
    {
        return base.Equals(other) && Options.SequenceEqual(other.Options);
    }

    protected override void AddTranslationsInternal(string languageCode)
    {
        foreach (var option in Options)
        {
            option.AddTranslation(languageCode);
        }
    }

    protected override void RemoveTranslationInternal(string languageCode)
    {
        foreach (var option in Options)
        {
            option.RemoveTranslation(languageCode);
        }
    }

    protected override TranslationStatus InternalGetTranslationStatus(string baseLanguageCode, string languageCode)
    {
        return Options.Any(x => string.IsNullOrWhiteSpace(x.Text[languageCode]))
            ? TranslationStatus.MissingTranslations
            : TranslationStatus.Translated;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Options);
    }
}