using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.LocalDisk;

internal static class Installer
{
    internal static IServiceCollection AddLocalDiskFileStorage(this IServiceCollection services, IConfiguration configurationSection)
    {
        services.Configure<LocalDiskOptions>(configurationSection);
        services.AddSingleton<IFileStorageService, LocalDiskFileStorageService>();

        return services;
    }
}
