using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Feature.PollingStation.Import;

public class ImportModelValidator : Validator<ImportModel>
{
    public ImportModelValidator()
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
                return tags.Keys.All(tag => !string.IsNullOrWhiteSpace(tag));
            })
            .WithMessage("Polling station on row {RowIndex} has invalid {PropertyName}. Tag key is empty.")
            .When(x => x.Tags != null && x.Tags.Any());
    }
}
