using CsvHelper.Configuration;

namespace Vote.Monitor.Api.Feature.Observer.Services;
internal sealed class ObserverImportModelMapper : ClassMap<ObserverImportModel>
{
    public ObserverImportModelMapper()
    {
        Map(m => m.Name).Name("Name");
        Map(m => m.Email).Name("Email");
        Map(m => m.PhoneNumber).Name("PhoneNumber");
    }

}
