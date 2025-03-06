using Vote.Monitor.Answer.Module.Validators;

namespace Feature.Form.Submissions.Upsert;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.PollingStationId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
        
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

        RuleFor(x => x.LastUpdatedAt)
            .Must(BeUtc)
            .WithMessage("LastUpdatedAt must be in UTC format.");
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
