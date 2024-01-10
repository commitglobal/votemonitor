namespace Vote.Monitor.Api.Feature.Forms.Update.Models;

public class RatingQuestionRequest : BaseQuestionRequest
{
    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; set; }
}
