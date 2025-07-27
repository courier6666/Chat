using System.Text.Json;
using System.Text;

namespace Chat.Message;

public static class MessageExtensions
{
    /// <summary>
    /// Serializes <see cref="Message{T}"/> into JSON and then into sequence of bytes using UTF8 encoding.
    /// </summary>
    /// <param name="message">Message to serialize.</param>
    /// <param name="options">JSON serialization options</param>
    /// <typeparam name="T">Message data type.</typeparam>
    /// <returns></returns>
    public static byte[] ToBytes<T>(this Message<T> message, JsonSerializerOptions? options = null)
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
    /// <typeparam name="T">Message data type.</typeparam>
    /// <returns><see cref="Message{T}"/> Message with data.</returns>
    public static Message<T>? MessageFromJsonBytes<T>(this byte[] data, JsonSerializerOptions? options = null)
    {
        var json = Encoding.UTF8.GetString(data);
        Console.WriteLine(json);
        return JsonSerializer.Deserialize<Message<T>>(json, options);
    }
}