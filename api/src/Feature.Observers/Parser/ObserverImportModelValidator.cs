namespace Feature.Observers.Parser;
internal class ObserverImportModelValidator : Validator<ObserverImportModel>
{
    public ObserverImportModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);
    }
}
