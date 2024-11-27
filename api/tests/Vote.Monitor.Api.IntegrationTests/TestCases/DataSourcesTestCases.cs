using System.Collections;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.TestCases;

public class DataSourcesTestCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return new object[] { DataSource.Ngo };
        yield return new object[] { DataSource.Coalition };
    }
}
