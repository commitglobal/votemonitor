namespace Vote.Monitor.Api.Feature.Observer.Parser;
internal class ObserverImportModelValidator : Validator<ObserverImportModel>
{
    public ObserverImportModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);
    }
}
