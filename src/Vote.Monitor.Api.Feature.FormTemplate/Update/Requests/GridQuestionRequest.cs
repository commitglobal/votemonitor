using Ardalis.SmartEnum.SystemTextJson;
using System.Text.Json.Serialization;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class GridQuestionRequest : BaseQuestionRequest
{
    public TranslatedString? ScalePlaceholder { get; set; }
    public bool HasNotKnownColumn { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; set; }
    public List<GridQuestionRowRequest> Rows { get; set; }
}
