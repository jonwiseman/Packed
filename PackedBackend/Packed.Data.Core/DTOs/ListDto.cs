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
    /// DTO representation of a <see cref="List"/> entity
    /// </summary>
    public class ListDto
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Parameterless constructor for use in deserialization
        /// </summary>
        [JsonConstructor]
        public ListDto()
        {
        }

        /// <summary>
        /// Create a DTO using an actual entity
        /// </summary>
        /// <param name="listEntity">List entity</param>
        public ListDto(List listEntity)
        {
            Id = listEntity.Id;
            Description = listEntity.Description;
            Items = listEntity.Items
                .Select(i => new ItemDto(i))
                .ToList();
            Containers = listEntity.Containers
                .Select(c => new ContainerDto(c))
                .ToList();
        }

        #endregion CONSTRUCTORS

        #region PROPERTIES

        /// <summary>
        /// Identifier for the list
        /// </summary>
        /// <remarks>
        /// Read-only
        /// </remarks>
        [JsonPropertyName("listId")]
        public int Id { get; private set; }

        /// <summary>
        /// List description
        /// </summary>
        [JsonPropertyName("description")]
        [Required]
        [MinLength(1)]
        public string Description { get; set; }

        /// <summary>
        /// All items associated with this list
        /// </summary>
        /// <remarks>
        /// Read-only
        /// </remarks>
        [JsonPropertyName("items")]
        public List<ItemDto> Items { get; private set; }

        /// <summary>
        /// All containers associated with this list
        /// </summary>
        /// <remarks>
        /// Read-only
        /// </remarks>
        [JsonPropertyName("containers")]
        public List<ContainerDto> Containers { get; private set; }

        #endregion PROPERTIES
    }
}