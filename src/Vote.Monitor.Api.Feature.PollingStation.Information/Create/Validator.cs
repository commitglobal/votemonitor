using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Answer.Module.Validators;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.PollingStationId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(v =>
            {
                v.Add<RatingAnswerRequest>(new RatingAnswerRequestValidator());
                v.Add<MultiSelectAnswerRequest>(new MultiSelectAnswerRequestValidator());
                v.Add<SingleSelectAnswerRequest>(new SingleSelectAnswerRequestValidator());
                v.Add<DateAnswerRequest>(new DateAnswerRequestValidator());
                v.Add<NumberAnswerRequest>(new NumberAnswerRequestValidator());
                v.Add<TextAnswerRequest>(new TextAnswerRequestValidator());
            });
    }
}
