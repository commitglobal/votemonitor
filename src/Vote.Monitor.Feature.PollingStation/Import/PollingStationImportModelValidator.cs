namespace Vote.Monitor.Feature.PollingStation.Import;

public class PollingStationImportModelValidator : Validator<PollingStationImportModel>
{
    public PollingStationImportModelValidator()
    {
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

        RuleFor(x => x.Tags)
            .Must((_, tags, context) =>
            {
                context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);
                return tags != null && tags.Any();
            })
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. At least one value for {PropertyName} is required.");

        RuleFor(x => x.Tags)
            .Must((_, tags, context) =>
            {
                context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);
                return tags.Select(x => x.Name).All(tag => !string.IsNullOrWhiteSpace(tag));
            })
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. Tag name is empty.")
            .When(x => x.Tags != null && x.Tags.Any());

        RuleFor(x => x.Tags)
            .Must((_, tags, context) =>
            {
                context.MessageFormatter.AppendArgument("RowIndex", context.RootContextData["RowIndex"]);

                var duplicatedTagName = tags
                    .GroupBy(t => t.Name)
                    .FirstOrDefault(group => group.Count() > 1)?.Key;

                context.MessageFormatter.AppendArgument("DuplicatedTagName", duplicatedTagName);

                return string.IsNullOrWhiteSpace(duplicatedTagName);
            })
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. Duplicated tag name found '{DuplicatedTagName}'.")
            .When(x => x.Tags != null && x.Tags.Any());
    }
}
