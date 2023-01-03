// Date Created: 2023/01/03
// Created by: JSW

using Newtonsoft.Json;

namespace Packed.API.Client.Responses;

/// <summary>
/// API response representing an item
/// </summary>
public class PackedItem
{
    /// <summary>
    /// Item ID
    /// </summary>
    [JsonProperty("itemId")]
    public int Id { get; set; }

    /// <summary>
    /// Name of the item
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Total count of this item in the list
    /// </summary>
    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// All placements for this item
    /// </summary>
    [JsonProperty("placements")]
    public List<PackedPlacement> Placements { get; set; } = null!;
}