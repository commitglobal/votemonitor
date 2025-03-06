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


        services.AddSingleton<IAmazonS3>(new AmazonS3Client(accessKey, secretKey, new AmazonS3Config()
        {
            ServiceURL = endpointUrl, // Set MinIO URL
            ForcePathStyle = true // Required for MinIO
        }));
        services.AddSingleton<IFileStorageService, MiniIOFileStorageService>();

        return services;
    }
}
