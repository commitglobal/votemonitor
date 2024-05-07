using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Upsert;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.QuickReportLocationType).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(1024);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(10000);

        RuleFor(x => x.PollingStationId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .When(x => x.QuickReportLocationType == QuickReportLocationType.VisitedPollingStation);

        RuleFor(x => x.PollingStationDetails)
            .NotEmpty()
            .MaximumLength(1024)
            .When(x => x.QuickReportLocationType == QuickReportLocationType.OtherPollingStation);


        RuleFor(x => x.Description).NotEmpty().MaximumLength(10000);
    }
}
