using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public class MultipleChoiceQuestionModel : BaseQuestionModel
{

    public MultipleChoiceQuestionModel(string headline, string subheader, bool isOptional, bool isFlagged) : base(headline, subheader, QuetionType.MultiResponse, isOptional, isFlagged)
    {
    }
}
