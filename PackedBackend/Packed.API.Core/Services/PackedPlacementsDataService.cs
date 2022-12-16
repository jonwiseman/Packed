// Date Created: 2022/12/15
// Created by: JSW

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API.Core.Services
{
    /// <summary>
    /// Data service for manipulating placements
    /// </summary>
    public class PackedPlacementsDataService : PackedDataServiceBase, IPackedPlacementsDataService
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Create a new data service
        /// </summary>
        /// <param name="packedUnitOfWork">Unit of work for interacting with Packed data store</param>
        public PackedPlacementsDataService(IPackedUnitOfWork packedUnitOfWork)
            : base(packedUnitOfWork)
        {
        }

        #endregion CONSTRUCTOR

        #region METHODS

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
        public async Task<List<PlacementDto>> GetPlacementsForItemAsync(int listId, int itemId)
        {
            // Get list
            var foundList = await GetList(listId);

            // Get item
            var foundItem = GetItem(foundList, itemId);

            // Return all placements for item
            return foundItem.Placements
                ?.Select(p => new PlacementDto(p))
                .ToList() ?? new List<PlacementDto>();
        }

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
        public async Task<PlacementDto> GetPlacementByIdAsync(int listId, int itemId, int placementId)
        {
            // Get list
            var foundList = await GetList(listId);

            // Get item
            var foundItem = GetItem(foundList, itemId);

            // Get placement
            var foundPlacement = GetPlacement(foundItem, placementId);

            return new PlacementDto(foundPlacement);
        }

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
        public async Task<PlacementDto> PlaceItemAsync(int listId, int itemId, PlacementDto newPlacement)
        {
            // Get list
            var foundList = await GetList(listId);

            // Get item
            var foundItem = GetItem(foundList, itemId);

            // Find container
            var foundContainer = GetContainer(foundList, newPlacement.ContainerId);

            // If adding a new placement would make us exceed the total quantity of the item, then throw an exception
            if (foundItem.Placements.Count + 1 > foundItem.Quantity)
            {
                throw new ItemQuantityException($"Cannot add an additional placement to item with ID {itemId}");
            }

            // Otherwise, we create a placement object
            var createdPlacement = new Placement
            {
                ItemId = foundItem.Id,
                ContainerId = foundContainer.Id
            };

            // Use repository to add the placement
            PackedUnitOfWork.PlacementRepository.Create(createdPlacement);

            // And now perform the actual update
            await PackedUnitOfWork.SaveChangesAsync();

            // Finally, return a representation of the new placement
            return new PlacementDto(createdPlacement);
        }

        /// <summary>
        /// Delete the specified placement
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">Item ID</param>
        /// <param name="placementId">Placement ID</param>
        /// <exception cref="ListNotFoundException">List not found</exception>
        /// <exception cref="ItemNotFoundException">Item not found</exception>
        /// <exception cref="PlacementNotFoundException">Placement not found</exception>
        public async Task DeletePlacementAsync(int listId, int itemId, int placementId)
        {
            // Get list
            var foundList = await GetList(listId);

            // Get item
            var foundItem = GetItem(foundList, itemId);

            // Get placement
            var foundPlacement = GetPlacement(foundItem, placementId);

            // Use repository to delete placement
            PackedUnitOfWork.PlacementRepository.Delete(foundPlacement);

            // Save changes to data store
            await PackedUnitOfWork.SaveChangesAsync();
        }

        #endregion METHODS
    }
}