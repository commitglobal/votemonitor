namespace Vote.Monitor.Api.Feature.Forms.Update.Models;

[PolyJsonConverter(distriminatorPropertyName: "$type")]
[PolyJsonConverter.SubType(typeof(OpenQuestionRequest), "openText")]
[PolyJsonConverter.SubType(typeof(RatingQuestionRequest), "rating")]
[PolyJsonConverter.SubType(typeof(MultiResponseQuestionRequest), "multipleChoiceMulti")]
[PolyJsonConverter.SubType(typeof(SingleResponseQuestionRequest), "multipleChoiceSingle")]
public class BaseQuestionRequest
{
    public Guid Id { get; set; }
    public string Headline { get; set; }
    public string Subheader { get; set; }
}
