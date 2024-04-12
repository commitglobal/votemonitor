using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.FileStorage.LocalDisk;
using Vote.Monitor.Core.Services.FileStorage.S3;

namespace Vote.Monitor.Core.Services.FileStorage;
public static class FileStorageInstaller
{
    public const string SectionKey = "FileStorage";
    public static IServiceCollection AddFileStorage(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        switch (configuration.GetValue<FileStorageType>("FileStorageType"))
        {
            case FileStorageType.LocalDisk:
                var localDiskSection = configuration.GetSection(LocalDiskOptions.SectionName);
                return serviceCollection.AddLocalDiskFileStorage(localDiskSection);

            case FileStorageType.S3:
                var blobStorageSection = configuration.GetSection(S3Options.SectionName);
                return serviceCollection.AddS3FileStorage(blobStorageSection);

            default:
                throw new ArgumentException("Unknown configuration for FileStorageType", nameof(configuration));
        }
    }
}
