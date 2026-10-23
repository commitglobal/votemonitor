using System.Security.Claims;

namespace Feature.Review;

public static class AuthorizationExtensions
{
    /// <summary>
    /// Whether the user is an admin or staff member of an active NGO that can work on this kind of report in the
    /// round: citizen reports need the round's citizen-reporting NGO, other reports any monitoring NGO.
    /// </summary>
    public static async Task<bool> CanWorkOnReportsAsync(this IAuthorizationService authorizationService,
        ClaimsPrincipal user,
        Guid electionRoundId,
        ReportRecordType recordType)
    {
        IAuthorizationRequirement requirement = recordType == ReportRecordType.CitizenReport
            ? new CitizenReportingNgoAdminOrStaffRequirement(electionRoundId)
            : new MonitoringNgoAdminOrStaffRequirement(electionRoundId);

        var result = await authorizationService.AuthorizeAsync(user, null, requirement);
        return result.Succeeded;
    }
}
