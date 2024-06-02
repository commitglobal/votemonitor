namespace Vote.Monitor.Hangfire.Extensions;

public static class ListExtensions
{
    public static List<T> PadListToTheRight<T>(this List<T> list, int desiredLength, T padValue)
    {
        if (list.Count < desiredLength)
        {
            var numberOfMissingElements = desiredLength - list.Count;
            for (int i = 0; i < numberOfMissingElements; i++)
            {
                list.Add(padValue);
            }
        }

        return list;
    }
}
