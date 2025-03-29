using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class MatrixOptionFaker : Faker<MatrixOption>
{
    public MatrixOptionFaker(Guid? id = null, string[]? languageList = null, bool isFlagged=false)
    {
        CustomInstantiator(f => MatrixOption.Create(id ?? f.Random.Guid(), new TranslatedStringFaker(languageList),
            isFlagged));
    }
}
