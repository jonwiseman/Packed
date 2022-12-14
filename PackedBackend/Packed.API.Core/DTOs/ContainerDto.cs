// Date Created: 2022/12/13
// Created by: JSW

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packed.Data.Core.Entities;

namespace Packed.API.Core.DTOs
{
    /// <summary>
    /// DTO representation of a <see cref="Container"/> entity
    /// </summary>
    public class ContainerDto
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Parameterless constructor for use in deserialization
        /// </summary>
        [JsonConstructor]
        public ContainerDto()
        {
        }

        /// <summary>
        /// Create a DTO using an actual container entity
        /// </summary>
        /// <param name="containerEntity">Container entity</param>
        public ContainerDto(Container containerEntity)
        {
            Id = containerEntity.Id;
            Name = containerEntity.Name;
        }

        #endregion CONSTRUCTORS

        #region PROPERTIES

        /// <summary>
        /// Container's ID
        /// </summary>
        [JsonPropertyName("containerId")]
        public int Id { get; private set; }

        /// <summary>
        /// Container's name
        /// </summary>
        [JsonPropertyName("name")]
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        #endregion PROPERTIES
    }
}