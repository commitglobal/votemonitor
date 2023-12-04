namespace Vote.Monitor.Api.Feature.Observer.Services;
internal class ObserverImportModelValidator : Validator<ObserverImportModel>
{
    public override ValidationResult Validate(FluentValidation.ValidationContext<ObserverImportModel> context)
    {

        var _rowindex = context.RootContextData["RowIndex"];
        RuleFor(x => x.Name)
          .MinimumLength(3)
          .WithMessage(x => $"Row {_rowindex}: {nameof(x.Name)}  should have at least 3 chars");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(x => $"Row {_rowindex}: {nameof(x.Email)}  it is not a valid email address");

        RuleFor(x => x.Password)
            .MinimumLength(3)
            .WithMessage(x => $"Row {_rowindex}: {nameof(x.Password)}  should have at least 3 chars");

        RuleFor(x => x.PhoneNumber)
           .MinimumLength(8) //lenhth of phone number can be 8 or 9
           .WithMessage(x => $"Row {_rowindex}: {nameof(x.PhoneNumber)}  should be at least 8 chars");
        return base.Validate(context);

    }
}
