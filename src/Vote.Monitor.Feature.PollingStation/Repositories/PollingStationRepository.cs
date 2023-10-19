using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal class PollingStationRepository : IPollingStationRepository
{
    private readonly AppDbContext _context;

    //temp 
    public PollingStationRepository(AppDbContext context)
    {
        _context = context;
    }




    public async Task<PollingStationModel> AddAsync(PollingStationModel entity)

    {
        if (entity.Tags == null || entity.Tags.Count == 0) throw new ArgumentException("At least 1 tag is required!");
        List<TagModel> tags = new List<TagModel>();
        foreach (var tag in entity.Tags)
        {
            var efTag = _context.Tags.FirstOrDefault(x => x.Key == tag.Key && x.Value == tag.Value);
            if (efTag != null)
            {
                tags.Add(efTag);
            }
            else tags.Add(tag);
        }
        entity.Tags = tags;
        await _context.PollingStations.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }


    public async Task<PollingStationModel> GetByIdAsync(int id)
    {
        var pollingStation = await _context.PollingStations
            .FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<PollingStationModel>($"Polling Station not found for ID: {id}");

        return pollingStation;
    }

    public async Task<IEnumerable<PollingStationModel>> GetAllAsync(int pageSize = 0, int page = 1)
    {
        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (pageSize > 0 && page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        if (pageSize == 0) return await _context.PollingStations.OrderBy(st => st.DisplayOrder).ToListAsync();

        return await _context.PollingStations.OrderBy(st => st.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    }

    public async Task<PollingStationModel> UpdateAsync(int id, PollingStationModel entity)
    {
        var pollingStation = await _context.PollingStations
           .FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<PollingStationModel>($"Polling Station not found for ID: {id}");

        pollingStation.DisplayOrder = entity.DisplayOrder;
        pollingStation.Address = entity.Address;

        if (entity.Tags != null)
        {
            pollingStation.Tags.Clear();

            foreach (var tag in entity.Tags)
            {
                var tagToUpdate = await _context.Tags.FirstOrDefaultAsync(t => t.Key == tag.Key);


                if (tagToUpdate == null)
                {
                    tagToUpdate = new TagModel
                    {
                        Key = tag.Key,
                        Value = tag.Value
                    };

                    _context.Tags.Add(tagToUpdate);
                }
                else
                {
                    tagToUpdate.Value = tag.Value;
                }

                pollingStation.Tags.Add(tagToUpdate);
            }
        }

        await _context.SaveChangesAsync();

        await DeleteOrphanedTags();

        return pollingStation;
    }


    public async Task DeleteAsync(int id)
    {
        var pollingStation = await _context.PollingStations.FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<PollingStationModel>($"Polling Station not found for ID: {id}");

        _context.PollingStations.Remove(pollingStation);

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAllAsync()
    {
        _context.PollingStations.RemoveRange(_context.PollingStations);

        await _context.SaveChangesAsync();
    }




    public async Task<IEnumerable<PollingStationModel>> GetAllAsync(Dictionary<string, string>? filterCriteria, int pageSize = 0, int page = 1)
    {
        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (pageSize > 0 && page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        if (filterCriteria == null || filterCriteria.Count == 0) return await GetAllAsync(pageSize, page);

        if (pageSize == 0) return _context.PollingStations.AsEnumerable().Where(
            station => filterCriteria.Count(filter => filterCriteria.All(tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
              ).OrderBy(st => st.DisplayOrder);

        return _context.PollingStations.AsEnumerable().Where(
            station => filterCriteria.Count(filter => filterCriteria.All(tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
              ).OrderBy(st => st.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

    }

    public async Task<int> CountAsync(Dictionary<string, string>? filterCriteria)
    {
        if (filterCriteria == null || filterCriteria.Count == 0) return await _context.PollingStations.CountAsync();

        return _context.PollingStations.AsEnumerable().Where(
            station => filterCriteria.Count(filter => filterCriteria.All(tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
              ).Count();
    }

    private async Task DeleteOrphanedTags()
    {
        var orphanedTags = _context.Tags
            .Where(tag => !tag.PollingStations.Any())
            .ToList();

        foreach (var tag in orphanedTags)
            _context.Tags.Remove(tag);

        await _context.SaveChangesAsync();
    }
}
