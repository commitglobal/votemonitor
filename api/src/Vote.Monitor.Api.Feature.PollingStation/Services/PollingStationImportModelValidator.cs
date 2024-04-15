namespace Vote.Monitor.Api.Feature.PollingStation.Services;

public class PollingStationImportModelValidator : Validator<PollingStationImportModel>
{
    public PollingStationImportModelValidator()
    {


        RuleFor(x => x.Level1)
            .NotEmpty();
      
        RuleFor(x => x.Level2)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Level3))
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. {PropertyName} should not be empty.");
            

        RuleFor(x => x.Level3)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Level4))
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. {PropertyName} should not be empty.");
            
        RuleFor(x => x.Level4)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Level5))
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. {PropertyName} should not be empty.");
            
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. {PropertyName} should not be empty.");


        RuleFor(x => x.DisplayOrder)
            .Must((_, displayOrder, context) =>
            {
                context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);
                return displayOrder >= 0;
            })
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. {PropertyName} should be greater than 0.");

        RuleFor(x => x.Address)
            .Must((_, address, context) =>
            {
                context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);
                return !string.IsNullOrWhiteSpace(address);
            })
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. {PropertyName} should be not be empty.");

        //RuleFor(x => x.Tags)
        //    .Must((_, tags, context) =>
        //    {
        //        context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);
        //        return tags != null && tags.Any();
        //    })
        //    .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. At least one value for {PropertyName} is required.");

        //RuleFor(x => x.Tags)
        //    .Must((_, tags, context) =>
        //    {
        //        context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);
        //        return tags.Select(x => x.Name).All(tag => !string.IsNullOrWhiteSpace(tag));
        //    })
        //    .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. Tag name is empty.")
        //    .When(x => x.Tags != null && x.Tags.Any());

        //RuleFor(x => x.Tags)
        //    .Must((_, tags, context) =>
        //    {
        //        context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);

        //        var duplicatedTagName = tags
        //            .GroupBy(t => t.Name)
        //            .FirstOrDefault(group => group.Count() > 1)?.Key;

        //        context.MessageFormatter.AppendArgument("DuplicatedTagName", duplicatedTagName);

        //        return string.IsNullOrWhiteSpace(duplicatedTagName);
        //    })
        //    .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. Duplicated tag name found '{DuplicatedTagName}'.")
        //    .When(x => x.Tags != null && x.Tags.Any());
    }
}
