using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatServer;

public static class JsonOptionsFactory
{
    public static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions()
        {
            WriteIndented = false,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }
}