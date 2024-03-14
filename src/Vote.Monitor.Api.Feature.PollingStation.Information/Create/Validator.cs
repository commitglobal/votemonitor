using Vote.Monitor.Api.Feature.PollingStation.Information.Models;
using Vote.Monitor.Api.Feature.PollingStation.Information.Validators;

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
                v.Add<RatingAnswerModel>(new RatingAnswerModelValidator());
                v.Add<MultiSelectAnswerModel>(new MultiSelectAnswerModelValidator());
                v.Add<SingleSelectAnswerModel>(new SingleSelectAnswerModelValidator());
                v.Add<DateAnswerModel>(new DateAnswerModelValidator());
                v.Add<NumberAnswerModel>(new NumberAnswerModelValidator());
                v.Add<TextAnswerModel>(new TextAnswerModelValidator());
            });
    }
}
