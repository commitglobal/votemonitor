using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace Vote.Monitor.Form.Module.Requests;

public class RatingQuestionRequest : BaseQuestionRequest
{
    [JsonConverter(typeof(SmartEnumNameConverter<RatingScaleModel, string>))]
    public RatingScaleModel Scale { get; set; }
}
