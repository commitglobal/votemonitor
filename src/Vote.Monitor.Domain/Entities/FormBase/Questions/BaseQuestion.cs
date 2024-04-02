using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextQuestion), QuestionTypes.TextQuestionType)]
[PolyJsonConverter.SubType(typeof(NumberQuestion), QuestionTypes.NumberQuestionType)]
[PolyJsonConverter.SubType(typeof(DateQuestion), QuestionTypes.DateQuestionType)]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestion), QuestionTypes.SingleSelectQuestionType)]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestion), QuestionTypes.MultiSelectQuestionType)]
[PolyJsonConverter.SubType(typeof(RatingQuestion), QuestionTypes.RatingQuestionType)]
public abstract class BaseQuestion
{
    [JsonPropertyName("$questionType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Text { get; private set; }
    public TranslatedString? Helptext { get; private set; }

    [JsonConstructor]
    internal BaseQuestion(Guid id, string code, TranslatedString text, TranslatedString? helptext)
    {
        Id = id;
        Code = code;
        Text = text;
        Helptext = helptext;
    }
}
