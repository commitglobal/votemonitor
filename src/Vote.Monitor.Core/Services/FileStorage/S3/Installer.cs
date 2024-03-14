using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.S3;

internal static class Installer
{
    internal static IServiceCollection AddS3FileStorage(this IServiceCollection services, IConfiguration configurationSection)
    {
        services.Configure<S3Options>(configurationSection);
        services.AddSingleton<IFileStorageService, S3FileStorageService>();

        return services;
    }
}
