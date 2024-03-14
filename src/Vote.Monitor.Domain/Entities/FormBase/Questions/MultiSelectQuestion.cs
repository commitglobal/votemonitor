using System.Text.Json.Serialization;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public class MultiSelectQuestion : BaseQuestion
{
    public string Code { get; private set; }
    public IReadOnlyList<SelectOption> Options { get; private set; }

    [JsonConstructor]
    private MultiSelectQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options) : base(id, text, helptext)
    {
        Code = code;
        Options = options.ToList().AsReadOnly();
    }

    public static MultiSelectQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options)
        => new(id, code, text, helptext, options);
}
