using Ardalis.SmartEnum.SystemTextJson;
using System.Text.Json.Serialization;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class GridQuestionModel : BaseQuestionModel
{
    public TranslatedString? ScalePlaceholder { get; init; }
    public bool HasNotKnownColumn { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; init; }
    public List<GridQuestionRowModel> Rows { get; init; } = [];

    public static GridQuestionModel FromEntity(GridQuestion question) =>
        new()
        {
            Text = question.Text,
            Helptext = question.Helptext,
            Scale = question.Scale,
            ScalePlaceholder = question.ScalePlaceholder,
            HasNotKnownColumn = question.HasNotKnownColumn,
            Rows = question.Rows.Select(GridQuestionRowModel.FromEntity).ToList()
        };
}
