using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Humanizer.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.S3;

internal static class Installer
{
    internal static IServiceCollection AddS3FileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3Options>(configuration);
        string awsRegion = configuration.GetSection("AWSRegion").Value!;
        string awsAccessKey = configuration.GetSection("AWSAccessKey").Value!;
        string awsSecretKey = configuration.GetSection("AWSSecretKey").Value!;

        var region = Amazon.RegionEndpoint.GetBySystemName(awsRegion);
        var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);

        Log.Logger.Warning("start AddDefaultAWSOptions");
        services.AddDefaultAWSOptions(new AWSOptions() { Credentials = credentials, Region = region });
        Log.Logger.Warning("done AddDefaultAWSOptions");

        Log.Logger.Warning("starting aws sdk init {@region} {awsAccessKey} {awsSecretKey}", region, awsAccessKey, awsSecretKey);
        services.AddAWSService<IAmazonS3>(new AWSOptions()
        {
            DefaultConfigurationMode = DefaultConfigurationMode.Standard
        });
        Log.Logger.Warning("done starting aws sdk init");

        services.AddSingleton<IFileStorageService, S3FileStorageService>();

        return services;
    }
}
