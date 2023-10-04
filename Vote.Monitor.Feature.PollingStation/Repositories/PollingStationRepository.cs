using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Feature.PollingStation.Context;
using Vote.Monitor.Feature.PollingStation.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal class PollingStationRepository : IPollingStationRepository
{
    //temp 

    PollingStationModel _model = null;
    public readonly ApplicationContext _context;

    public PollingStationRepository(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<PollingStationModel> Add(PollingStationModel entity)
    {
        _model = entity;
        _model.Id = 1;

        var pollingStation = new PollingStationModel
        {
            DisplayOrder = entity.DisplayOrder,
            Address = entity.Address,
            Tags = entity.Tags
        };

        _context.PollingStation.Add(pollingStation);

        await _context.SaveChangesAsync();

        return pollingStation;
    }

    public async Task<PollingStationModel> GetById(int id)
    {
        var pollingStation = await _context.PollingStation
            .Include(ps => ps.Tags)
            .FirstOrDefaultAsync(ps => ps.Id == id);

        if (pollingStation == null) throw new Exception("Not found");

        return pollingStation;
    }

    public async Task<IEnumerable<PollingStationModel>> GetAll()
    {
        var result = await _context.PollingStation.Include(ps => ps.Tags).ToListAsync();
        return result;
    }

    public async Task<PollingStationModel> Update(int id, PollingStationModel entity)
    {
        var pollingStation = await _context.PollingStation
           .Include(ps => ps.Tags)
           .FirstOrDefaultAsync(ps => ps.Id == id);

        if (pollingStation == null) throw new Exception("Not found");

        pollingStation.DisplayOrder = entity.DisplayOrder;
        pollingStation.Address = entity.Address;

        if (entity.Tags != null)
        {
            pollingStation.Tags.Clear();

            foreach (var tag in entity.Tags)
            {
                var tagToUpdate = await _context.Tag.FirstOrDefaultAsync(t => t.Name == tag.Key);

                if (tagToUpdate == null)
                {
                    tagToUpdate = new Tag
                    {
                        Name = tag.Key,
                        Value = tag.Value
                    };
                    _context.Tag.Add(tagToUpdate);
                };

                //pollingStation.Tags.Add(tagToUpdate.Id.ToString() ,new PollingStationTag { Tag = tagToUpdate });
                pollingStation.Tags.Add(tagToUpdate.Name, tagToUpdate.Value);
            }
        }
        await _context.SaveChangesAsync();

        return pollingStation;
    }

    public async Task Delete(int id)
    {
        var pollingStation = await _context.PollingStation.FirstOrDefaultAsync(ps => ps.Id == id);

        if (pollingStation == null) throw new Exception("Not found");

        _context.PollingStation.Remove(pollingStation);

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAll()
    {
        _context.PollingStation.RemoveRange(_context.PollingStation);

        await _context.SaveChangesAsync();
    }

    public Task<IEnumerable<PollingStationModel>> GetByTags(Dictionary<string, string> tags)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Tag>> GetTags()
    {
        return await _context.Tag
            .Select(tag => new Tag { Id = tag.Id, Name = tag.Name, Value = tag.Value })
            .Distinct()
            .ToListAsync();
    }
}
