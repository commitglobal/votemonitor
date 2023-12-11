using System;
using static Vote.Monitor.Api.Feature.Forms.Update.Request;

namespace Vote.Monitor.Api.Feature.Forms.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        //RuleFor(x => x.ElectionRoundId)
        //    .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.LanguageCode)
            .NotEmpty();


        RuleForEach(x => x.Questions).SetInheritanceValidator(v =>
        {
            v.Add<OpenQuestionRequest>(new OpenQuestionRequestValidator());
            v.Add<RatingQuestionRequest>(new RatingQuestionRequestValidator());
            v.Add<MultiResponseQuestionRequest>(new MultiResponseQuestionRequestValidator());
            v.Add<SingleResponseQuestionRequest>(new SingleResponseQuestionRequestValidator());
        });
    }
}

public class OpenQuestionRequestValidator : Validator<OpenQuestionRequest>
{
    public OpenQuestionRequestValidator()
    {
    }
}
public class RatingQuestionRequestValidator : Validator<RatingQuestionRequest>
{
    public RatingQuestionRequestValidator()
    {
    }
}
public class MultiResponseQuestionRequestValidator : Validator<MultiResponseQuestionRequest>
{
    public MultiResponseQuestionRequestValidator()
    {
    }
}
public class SingleResponseQuestionRequestValidator : Validator<SingleResponseQuestionRequest>
{
    public SingleResponseQuestionRequestValidator()
    {
    }
}
