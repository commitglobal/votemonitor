
using Vote.Monitor.Api.Feature.Forms.Update.Models;

namespace Vote.Monitor.Api.Feature.Forms.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.LanguageId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleForEach(x => x.Questions).SetInheritanceValidator(v =>
        {
            v.Add<BaseQuestionRequest>(new BaseQuestionRequestValidator());
            v.Add<OpenQuestionRequest>(new OpenQuestionRequestValidator());
            v.Add<RatingQuestionRequest>(new RatingQuestionRequestValidator());
            v.Add<MultiResponseQuestionRequest>(new MultiResponseQuestionRequestValidator());
            v.Add<SingleResponseQuestionRequest>(new SingleResponseQuestionRequestValidator());
        });
    }
}

public class BaseQuestionRequestValidator : Validator<BaseQuestionRequest>
{
    public BaseQuestionRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Headline)
            .NotEmpty();

        RuleFor(x => x.Subheader)
            .NotEmpty();
    }
}
public class OpenQuestionRequestValidator : Validator<OpenQuestionRequest>
{
    public OpenQuestionRequestValidator()
    {
        Include(new BaseQuestionRequestValidator());
    }
}

public class RatingQuestionRequestValidator : Validator<RatingQuestionRequest>
{
    public RatingQuestionRequestValidator()
    {
        Include(new BaseQuestionRequestValidator());

    }
}

public class MultiResponseQuestionRequestValidator : Validator<MultiResponseQuestionRequest>
{
    public MultiResponseQuestionRequestValidator()
    {
        Include(new BaseQuestionRequestValidator());

    }
}

public class SingleResponseQuestionRequestValidator : Validator<SingleResponseQuestionRequest>
{
    public SingleResponseQuestionRequestValidator()
    {
        Include(new BaseQuestionRequestValidator());
    }
}
