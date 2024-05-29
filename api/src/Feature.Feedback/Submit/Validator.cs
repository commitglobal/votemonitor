namespace Feature.Feedback.Submit;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.UserFeedback).NotEmpty().MaximumLength(10_000);
        RuleFor(x => x.Metadata).NotNull();
    }
}
