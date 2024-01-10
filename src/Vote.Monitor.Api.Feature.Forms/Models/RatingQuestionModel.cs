using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public class RatingQuestionModel : BaseQuestionModel
{
    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Range { get; }

    public RatingQuestionModel(
        Guid id,
        string headline, 
        string subheader,
        RatingScale range) : base(id, headline, subheader, QuetionType.Rating)
    {
        Range = range;
    }
}
