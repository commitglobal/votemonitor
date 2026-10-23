using Feature.Review;

namespace Feature.Severity.Set;

public class Validator : ReportRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Severity).NotNull();
    }
}
