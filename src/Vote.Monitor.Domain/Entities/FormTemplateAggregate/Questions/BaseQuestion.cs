using System.Text.Json.Serialization;
using PolyJson;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextInputQuestion), "textInputQuestion")]
[PolyJsonConverter.SubType(typeof(NumberInputQuestion), "numberInputQuestion")]
[PolyJsonConverter.SubType(typeof(DateInputQuestion), "dateInputQuestion")]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestion), "singleSelectQuestion")]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestion), "multiSelectQuestion")]
[PolyJsonConverter.SubType(typeof(RatingQuestion), "ratingQuestion")]
public abstract class BaseQuestion
{
    [JsonPropertyName("$questionType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public TranslatedString Text { get; private set; }
    public TranslatedString? Helptext { get; private set; }

    [JsonConstructor]
    internal BaseQuestion(TranslatedString text, TranslatedString? helptext)
    {
        Text = text;
        Helptext = helptext;
    }
}
