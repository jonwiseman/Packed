// Date Created: 2023/01/03
// Created by: JSW

using Newtonsoft.Json;

namespace Packed.API.Client.Responses;

/// <summary>
/// API response representing a container
/// </summary>
public class PackedContainer
{
    /// <summary>
    /// Container's ID
    /// </summary>
    [JsonProperty("containerId")]
    public int Id { get; set; }

    /// <summary>
    /// Container's name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;
}