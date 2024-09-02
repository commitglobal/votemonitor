using Vote.Monitor.Answer.Module.Validators;

namespace Feature.CitizenReports.Upsert;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.CitizenReportId).NotEmpty();
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

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(256)
            .When(x=>!string.IsNullOrWhiteSpace(x.Email));
        
        RuleFor(x => x.ContactInformation)
            .MaximumLength(2048)
            .When(x=>!string.IsNullOrWhiteSpace(x.ContactInformation));
    }
}