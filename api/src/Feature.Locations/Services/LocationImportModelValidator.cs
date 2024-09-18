namespace Feature.Locations.Services;

public class LocationImportModelValidator : Validator<LocationImportModel>
{
    public LocationImportModelValidator()
    {
        RuleFor(x => x.Level1)
            .NotEmpty();
      
        RuleFor(x => x.Level2)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Level3))
            .WithMessage("Location on row {RowIndex} has invalid {PropertyName}. {PropertyName} should not be empty.");
        
        RuleFor(x => x.Level3)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Level4))
            .WithMessage("Location on row {RowIndex} has invalid {PropertyName}. {PropertyName} should not be empty.");
            
        RuleFor(x => x.Level4)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Level5))
            .WithMessage("Location on row {RowIndex} has invalid {PropertyName}. {PropertyName} should not be empty.");
        
        RuleFor(x => x.DisplayOrder)
            .Must((_, displayOrder, context) =>
            {
                context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);
                return displayOrder >= 0;
            })
            .WithMessage("Location on row {RowIndex} has invalid {PropertyName}. {PropertyName} should be greater than 0.");
    }
}
