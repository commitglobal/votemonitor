using Amazon.Runtime;
using Amazon.S3;
using Humanizer.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.S3;

internal static class Installer
{
    internal static IServiceCollection AddS3FileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3Options>(configuration);

        services.AddSingleton<IAmazonS3>(_ =>
        {
            string awsRegion = configuration.GetSection("AWSRegion").Value!;
            string awsAccessKey = configuration.GetSection("AWSAccessKey").Value!;
            string awsSecretKey = configuration.GetSection("AWSSecretKey").Value!;

            var region = Amazon.RegionEndpoint.GetBySystemName(awsRegion);
            var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);

            return new AmazonS3Client(credentials, region);

        });
        services.AddSingleton<IFileStorageService, S3FileStorageService>();

        return services;
    }
}
