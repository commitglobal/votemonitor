using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Extensions;

public static class PollingStationsExtensions
{
        public static List<LocationNode> ToPollingStationNodes(this List<PollingStationModel> pollingStations)
    {
        Dictionary<string, LocationNode> cache = new();
        var result = new List<LocationNode>();
        int id = 0;

        foreach (var ps in pollingStations)
        {
            var parentNode = cache.GetOrCreate(BuildKey(ps.Level1), () => new LocationNode
            {
                Id = ++id, 
                Name = ps.Level1,
                Depth = 1
            });

            if (!string.IsNullOrWhiteSpace(ps.Level2))
            {
                var level2Key = BuildKey(ps.Level1, ps.Level2);
                parentNode = cache.GetOrCreate(level2Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = ps.Level2, 
                    ParentId = parentNode.Id,
                    Depth = 2
                });
            }

            if (!string.IsNullOrWhiteSpace(ps.Level3))
            {
                var level3Key = BuildKey(ps.Level1, ps.Level2, ps.Level3);
                parentNode = cache.GetOrCreate(level3Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = ps.Level3, 
                    ParentId = parentNode.Id,
                    Depth = 3
                });
            }

            if (!string.IsNullOrWhiteSpace(ps.Level4))
            {
                var level4Key = BuildKey(ps.Level1, ps.Level2, ps.Level3, ps.Level4);
                parentNode = cache.GetOrCreate(level4Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = ps.Level4, 
                    ParentId = parentNode.Id,
                    Depth = 4
                });
            }

            if (!string.IsNullOrWhiteSpace(ps.Level5))
            {
                var level5Key = BuildKey(ps.Level1, ps.Level2, ps.Level3, ps.Level4, ps.Level5);
                parentNode = cache.GetOrCreate(level5Key, () => new LocationNode
                {
                    Id = ++id,
                    Name = ps.Level5, 
                    ParentId = parentNode.Id,
                    Depth = 5
                });
            }

            result.Add(new LocationNode
            {
                Id = ++id,
                Name = ps.Address,
                ParentId = parentNode!.Id,
                Number = ps.Number,
                PollingStationId = ps.Id
            });
        }

        return [.. cache.Values, .. result];
    }

        
    private static string BuildKey(params string[] keyParts)
    {
        return string.Join("-", keyParts);
    }
}
