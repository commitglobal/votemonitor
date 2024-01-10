using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public abstract class BaseQuestionModel
{
    public Guid Id { get;}
    public string Headline { get; }
    public string Subheader { get; }
    public QuetionType Type { get; }

    protected BaseQuestionModel(Guid id, string headline, string subheader, QuetionType type)
    {
        Id = id;
        Headline = headline;
        Subheader = subheader;
        Type = type;
    }
}
