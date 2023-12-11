using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public class RatingQuestionModel : BaseQuestionModel
{

    public RatingQuestionModel(string headline, string subheader, bool isOptional, bool isFlagged, int range ) : base(headline, subheader, QuetionType.Rating, isOptional, isFlagged)
    {
    }
}
