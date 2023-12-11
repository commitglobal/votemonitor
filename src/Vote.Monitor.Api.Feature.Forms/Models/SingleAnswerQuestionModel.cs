using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public class SingleAnswerQuestionModel : BaseQuestionModel
{
    public SingleAnswerQuestionModel(string headline, string subheader, bool isOptional, bool isFlagged) : base(headline, subheader, QuetionType.SingleResponse, isOptional, isFlagged)
    {
    }
}
