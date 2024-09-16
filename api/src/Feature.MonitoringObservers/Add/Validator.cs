using Feature.MonitoringObservers.Parser;

namespace Feature.MonitoringObservers.Add;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleFor(x => x.Observers).NotEmpty();
        RuleForEach(x => x.Observers).SetValidator(new MonitoringObserverImportModelValidator());
    }
}