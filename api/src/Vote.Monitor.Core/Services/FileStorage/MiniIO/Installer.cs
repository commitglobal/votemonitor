using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.MiniIO;

internal static class Installer
{
    internal static IServiceCollection AddMiniIOFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MiniIOOptions>(configuration);
        string accessKey = configuration.GetSection("AccessKey").Value!;
        string secretKey = configuration.GetSection("SecretKey").Value!;
        string endpointUrl = configuration.GetSection("EndpointUrl").Value!;
        string awsRegion = configuration.GetSection("Region").Value!;

        var region = Amazon.RegionEndpoint.GetBySystemName(awsRegion);

        services.AddSingleton<IAmazonS3>(new AmazonS3Client(accessKey, secretKey, new AmazonS3Config()
        {
            AuthenticationRegion = "eu-central-1",
            RegionEndpoint = region,
            ServiceURL = endpointUrl, // Set MinIO URL
            ForcePathStyle = true // Required for MinIO,
        }));
        services.AddSingleton<IFileStorageService, MiniIOFileStorageService>();

        return services;
    }
}
