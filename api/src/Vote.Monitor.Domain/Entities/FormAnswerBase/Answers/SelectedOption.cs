using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public record SelectedOption
{
    public Guid OptionId { get; private set; }
    public string Text { get; private set; }

    [JsonConstructor]
    internal SelectedOption(Guid optionId, string text)
    {
        OptionId = optionId;
        Text = text;
    }

    public static SelectedOption Create(Guid optionId, string text) => new(optionId, text);
}
