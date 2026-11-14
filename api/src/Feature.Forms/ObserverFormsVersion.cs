using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.Forms;

/// <summary>
/// The cache key of the forms an observer's monitoring NGO hands out (<c>forms:version</c> and <c>forms:fetchAll</c>).
/// An NGO's own <c>FormsVersion</c> changes whenever one of its forms changes, but in a coalition its observers also
/// receive forms owned by other NGOs (<see cref="Vote.Monitor.Domain.Entities.CoalitionAggregate.CoalitionFormAccess"/>):
/// the key therefore also follows those owners' versions and which forms are shared, so editing a published shared
/// form or granting access to one gives every receiving NGO a new key.
/// </summary>
public static class ObserverFormsVersion
{
    public static async Task<string> GetAsync(VoteMonitorContext context, Guid electionRoundId, Guid monitoringNgoId,
        Guid ownFormsVersion, CancellationToken ct)
    {
        var shared = await context.CoalitionFormAccess
            .Where(x => x.MonitoringNgoId == monitoringNgoId && x.Coalition.ElectionRoundId == electionRoundId)
            .Where(x => x.Form.FormType != FormType.CitizenReporting)
            .Select(x => new { x.FormId, x.Form.MonitoringNgo.FormsVersion })
            .AsNoTracking()
            .ToListAsync(ct);

        // Outside a coalition the key stays the NGO's own version, as before.
        if (shared.Count == 0)
        {
            return ownFormsVersion.ToString();
        }

        var parts = shared
            .Select(x => x.FormsVersion)
            .Distinct()
            .Order()
            .Select(version => $"owner:{version}")
            .Concat(shared.Select(x => x.FormId).Distinct().Order().Select(id => $"form:{id}"))
            .Prepend($"own:{ownFormsVersion}");

        return DeterministicGuid.Create(string.Join('|', parts)).ToString();
    }
}
