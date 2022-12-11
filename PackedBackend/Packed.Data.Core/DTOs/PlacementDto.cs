// Date Created: 2022/12/10
// Created by: JSW

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packed.Data.Core.Entities;

namespace Packed.Data.Core.DTOs
{
    /// <summary>
    /// DTO representation of a <see cref="Placement"/> entity
    /// </summary>
    public class PlacementDto
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Parameterless constructor for use in deserialization
        /// </summary>
        [JsonConstructor]
        public PlacementDto()
        {
        }

        /// <summary>
        /// Create a DTO using an actual placement entity
        /// </summary>
        /// <param name="placementEntity"></param>
        public PlacementDto(Placement placementEntity)
        {
            Id = placementEntity.Id;
            ContainerId = placementEntity.ContainerId;
        }

        #endregion CONSTRUCTORS

        #region PROPERTIES

        /// <summary>
        /// Placement ID
        /// </summary>
        [JsonPropertyName("placementId")]
        public int Id { get; private set; }

        /// <summary>
        /// Container ID
        /// </summary>
        [JsonPropertyName("containerId")]
        [Required]
        [Range(1, int.MaxValue)]
        public int ContainerId { get; set; }

        #endregion PROPERTIES
    }
}