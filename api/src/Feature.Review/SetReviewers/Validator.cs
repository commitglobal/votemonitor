using Feature.Review.Services;

namespace Feature.Review.SetReviewers;

public class Validator : ReportRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.UserIds).NotNull();
        RuleFor(x => x.UserIds.Distinct().Count())
            .LessThanOrEqualTo(ReviewService.MaxReviewers)
            .OverridePropertyName(nameof(Request.UserIds))
            .WithMessage($"A report can have at most {ReviewService.MaxReviewers} reviewers");
        RuleForEach(x => x.UserIds).NotEmpty();
    }
}
