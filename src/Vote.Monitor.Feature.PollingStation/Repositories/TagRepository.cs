using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;

internal class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    //temp 
    public TagRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetAllTagKeysAsync()
    {
        var tagskey = await _context.Tags
            .Select(tag => tag.Key).Distinct()
            .ToListAsync();
        return tagskey;

    }


    public async Task<IEnumerable<TagModel>> GetTagsAsync(string selectKey, List<TagModel>? filterCriteria)
    {

        // var st = _context.PollingStations.ToList();
        IEnumerable<PollingStationModel> stationswithtags;
        if (filterCriteria != null && filterCriteria.Count > 0)
            stationswithtags = from station in await _context.PollingStations.Include(x => x.Tags).ToListAsync()
                               where
                                (from tag in station.Tags
                                 join filter in filterCriteria
                                on new { tag.Key, tag.Value } equals new { filter.Key, filter.Value }
                                 select tag)
                                       .Count() == filterCriteria.Count

                               select station;
        else
            stationswithtags = await _context.PollingStations.ToListAsync();

        var tags = stationswithtags.Select(t => t.Tags).SelectMany(x => x).Where(c => c.Key == selectKey).Distinct().ToList();
        return tags;

    }
}
