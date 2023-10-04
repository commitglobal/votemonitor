using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vote.Monitor.Core.Data;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal class PollingStationRepository : IPollingStationRepository
{
    private readonly AppDbContext _context;

    //temp 
    public PollingStationRepository(AppDbContext context)
    {
        _context = context;
    }




    public async Task<PollingStationModel> Add(PollingStationModel entity)

    {
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


    public async Task<PollingStationModel> GetById(int id)
    {
        var pollingStation = await _context.PollingStations
            .FirstOrDefaultAsync(ps => ps.Id == id);

        if (pollingStation == null) throw new Exception("Not found");

        return pollingStation;
    }

    public async Task<IEnumerable<PollingStationModel>> GetAll()
    {
        var result = await _context.PollingStations.ToListAsync();
        return result;
    }

    public async Task<PollingStationModel> Update(int id, PollingStationModel entity)
    {
        var pollingStation = await _context.PollingStations
           .FirstOrDefaultAsync(ps => ps.Id == id);

        if (pollingStation == null) throw new Exception("Not found");

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
                };

                //pollingStation.Tags.Add(tagToUpdate.Id.ToString() ,new PollingStationTag { Tag = tagToUpdate });
                pollingStation.Tags.Add(tagToUpdate);
            }
        }
        await _context.SaveChangesAsync();

        return pollingStation;
    }


    public async Task Delete(int id)
    {
        var pollingStation = await _context.PollingStations.FirstOrDefaultAsync(ps => ps.Id == id);

        if (pollingStation == null) throw new Exception("Not found");

        _context.PollingStations.Remove(pollingStation);

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAll()
    {
        _context.PollingStations.RemoveRange(_context.PollingStations);

        await _context.SaveChangesAsync();
    }

    public Task<IEnumerable<PollingStationModel>> GetByTags(Dictionary<string, string> tags)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TagModel>> GetTags()
    {
        return await _context.Tags
            .Select(tag => new TagModel { Id = tag.Id, Key = tag.Key, Value = tag.Value })
            .Distinct()
            .ToListAsync();
    }


}
