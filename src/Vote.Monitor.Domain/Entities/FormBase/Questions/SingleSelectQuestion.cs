using System.Text.Json.Serialization;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public class SingleSelectQuestion : BaseQuestion
{
    public IReadOnlyList<SelectOption> Options { get; private set; }

    [JsonConstructor]
    internal SingleSelectQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options) : base(id, code, text, helptext)
    {
        Options = options.ToList().AsReadOnly();
    }

    public static SingleSelectQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options) =>
        new(id, code, text, helptext, options);
}
