// Date Created: 2022/12/15
// Created by: JSW

using System.Collections.Generic;
using System.Threading.Tasks;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;

namespace Packed.API.Core.Services
{
    /// <summary>
    /// Interface defining a service which interacts with placements
    /// </summary>
    public interface IPackedPlacementsDataService
    {
        /// <summary>
        /// Get all placements for the specified item
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">Item ID</param>
        /// <returns>
        /// All placements for the item
        /// </returns>
        /// <exception cref="ListNotFoundException">List not found</exception>
        /// <exception cref="ItemNotFoundException">Item not found</exception>
        Task<List<PlacementDto>> GetPlacementsForItemAsync(int listId, int itemId);

        /// <summary>
        /// Retrieve a specific placement by ID
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">Item ID</param>
        /// <param name="placementId">Placement ID</param>
        /// <returns>
        /// The specified placement
        /// </returns>
        /// <exception cref="ListNotFoundException">List not found</exception>
        /// <exception cref="ItemNotFoundException">Item not found</exception>
        /// <exception cref="PlacementNotFoundException">Placement not found</exception>
        Task<PlacementDto> GetPlacementByIdAsync(int listId, int itemId, int placementId);

        /// <summary>
        /// Place an item into a container
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">Item ID</param>
        /// <param name="newPlacement">New placement</param>
        /// <returns>
        /// A representation of the new placement
        /// </returns>
        /// <exception cref="ListNotFoundException">List not found</exception>
        /// <exception cref="ItemNotFoundException">Item not found</exception>
        /// <exception cref="ContainerNotFoundException">Container not found</exception>
        /// <exception cref="ItemQuantityException">Item has too many placements</exception>
        Task<PlacementDto> PlaceItemAsync(int listId, int itemId, PlacementDto newPlacement);

        /// <summary>
        /// Delete the specified placement
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">Item ID</param>
        /// <param name="placementId">Placement ID</param>
        /// <exception cref="ListNotFoundException">List not found</exception>
        /// <exception cref="ItemNotFoundException">Item not found</exception>
        /// <exception cref="ContainerNotFoundException">Container not found</exception>
        /// <exception cref="PlacementNotFoundException">Placement not found</exception>
        Task DeletePlacementAsync(int listId, int itemId, int placementId);
    }
}