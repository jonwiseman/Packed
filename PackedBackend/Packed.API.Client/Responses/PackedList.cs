// Date Created: 2022/12/30
// Created by: JSW

using Newtonsoft.Json;

namespace Packed.API.Client.Responses;

/// <summary>
/// API response representing a list
/// </summary>
public class PackedList
{
    /// <summary>
    /// Identifier for the list
    /// </summary>
    [JsonProperty("listId")]
    public int Id { get; set; }

    /// <summary>
    /// List description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = null!;

    /// <summary>
    /// All items associated with this list
    /// </summary>
    [JsonProperty("items")]
    public List<PackedItem> Items { get; set; } = null!;

    /// <summary>
    /// All containers associated with this list
    /// </summary>
    [JsonProperty("containers")]
    public List<PackedContainer> Containers { get; set; } = null!;
}