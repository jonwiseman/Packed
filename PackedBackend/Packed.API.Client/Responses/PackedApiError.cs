// Date Created: 2023/01/03
// Created by: JSW

using Newtonsoft.Json;

namespace Packed.API.Client.Responses;

/// <summary>
/// Represents an error response from the API
/// </summary>
public class PackedApiError
{
    /// <summary>
    /// URI to resource describing the error
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// Short title of error
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// HTTP status code of error
    /// </summary>
    [JsonProperty("status")]
    public int StatusCode { get; set; }

    /// <summary>
    /// Detailed description of the error
    /// </summary>
    [JsonProperty("detail")]
    public string Detail { get; set; } = null!;

    /// <summary>
    /// Request URI which caused the error
    /// </summary>
    [JsonProperty("instance")]
    public string Instance { get; set; } = null!;

    /// <summary>
    /// Unique tracking number for the error
    /// </summary>
    [JsonProperty("errorId")]
    public Guid ErrorId { get; set; }

    /// <summary>
    /// Time at which the error occurred
    /// </summary>
    [JsonProperty("timestamp")]
    public DateTime TimeStamp { get; set; }
}