// Date Created: 2022/12/13
// Created by: JSW

using System.Collections.Generic;
using System.Threading.Tasks;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;

namespace Packed.API.Core.Services
{
    /// <summary>
    /// Interface defining a service that interfaces with the Packed back end to
    /// support API operations on Items
    /// </summary>
    public interface IPackedItemsDataService
    {
        /// <summary>
        /// Retrieve all items belonging to the specified list
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <returns>
        /// All items belonging to the specified list
        /// </returns>
        /// <exception cref="ListNotFoundException">List with specified ID does not exist</exception>
        Task<List<ItemDto>> GetItemsForListAsync(int listId);

        /// <summary>
        /// Add a new item to a list
        /// </summary>
        /// <param name="listId">ID of list to add item to</param>
        /// <param name="newItem">Item to add</param>
        /// <returns>
        /// A representation of the created item
        /// </returns>
        /// <exception cref="ListNotFoundException">Specified list could not be found</exception>
        /// <exception cref="DuplicateItemException">Item with same name already exists in list</exception>
        Task<ItemDto> AddItemToListAsync(int listId, ItemDto newItem);

        /// <summary>
        /// Get the specified item
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">Item I</param>
        /// <returns>
        /// The specified item
        /// </returns>
        /// <exception cref="ListNotFoundException">Specified list could not be found</exception>
        /// <exception cref="ItemNotFoundException">Specified item could not be found</exception>
        Task<ItemDto> GetItemByIdAsync(int listId, int itemId);

        /// <summary>
        /// Update an existing item
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">ID of item to update</param>
        /// <param name="updatedItem">Updated item</param>
        /// <returns>
        /// A representation of the updated item
        /// </returns>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        /// <exception cref="ItemNotFoundException">Item could not be found</exception>
        /// <exception cref="DuplicateItemException">Item with same name already exists in list</exception>
        /// <exception cref="ItemQuantityException">Reducing number of items to below number of placements</exception>
        Task<ItemDto> UpdateItemAsync(int listId, int itemId, ItemDto updatedItem);

        /// <summary>
        /// Delete an item
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">Item ID</param>
        /// <exception cref="ListNotFoundException">List not found</exception>
        /// <exception cref="ItemNotFoundException">Item not found</exception>
        Task DeleteItemAsync(int listId, int itemId);
    }
}