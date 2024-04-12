namespace Feature.MonitoringObservers.Parser;
internal class MonitoringObserverImportModelValidator : Validator<MonitoringObserverImportModel>
{
    public MonitoringObserverImportModelValidator()
    {

        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber).MinimumLength(3);
    }
}
