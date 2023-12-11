using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public abstract class BaseQuestionModel
{
    public string Headline { get; }
    public string Subheader { get; }
    public QuetionType Type { get; }
    public bool IsOptional { get; }
    public bool IsFlagged { get; }


    protected BaseQuestionModel(string headline, string subheader, QuetionType type, bool isOptional, bool isFlagged)
    {
        Headline = headline;
        Subheader = subheader;
        Type = type;
        IsOptional = isOptional;
        IsFlagged = isFlagged;
    }
}
