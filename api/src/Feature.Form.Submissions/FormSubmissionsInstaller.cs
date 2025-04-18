using Microsoft.Extensions.DependencyInjection;
using Module.Answers.Models;
using Vote.Monitor.Core.Converters;
using Vote.Monitor.Core.Models;
using Module.Forms.Models;

namespace Feature.Form.Submissions;

public static class FormSubmissionsInstaller
{
    public static IServiceCollection AddFormSubmissionsFeature(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(typeof(BaseQuestionModel[]), new JsonToObjectConverter<BaseQuestionModel[]>());
        SqlMapper.AddTypeHandler(typeof(BaseAnswerModel[]), new JsonToObjectConverter<BaseAnswerModel[]>());
        SqlMapper.AddTypeHandler(typeof(NoteModel[]), new JsonToObjectConverter<NoteModel[]>());
        SqlMapper.AddTypeHandler(typeof(AttachmentModel[]), new JsonToObjectConverter<AttachmentModel[]>());
        SqlMapper.AddTypeHandler(typeof(ObservationBreakModel[]), new JsonToObjectConverter<ObservationBreakModel[]>());

        return services;
    }
}
