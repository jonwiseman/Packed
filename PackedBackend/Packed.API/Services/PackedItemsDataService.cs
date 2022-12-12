// Date Created: 2022/12/11
// Created by: JSW

using Npgsql;
using Packed.API.Exceptions;
using Packed.Data.Core.DTOs;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API.Services;

/// <summary>
/// Data service for operating on items
/// </summary>
public class PackedItemsDataService : IPackedItemsDataService
{
    #region FIELDS

    /// <summary>
    /// Unit of work for accessing Packed data repositories
    /// </summary>
    private readonly IPackedUnitOfWork _packedUnitOfWork;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new data service
    /// </summary>
    /// <param name="packedUnitOfWork">Unit of work</param>
    public PackedItemsDataService(IPackedUnitOfWork packedUnitOfWork)
    {
        _packedUnitOfWork = packedUnitOfWork ?? throw new ArgumentNullException(nameof(packedUnitOfWork));
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
        await GetList(listId);

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
            _packedUnitOfWork.ItemRepository.Create(itemToAdd);

            // Attempt to save changes to the database
            await _packedUnitOfWork.SaveChangesAsync();
        }
        // Catch any exceptions...
        catch (Exception e)
        {
            // If the exception is that we have a unique violation, then we throw a DuplicateListException
            if (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
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

        // If we found the specified item, then update it
        foundItem.Name = updatedItem.Name;
        foundItem.Quantity = updatedItem.Quantity;

        try
        {
            // Update the item via context
            _packedUnitOfWork.ItemRepository.Update(foundItem);

            // Attempt to save changes to DB
            await _packedUnitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            // If the exception is that we have a unique violation, then we throw a DuplicateListException
            if (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
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
        _packedUnitOfWork.ItemRepository.Delete(foundItem);
        await _packedUnitOfWork.SaveChangesAsync();
    }

    #endregion METHODS

    #region HELPER METHODS

    /// <summary>
    /// Verify that a specified list exists and return it
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <returns>
    /// The specified list
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    private async Task<List> GetList(int listId)
    {
        // Start by trying to retrieve the specified list
        var foundList = await _packedUnitOfWork.ListRepository.GetListByIdAsync(listId);

        // If list could not be found, throw a ListNotFoundException
        if (foundList is null)
        {
            throw new ListNotFoundException($"List with ID {listId} could not be found");
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
    private Item GetItem(List foundList, int itemId)
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

    #endregion HELPER METHODS
}