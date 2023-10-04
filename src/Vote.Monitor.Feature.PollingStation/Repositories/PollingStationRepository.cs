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
    public  PollingStationRepository(AppDbContext context)
    {
        _context=  context;
    }




    public void   Add(PollingStationEf entity)
    {
        List<TagEf> tags = new List<TagEf>();
        foreach (var tag in entity.Tags)
        {
           var efTag=_context.Tags.FirstOrDefault(x => x.Key == tag.Key && x.Value == tag.Value);
           if (efTag != null)
            {
                tags.Add(efTag);
            }
           else tags.Add(tag);
        }
        entity.Tags = tags;
        _context.PollingStations.Add(entity);
        _context.SaveChanges();
    }

    public void Delete(PollingStationEf entity)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PollingStationEf> GetAll()
    {
        throw new NotImplementedException();
    }

    public PollingStationEf GetById(int id)
    {
        return _context?.PollingStations?.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<Core.Models.PollingStationEf> GetByTags(Dictionary<string, string> tags)
    {
        throw new NotImplementedException();
    }

    public void Update(Core.Models.PollingStationEf entity)
    {
        throw new NotImplementedException();
    }
}
