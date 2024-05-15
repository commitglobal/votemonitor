using Bogus;

namespace SubmissionsFaker;

public static class FakerExtensions
{
    public static List<T> GenerateUnique<T>(this Faker<T> fake, int count) where T : class
    {
        var fakes = new HashSet<T>();

        while (fakes.Count < count)
        {
            var fakeData = fake.Generate();
            fakes.Add(fakeData);
        }

        return fakes.ToList();
    }
}