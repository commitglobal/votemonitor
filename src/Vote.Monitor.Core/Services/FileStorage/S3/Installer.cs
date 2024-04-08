using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.S3;

internal static class Installer
{
    internal static IServiceCollection AddS3FileStorage(this IServiceCollection services, IConfiguration configurationSection)
    {
        services.Configure<S3Options>(configurationSection);

        services.AddAWSService<IAmazonS3>(GetAwsOptions(configurationSection));
        services.AddSingleton<IFileStorageService, S3FileStorageService>();

        return services;
    }

    private static AWSOptions GetAwsOptions(IConfiguration configuration)
    {
        string region = configuration.GetSection("AWSRegion").Value;
        string awsAccessKey = configuration.GetSection("AWSAccessKey").Value;
        string awsSecretKey = configuration.GetSection("AWSSecretKey").Value;

        var awsOptions = new AWSOptions
        {
            Region = Amazon.RegionEndpoint.GetBySystemName(region),
            Credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey)
        };

        return awsOptions;
    }
}
