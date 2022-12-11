// Date Created: 2022/12/10
// Created by: JSW

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Packed.Data.Core.Entities;

namespace Packed.Data.Core.DTOs
{
    /// <summary>
    /// DTO representation of a <see cref="Item"/> entity
    /// </summary>
    public class ItemDto
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Parameterless constructor for use in deserialization
        /// </summary>
        [JsonConstructor]
        public ItemDto()
        {
        }

        /// <summary>
        /// Create a DTO using an actual item entity
        /// </summary>
        /// <param name="itemEntity">Item entity</param>
        public ItemDto(Item itemEntity)
        {
            Id = itemEntity.Id;
            Name = itemEntity.Name;
            Quantity = itemEntity.Quantity;
            Placements = itemEntity.Placements
                ?.Select(p => new PlacementDto(p))
                .ToList() ?? new List<PlacementDto>();
        }

        #endregion CONSTRUCTORS

        #region PROPERTIES

        /// <summary>
        /// Item ID
        /// </summary>
        [JsonPropertyName("itemId")]
        public int Id { get; private set; }

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonPropertyName("name")]
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        /// <summary>
        /// Total count of this item in the list
        /// </summary>
        [JsonPropertyName("quantity")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        /// <summary>
        /// All placements for this item
        /// </summary>
        [JsonPropertyName("placements")]
        public List<PlacementDto> Placements { get; set; }

        #endregion PROPERTIES
    }
}