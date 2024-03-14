using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Form.Module.Models;

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
