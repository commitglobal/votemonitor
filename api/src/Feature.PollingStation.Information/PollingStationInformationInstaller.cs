using Feature.PollingStation.Information.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.PollingStation.Information;

public static class PollingStationInformationInstaller
{
    public static IServiceCollection AddPollingStationInformationFeature(this IServiceCollection services)
    {
        services.AddScoped<IRelatedDataQueryService, RelatedDataQueryService>();

        return services;
    }
}
