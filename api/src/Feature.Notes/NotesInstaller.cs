using Microsoft.Extensions.DependencyInjection;

namespace Feature.Notes;

public static class NotesInstaller
{
    public static IServiceCollection AddNotesFeature(this IServiceCollection services)
    {
        return services;
    }
}
