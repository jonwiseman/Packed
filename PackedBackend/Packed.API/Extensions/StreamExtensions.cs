// Date Created: 2022/12/27
// Created by: JSW

using Newtonsoft.Json;

namespace Packed.API.Extensions;

public static class StreamExtensions
{
    /// <summary>
    /// Read and deserialize a stream into the given object type
    /// </summary>
    /// <typeparam name="T">Type to deserialize into</typeparam>
    /// <param name="stream">The stream</param>
    /// <returns>
    /// An instance of type T
    /// </returns>
    /// <exception cref="ArgumentNullException">If stream is null</exception>
    /// <exception cref="InvalidOperationException">If can't read stream</exception>
    /// <exception cref="JsonSerializationException">If stream can't be deserialized into given type</exception>
    public static async Task<T> ReadAndDeserializeFromJson<T>(this Stream stream)
        where T : class
    {
        // If stream is null, throw exception
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        // If we can't read the stream, throw an exception
        if (!stream.CanRead)
        {
            throw new InvalidOperationException("Cannot read stream");
        }

        // Initialize stream reader
        using var streamReader = new StreamReader(stream);

        // Get content
        var content = await streamReader.ReadToEndAsync();

        // Attempt to deserialize the JSON content into the appropriate type
        return JsonConvert.DeserializeObject<T>(content) ??
               throw new JsonSerializationException("Could not read and parse stream into appropriate type");
    }
}