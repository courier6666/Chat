using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatServer;

public static class JsonOptions
{
    private static JsonSerializerOptions jsonSerializerOptions;
    
    public static JsonSerializerOptions Instance => jsonSerializerOptions ??= GetJsonOptions();
    
    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }
}