using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;


[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextInputQuestionModel), "textInputQuestion")]
[PolyJsonConverter.SubType(typeof(NumberInputQuestionModel), "numberInputQuestion")]
[PolyJsonConverter.SubType(typeof(DateInputQuestionModel), "dateInputQuestion")]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestionModel), "singleSelectQuestion")]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestionModel), "multiSelectQuestion")]
[PolyJsonConverter.SubType(typeof(RatingQuestionModel), "ratingQuestion")]
[PolyJsonConverter.SubType(typeof(GridQuestionModel), "gridQuestion")]
public abstract class BaseQuestionModel
{
    [JsonPropertyName("$questionType")] 
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public TranslatedString Text { get; init; }

    public TranslatedString? Helptext { get; init; }

    public static BaseQuestionModel FromEntity(BaseQuestion question)
    {
        switch (question)
        {
            case TextInputQuestion textInputQuestion:
                return TextInputQuestionModel.FromEntity(textInputQuestion);

            case NumberInputQuestion numberInputQuestionRequest:
                return NumberInputQuestionModel.FromEntity(numberInputQuestionRequest);

            case DateInputQuestion dateInputQuestionRequest:
                return DateInputQuestionModel.FromEntity(dateInputQuestionRequest);

            case SingleSelectQuestion singleSelectQuestionRequest:
                return SingleSelectQuestionModel.FromEntity(singleSelectQuestionRequest);

            case MultiSelectQuestion multiSelectQuestionRequest:
                return MultiSelectQuestionModel.FromEntity(multiSelectQuestionRequest);

            case RatingQuestion ratingQuestionRequest:
                return RatingQuestionModel.FromEntity(ratingQuestionRequest);

            case GridQuestion gridQuestionRequest:
                return GridQuestionModel.FromEntity(gridQuestionRequest);

            default: throw new ApplicationException("Unknown question type");
        }
    }
}
