using Vote.Monitor.Answer.Module.Validators;

namespace Feature.PollingStation.Information.Upsert;

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
                v.Add(new RatingAnswerRequestValidator());
                v.Add(new MultiSelectAnswerRequestValidator());
                v.Add(new SingleSelectAnswerRequestValidator());
                v.Add(new DateAnswerRequestValidator());
                v.Add(new NumberAnswerRequestValidator());
                v.Add(new TextAnswerRequestValidator());
            });
        
        RuleForEach(x => x.Breaks)
            .Must(observationBreak =>
                !observationBreak.End.HasValue || observationBreak.Start <= observationBreak.End);
        
        RuleFor(x => x.LastUpdatedAt)
            .Must(BeUtc).WithMessage("LastUpdatedAt must be in UTC format.");
    }

    private bool BeUtc(DateTime? date)
    {
        if (!date.HasValue)
        {
            return true;
        }
        
        return date.Value.Kind == DateTimeKind.Utc;
    }
}
