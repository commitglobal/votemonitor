using System.Text.Json;
using Dapper;
using Feature.Form.Submissions.Models;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Form.Module.Models;

namespace Feature.Form.Submissions;

public class JsonToObjectConverter<T> : SqlMapper.TypeHandler<T> where T : class
{
    public override T Parse(object value)
    {
        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}

public static class FormSubmissionsInstaller
{
    public static IServiceCollection AddFormSubmissionsFeature(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(typeof(BaseQuestionModel[]), new JsonToObjectConverter<BaseQuestionModel[]>());
        SqlMapper.AddTypeHandler(typeof(BaseAnswerModel[]), new JsonToObjectConverter<BaseAnswerModel[]>());
        SqlMapper.AddTypeHandler(typeof(NoteModel[]), new JsonToObjectConverter<NoteModel[]>());
        SqlMapper.AddTypeHandler(typeof(AttachmentModel[]), new JsonToObjectConverter<AttachmentModel[]>());

        return services;
    }
}
