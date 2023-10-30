using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using EFCore.BulkExtensions;
using Vote.Monitor.Core;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal class PollingStationRepository : IPollingStationRepository
{
    private readonly AppDbContext _context;

    //temp 
    public PollingStationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Models.PollingStation> AddAsync(Domain.Models.PollingStation entity)
    {
        await _context.PollingStations.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Domain.Models.PollingStation> GetByIdAsync(Guid id)
    {
        var pollingStation = await _context.PollingStations
            .FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<Domain.Models.PollingStation>($"Polling Station not found for ID: {id}");

        return pollingStation;
    }

    public async Task<IEnumerable<Domain.Models.PollingStation>> GetAllAsync(int pageSize = 0, int page = 1)
    {
        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (pageSize > 0 && page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        if (pageSize == 0) return await _context.PollingStations.OrderBy(st => st.DisplayOrder).ToListAsync();

        return await _context.PollingStations.OrderBy(st => st.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    }

    public async Task<Domain.Models.PollingStation> UpdateAsync(Guid id, Domain.Models.PollingStation entity)
    {
        var pollingStation = await _context.PollingStations
           .FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<Domain.Models.PollingStation>($"Polling Station not found for ID: {id}");

        pollingStation.DisplayOrder = entity.DisplayOrder;
        pollingStation.Address = entity.Address;
        pollingStation.Tags = entity.Tags;

        await _context.SaveChangesAsync();

        return pollingStation;
    }

    public async Task DeleteAsync(Guid id)
    {
        var pollingStation = await _context.PollingStations.FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<Domain.Models.PollingStation>($"Polling Station not found for ID: {id}");

        _context.PollingStations.Remove(pollingStation);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        await _context.PollingStations.BatchDeleteAsync();
    }

    public async Task<IEnumerable<Domain.Models.PollingStation>> GetAllAsync(Dictionary<string, string>? filter, int pageSize = 0, int page = 1)
    {
        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (pageSize > 0 && page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        filter = filter ?? new Dictionary<string, string>();
        return await _context.PollingStations
            .Where(station => filter.Count == 0 || EF.Functions.JsonContains(station.Tags, filter))
            .OrderBy(st => st.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
             .ToListAsync();

    }

    public async Task<int> CountAsync(Dictionary<string, string>? filter)
    {
        filter = filter ?? new Dictionary<string, string>();
        return await _context.PollingStations
            .Where(station => filter.Count == 0 || EF.Functions.JsonContains(station.Tags, filter))
            .CountAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Domain.Models.PollingStation> entities)
    {
        await _context.BulkInsertAsync(entities, new BulkConfig()
        {
            PropertiesToExclude = new List<string>() { nameof(Domain.Models.PollingStation.Id) }
        });
        await _context.BulkSaveChangesAsync();
    }


    public async Task<List<string>> GetTagKeys(Dictionary<string, string>? filter = null)
    {
        filter = filter ?? new Dictionary<string, string>();

        return await _context
            .PollingStations
            .Where(station => filter.Count == 0 || EF.Functions.JsonContains(station.Tags, filter))
            .Select(x => Domain.Postgres.Functions.ObjectKeys(x.Tags))
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<TagModel>> GetTagValuesAsync(string selectTag, Dictionary<string, string>? filter)
    {
        filter = filter ?? new Dictionary<string, string>();

        return await _context
            .PollingStations
            .Where(station => filter.Count == 0 || EF.Functions.JsonContains(station.Tags, filter))
            .Where(station => EF.Functions.JsonExists(station.Tags, selectTag))
            .Select(station => new TagModel
            {
                Key = selectTag,
                Value = station.Tags.RootElement.GetProperty(selectTag).GetString()
            })
            .Distinct()
            .ToListAsync();
    }
}
