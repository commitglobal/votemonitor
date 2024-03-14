using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextQuestion), "textInputQuestion")]
[PolyJsonConverter.SubType(typeof(NumberQuestion), "numberInputQuestion")]
[PolyJsonConverter.SubType(typeof(DateQuestion), "dateInputQuestion")]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestion), "singleSelectQuestion")]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestion), "multiSelectQuestion")]
[PolyJsonConverter.SubType(typeof(RatingQuestion), "ratingQuestion")]
public abstract class BaseQuestion
{
    [JsonPropertyName("$questionType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid Id { get; private set; }
    public TranslatedString Text { get; private set; }
    public TranslatedString? Helptext { get; private set; }

    [JsonConstructor]
    internal BaseQuestion(Guid id, TranslatedString text, TranslatedString? helptext)
    {
        Id = id;
        Text = text;
        Helptext = helptext;
    }
}
