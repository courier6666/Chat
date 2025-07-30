using System.Text.Json;
using System.Text;

namespace Chat.Message;

public static class MessageExtensions
{
    /// <summary>
    /// Serializes into JSON and then into sequence of bytes using UTF8 encoding.
    /// </summary>
    /// <param name="message">Message to serialize.</param>
    /// <param name="options">JSON serialization options</param>
    /// <typeparam name="TMessage">Type message.</typeparam>
    /// <returns>An array of bytes representing a serialized message.</returns>
    public static byte[] ToBytes<TMessage>(this TMessage message, JsonSerializerOptions? options = null)
        where TMessage : Message, new()
    {
        var json = JsonSerializer.Serialize(message, options);
        Console.WriteLine(json);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Retrieves a message from a sequence of bytes using UTF8 encoding and JSON deserialization.
    /// </summary>
    /// <param name="data">Data to deserialize from.</param>
    /// <param name="options">JSON serialization options.</param>
    /// <typeparam name="TMessage">Message type.</typeparam>
    /// <returns>Message of type <see cref="Message"/>. </returns>
    public static TMessage? MessageFromJsonBytes<TMessage>(this byte[] data, JsonSerializerOptions? options = null)
        where TMessage : Message, new()
    {
        var json = Encoding.UTF8.GetString(data);
        Console.WriteLine(json);
        return JsonSerializer.Deserialize<TMessage>(json, options);
    }
}