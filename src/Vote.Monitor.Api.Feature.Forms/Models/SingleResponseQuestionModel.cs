using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public class SingleResponseQuestionModel : BaseQuestionModel
{
    public List<QuestionOptionModel> Options { get; }

    public SingleResponseQuestionModel(Guid id,
        string headline,
        string subheader,
        List<QuestionOptionModel> options) : base(id, headline, subheader, QuetionType.SingleResponse)
    {
        Options = options;
    }
}
