using Microsoft.Extensions.DependencyInjection;

namespace Feature.Form.Submission.Comments;

public static class FormSubmissionCommentsInstaller
{
    public static IServiceCollection AddFormSubmissionCommentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
