using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Domain.Seeders;

public class PlatformAdminSeeder(UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    ILogger<PlatformAdminSeeder> logger) : IAmDbSeeder
{
    public string SectionKey => "PlatformAdminSeeder";

    public async Task SeedAsync()
    {
        var seedOption = new PlatformAdminSeederSettings();
        configuration.GetSection("Seeders").GetSection(SectionKey).Bind(seedOption);

        if (await userManager.FindByEmailAsync(seedOption.Email) is not ApplicationUser adminUser)
        {
            adminUser = ApplicationUser.CreatePlatformAdmin(seedOption.FirstName, seedOption.LastName, seedOption.Email,
                seedOption.PhoneNumber, seedOption.Password);

            logger.LogInformation("Seeding PlatformAdmin {user}", seedOption.Email);

            await userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await userManager.IsInRoleAsync(adminUser, UserRole.PlatformAdmin))
        {
            logger.LogInformation("Assigning PlatformAdminRole for {user}", seedOption.Email);
            await userManager.AddToRoleAsync(adminUser, UserRole.PlatformAdmin);
        }

    }
}
