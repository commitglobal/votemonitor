using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vote.Monitor.Feature.PollingStation.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal class PollingStationRepository : IPollingStationRepository
{
    //temp 

    PollingStationModel _model=null; 

    public void   Add(PollingStationModel entity)
    {
        _model= entity;
        _model.Id = 1;
    }

    public void Delete(PollingStationModel entity)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PollingStationModel> GetAll()
    {
        throw new NotImplementedException();
    }

    public PollingStationModel GetById(int id)
    {
        if (_model == null ) throw new Exception("Not found");
        if (_model.Id != id) throw new Exception("Not found");  
        return _model;
    }

    public IEnumerable<PollingStationModel> GetByTags(Dictionary<string, string> tags)
    {
        throw new NotImplementedException();
    }

    public void Update(PollingStationModel entity)
    {
        throw new NotImplementedException();
    }
}
