using Microsoft.AspNetCore.Identity;

namespace Vote.Monitor.Core.Extensions;

public static class IdentityResultExtensions
{
    public static List<string> GetErrors(this IdentityResult result) =>
        result.Errors.Select(e => e.Description).ToList();

    public static string GetAllErrors(this IdentityResult result) =>
        string.Join(".", result.Errors.Select(e => e.Description).ToList());
}
