using System.Collections;
using Ardalis.SmartEnum;
using NJsonSchema.Generation;
using NJsonSchema;
using System.Reflection;
using Namotion.Reflection;

namespace Vote.Monitor.Api.Swagger;

public class SmartEnumSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        if (!IsTypeDerivedFromGenericType(context.Type, typeof(SmartEnum<,>)))
            return;

        var listProperty = context.Type.GetProperty("List", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        var enumValues = (IEnumerable)listProperty.GetValue(null);

        context.Schema.Enumeration.Clear();
        context.Schema.Properties.Clear();
        context.Schema.Type = JsonObjectType.String;

        foreach (var value in enumValues)
            context.Schema.Enumeration.Add(value.TryGetPropertyValue<string>("Value"));
    }

    private static bool IsTypeDerivedFromGenericType(Type typeToCheck, Type genericType)
    {
        if (typeToCheck == typeof(object))
        {
            return false;
        }

        if (typeToCheck == null)
        {
            return false;
        }

        if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
        {
            return true;
        }

        return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
    }
}
