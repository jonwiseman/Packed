// Date Created: 2022/12/13
// Created by: JSW

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API.Core.Services
{
    /// <summary>
    /// Data service for operating on items
    /// </summary>
    public class PackedItemsDataService : PackedDataServiceBase, IPackedItemsDataService
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Create a new data service
        /// </summary>
        /// <param name="packedUnitOfWork">Unit of work</param>
        public PackedItemsDataService(IPackedUnitOfWork packedUnitOfWork)
            : base(packedUnitOfWork)
        {
        }

        #endregion CONSTRUCTOR

        #region METHODS

        /// <summary>
        /// Retrieve all items belonging to the specified list
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <returns>
        /// All items belonging to the specified list
        /// </returns>
        /// <exception cref="ListNotFoundException">List with specified ID does not exist</exception>
        public async Task<List<ItemDto>> GetItemsForListAsync(int listId)
        {
            // Get the list
            var foundList = await GetList(listId);

            // If we did find the list, then return all items formatted as DTOs
            return foundList.Items
                .Select(i => new ItemDto(i))
                .ToList();
        }

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
        public async Task<ItemDto> AddItemToListAsync(int listId, ItemDto newItem)
        {
            // Make sure the list actually exists. This method will throw an exception if it does not
            var foundList = await GetList(listId);

            // If an item with this name already exists in this list, then we won't even try to
            // add it via our data store. Instead, throw an exception right away
            if (foundList.Items.Any(i => string.Equals(i.Name, newItem.Name)))
            {
                throw new DuplicateItemException("An item with the same name already exists");
            }

            // If list was found, then attempt to add the item
            // Start by creating a representation of the item
            var itemToAdd = new Item
            {
                ListId = listId,
                Name = newItem.Name,
                Quantity = newItem.Quantity,
                Placements = new List<Placement>()
            };

            // Try to perform an update using the DB context
            try
            {
                // Add the new item
                PackedUnitOfWork.ItemRepository.Create(itemToAdd);

                // Attempt to save changes to the database
                await PackedUnitOfWork.SaveChangesAsync();
            }
            // Catch any exceptions...
            catch (Exception e)
            {
                // If the exception is that we have a unique violation, then we throw a DuplicateListException
                if (e.InnerException is PostgresException p && p.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new DuplicateItemException("An item with the same name already exists", e);
                }

                // Otherwise something else happen and we rethrow the exception
                throw;
            }

            return new ItemDto(itemToAdd);
        }

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
        public async Task<ItemDto> GetItemByIdAsync(int listId, int itemId)
        {
            // Get the list
            var foundList = await GetList(listId);

            // Get the item
            var foundItem = GetItem(foundList, itemId);

            // If we found the item, then return a DTO representation of the item
            return new ItemDto(foundItem);
        }

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
        public async Task<ItemDto> UpdateItemAsync(int listId, int itemId, ItemDto updatedItem)
        {
            // Get the list
            var foundList = await GetList(listId);

            // Get the item
            var foundItem = GetItem(foundList, itemId);

            // If we're reducing quantity such that we have more placements than items, throw an exception
            if (updatedItem.Quantity < foundItem.Placements.Count)
            {
                throw new ItemQuantityException(
                    $"Currently have {foundItem.Placements.Count} placements, cannot reduce to {updatedItem.Quantity}");
            }

            // If an item with this name already exists in this list, then we won't even try to
            // add it via our data store. Instead, throw an exception right away
            if (foundList.Items
                .Where(i => i.Id != itemId)
                .Any(i => string.Equals(i.Name, updatedItem.Name)))
            {
                throw new DuplicateItemException("An item with the same name already exists");
            }

            // If we found the specified item, then update it
            foundItem.Name = updatedItem.Name;
            foundItem.Quantity = updatedItem.Quantity;

            try
            {
                // Update the item via context
                PackedUnitOfWork.ItemRepository.Update(foundItem);

                // Attempt to save changes to DB
                await PackedUnitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // If the exception is that we have a unique violation, then we throw a DuplicateListException
                if (e.InnerException is PostgresException p && p.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new DuplicateItemException("An item with the same name already exists", e);
                }

                // Otherwise something else happen and we rethrow the exception
                throw;
            }

            return new ItemDto(foundItem);
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="itemId">Item ID</param>
        /// <exception cref="ListNotFoundException">List not found</exception>
        /// <exception cref="ItemNotFoundException">Item not found</exception>
        public async Task DeleteItemAsync(int listId, int itemId)
        {
            // Get the list
            var foundList = await GetList(listId);

            // Get the item
            var foundItem = GetItem(foundList, itemId);

            // Delete the item
            PackedUnitOfWork.ItemRepository.Delete(foundItem);
            await PackedUnitOfWork.SaveChangesAsync();
        }

        #endregion METHODS
    }
}