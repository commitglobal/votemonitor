using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.Answers.Notes;

public static class AnswerNotesInstaller
{
    public static IServiceCollection AddAnswerNotesFeature(this IServiceCollection services)
    {
        return services;
    }
}
