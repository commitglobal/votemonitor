using Microsoft.Extensions.DependencyInjection;

namespace Feature.Form.Submissions;

public static class FormSubmissionsInstaller
{
    public static IServiceCollection AddFormSubmissionsFeature(this IServiceCollection services)
    {
        return services;
    }
}
