using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Mailing.Contracts;

namespace Vote.Monitor.Core.Services.Mailing.Smtp;

internal static class Installer
{
    internal static IServiceCollection AddSmtp(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration);
        services.AddSingleton<IMailService, SmtpMailService>();

        return services;
    }
}
