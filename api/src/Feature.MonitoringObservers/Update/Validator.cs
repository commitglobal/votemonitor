namespace Feature.MonitoringObservers.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Tags)
            .NotEmpty();

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(256);
    }
}
