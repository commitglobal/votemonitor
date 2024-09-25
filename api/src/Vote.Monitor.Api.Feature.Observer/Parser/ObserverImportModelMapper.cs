﻿using CsvHelper.Configuration;

namespace Vote.Monitor.Api.Feature.Observer.Parser;
internal sealed class ObserverImportModelMapper : ClassMap<ObserverImportModel>
{
    public ObserverImportModelMapper()
    {
        Map(m => m.FirstName).Name("FirstName");
        Map(m => m.LastName).Name("LastName");
        Map(m => m.Email).Name("Email");
        Map(m => m.PhoneNumber).Name("PhoneNumber");
        Map(m => m.Password).Name("Password");
    }

}
