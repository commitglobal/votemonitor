using Feature.Notifications.RecipientsV2;

namespace Feature.Notifications.SendV2;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Body).NotEmpty();

        // Compile up front so an invalid filter is a 400 instead of failing the query.
        RuleFor(x => x.Filter).Custom((_, context) =>
        {
            if (RecipientsV2Query.Validate(context.InstanceToValidate) is { } error)
            {
                context.AddFailure(error);
            }
        });
    }
}
