using System.Text.Json;
using System.Text.Json.Serialization;
using TesterMarten.Models;

namespace TesterMarten.Converters;

public class ContainerBaseJsonConverter : JsonConverter<ContainerBase>
{
    public override ContainerBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        if (!doc.RootElement.TryGetProperty("type", out var typeProp))
            throw new JsonException("Missing 'type' property for container polymorphic deserialization");

        var type = typeProp.GetString();
        var json = doc.RootElement.GetRawText();

        return type switch
        {
            "CompilerContainer" => (ContainerBase?)JsonSerializer.Deserialize<CompilerContainer>(json, options),
            "DocumentContainer" => (ContainerBase?)JsonSerializer.Deserialize<DocumentContainer>(json, options),
            _ => throw new JsonException($"Unknown container type: {type}")
        } ?? throw new JsonException("Failed to deserialize container");
    }

    public override void Write(Utf8JsonWriter writer, ContainerBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}