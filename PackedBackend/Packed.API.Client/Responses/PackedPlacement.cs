// Date Created: 2023/01/03
// Created by: JSW

using Newtonsoft.Json;

namespace Packed.API.Client.Responses;

/// <summary>
/// API response representing a placement
/// </summary>
public class PackedPlacement
{
    /// <summary>
    /// Placement ID
    /// </summary>
    [JsonProperty("placementId")]
    public int Id { get; set; }

    /// <summary>
    /// Container ID
    /// </summary>
    [JsonProperty("containerId")]
    public int ContainerId { get; set; }
}