using Amazon.Extensions.NETCore.Setup;
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

        // Get the AWS profile information from configuration providers
        AWSOptions awsOptions = configurationSection.GetAWSOptions();

        // Configure AWS service clients to use these credentials
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();
        services.AddSingleton<IFileStorageService, S3FileStorageService>();

        return services;
    }
}
