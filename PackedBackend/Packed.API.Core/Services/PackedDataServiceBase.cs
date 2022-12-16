// Date Created: 2022/12/15
// Created by: JSW

using System;
using System.Linq;
using System.Threading.Tasks;
using Packed.API.Core.Exceptions;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API.Core.Services
{
    /// <summary>
    /// Base class for all data service implementations
    /// </summary>
    public abstract class PackedDataServiceBase
    {
        #region FIELDS

        /// <summary>
        /// Unit of work for interacting with Packed data store
        /// </summary>
        protected readonly IPackedUnitOfWork PackedUnitOfWork;

        #endregion FIELDS

        #region CONSTRUCTOR

        /// <summary>
        /// Create a new data service
        /// </summary>
        /// <param name="packedUnitOfWork">Unit of work for interacting with Packed data store</param>
        protected PackedDataServiceBase(IPackedUnitOfWork packedUnitOfWork)
        {
            PackedUnitOfWork = packedUnitOfWork ?? throw new ArgumentNullException(nameof(packedUnitOfWork));
        }

        #endregion CONSTRUCTOR

        #region HELPER METHODS

        /// <summary>
        /// Retrieve the specified list and throw an exception if it does not exist
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <returns>
        /// The specified list
        /// </returns>
        /// <exception cref="ListNotFoundException">The list could not be found</exception>
        protected async Task<List> GetList(int listId)
        {
            // Attempt to find the list
            var foundList = await PackedUnitOfWork.ListRepository.GetListByIdAsync(listId);

            // If we couldn't find the list, then throw a ListNotFoundException
            if (foundList is null)
            {
                throw new ListNotFoundException($"Could not find list with ID {listId}");
            }

            return foundList;
        }

        /// <summary>
        /// Given a list, retrieve a specific item and throw an exception if it is not found
        /// </summary>
        /// <param name="foundList">List</param>
        /// <param name="itemId">Item ID</param>
        /// <returns>
        /// The specified item
        /// </returns>
        /// <exception cref="ItemNotFoundException">The item could not be found in the list</exception>
        protected Item GetItem(List foundList, int itemId)
        {
            // Attempt to find the specified item in the given list
            var foundItem = foundList.Items
                .SingleOrDefault(i => i.Id == itemId);

            // If specified item was not found, then throw a ItemNotFoundException
            if (foundItem is null)
            {
                throw new ItemNotFoundException(
                    $"Item with ID {itemId} could not be found in list with ID {foundList.Id}");
            }

            return foundItem;
        }

        /// <summary>
        /// Retrieve a specific container from a list and ensure it exists
        /// </summary>
        /// <param name="foundList">The loaded list</param>
        /// <param name="containerId">The specific container ID</param>
        /// <returns>
        /// The specified container
        /// </returns>
        /// <exception cref="ContainerNotFoundException">The container could not be found</exception>
        protected Container GetContainer(List foundList, int containerId)
        {
            // Try to find the container in the list's collection of containers
            var foundContainer = foundList.Containers
                .SingleOrDefault(c => c.Id == containerId);

            // If the container was not found, then throw an exception
            if (foundContainer is null)
            {
                throw new ContainerNotFoundException(
                    $"Container with ID {containerId} could not be found in list with ID {foundList.Id}");
            }

            return foundContainer;
        }

        /// <summary>
        /// Given an item, find the specified placement
        /// </summary>
        /// <param name="foundItem">Item</param>
        /// <param name="placementId">Placement ID</param>
        /// <returns>
        /// The specified placement
        /// </returns>
        protected Placement GetPlacement(Item foundItem, int placementId)
        {
            // Try to find the specific placement
            var foundPlacement = foundItem.Placements
                .SingleOrDefault(p => p.Id == placementId);

            // If placement not found, throw an exception
            if (foundPlacement is null)
            {
                throw new PlacementNotFoundException(
                    $"Placement with ID {placementId} not found");
            }

            // Otherwise return the placement
            return foundPlacement;
        }

        #endregion HELPER METHODS
    }
}