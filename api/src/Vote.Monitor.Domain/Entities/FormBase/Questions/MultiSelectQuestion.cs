using System.Text.Json.Serialization;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record MultiSelectQuestion : BaseQuestion
{
    public IReadOnlyList<SelectOption> Options { get; private set; }

    [JsonConstructor]
    internal MultiSelectQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options) : base(id, code, text, helptext)
    {
        Options = options.ToList().AsReadOnly();
    }

    public static MultiSelectQuestion Create(Guid id,
        string code,
        TranslatedString text,
        IReadOnlyList<SelectOption> options,
        TranslatedString? helptext = null)
        => new(id, code, text, helptext, options);

    public virtual bool Equals(MultiSelectQuestion? other)
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

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Options);
    }
}
