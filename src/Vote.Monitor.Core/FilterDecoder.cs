namespace Vote.Monitor.Core;

public static class FilterDecoder
{
    public static Dictionary<string, string> DecodeFilter(string filterString)
    {
        var filterDict = new Dictionary<string, string>();

        foreach (var filterPair in filterString.Split(','))
        {
            var keyValue = filterPair.Split(':');
            var key = keyValue[0];
            var value = keyValue[1];

            filterDict.Add(key, value);
        }

        return filterDict;
    }

}
