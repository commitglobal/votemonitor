using Ardalis.SmartEnum.SystemTextJson;
using System.Text.Json.Serialization;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class RatingQuestionModel : BaseQuestionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; init; }
    public static RatingQuestionModel FromEntity(RatingQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            Scale = question.Scale
        };
}
