using PolyJson;
using Vote.Monitor.Domain.Entities.FormAggregate.OpenQuestion;
using Vote.Monitor.Domain.Entities.FormAggregate.RatingQuestion;

namespace Vote.Monitor.Api.Feature.Forms.Update;

public class Request
{
    //public Guid ElectionRoundId { get; set; }
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public Guid LanguageCode { get; set; }

    public List<BaseQuestionRequest> Questions { get; set; } = new();

    [PolyJsonConverter(distriminatorPropertyName: "$type")]
    [PolyJsonConverter.SubType(typeof(OpenQuestionRequest), "openText")]
    [PolyJsonConverter.SubType(typeof(RatingQuestionRequest), "rating")]
    [PolyJsonConverter.SubType(typeof(MultiResponseQuestionRequest), "multipleChoiceMulti")]
    [PolyJsonConverter.SubType(typeof(SingleResponseQuestionRequest), "multipleChoiceSingle")]
    public class BaseQuestionRequest
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string Subheader { get; set; }
    }

    public class OpenQuestionRequest : BaseQuestionRequest
    {
        public string Placeholder { get; set; }
        public bool LongAnswer { get; set; }
        public OpenQuestionType OpenQuestionType { get; set; }
    }

    public class RatingQuestionRequest : BaseQuestionRequest
    {
        public RatingScale Scale { get; set; }
    }

    public class MultiResponseQuestionRequest : BaseQuestionRequest
    {
        public List<OptionRequest> Options { get; set; }
    }

    public class SingleResponseQuestionRequest : BaseQuestionRequest
    {
        public List<OptionRequest> Options { get; set; }
    }

    public class OptionRequest
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string IsFlagged { get; set; }
    }
}


