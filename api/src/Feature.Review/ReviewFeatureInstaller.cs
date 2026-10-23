using Feature.Review.Rules;
using Feature.Review.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Review;

public static class ReviewFeatureInstaller
{
    public static IServiceCollection AddReviewFeature(this IServiceCollection services)
    {
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IReviewerRuleRunner, ReviewerRuleRunner>();
        return services;
    }
}
