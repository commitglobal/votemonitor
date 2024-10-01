using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using FormAggregate = Vote.Monitor.Domain.Entities.FormAggregate.Form;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{

    public static FormAggregate Form(string defaultLanguage, params BaseQuestion[] questions)
    {
        return FormAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Opening, "F1", new TranslatedString(), new TranslatedString(), defaultLanguage, [], questions);
    }
}
