using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Core.Services.Mailing.Ses;
internal static class Installer
{
    internal static IServiceCollection AddSes(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SesOptions>(configuration);
        throw new NotImplementedException("Not yet implemented");
    }
}
