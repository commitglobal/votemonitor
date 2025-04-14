using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class MatrixRowFaker : Faker<MatrixRow>
{
    public MatrixRowFaker(Guid? id = null, string[]? languageList = null)
    {
        CustomInstantiator(f => MatrixRow.Create(id ?? f.Random.Guid(), new TranslatedStringFaker(languageList)));
    }
}
