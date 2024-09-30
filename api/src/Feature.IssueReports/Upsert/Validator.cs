using Vote.Monitor.Answer.Module.Validators;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;

namespace Feature.IssueReports.Upsert;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.IssueReportId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();

        RuleFor(x => x.LocationDescription)
            .NotEmpty()
            .MaximumLength(1024)
            .When(x => x.LocationType == IssueReportLocationType.OtherLocation);

        RuleFor(x => x.PollingStationId)
            .NotNull()
            .NotEqual(Guid.Empty)
            .When(x => x.LocationType == IssueReportLocationType.PollingStation);

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
    }
}