using Amazon.SimpleEmail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Mailing.Contracts;

namespace Vote.Monitor.Core.Services.Mailing.Ses;
internal static class Installer
{
    internal static IServiceCollection AddSes(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SesOptions>(configuration);
        string awsRegion = configuration.GetSection("AWSRegion").Value!;
        string awsAccessKey = configuration.GetSection("AWSAccessKey").Value!;
        string awsSecretKey = configuration.GetSection("AWSSecretKey").Value!;

        var region = Amazon.RegionEndpoint.GetBySystemName(awsRegion);

        services.AddSingleton<IAmazonSimpleEmailService>(new AmazonSimpleEmailServiceClient(awsAccessKey, awsSecretKey, 
            region));

        services.AddSingleton<IMailService, SesMailService>();

        return services;
    }
}
