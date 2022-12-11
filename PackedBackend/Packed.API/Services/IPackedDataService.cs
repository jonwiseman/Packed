// Date Created: 2022/12/10
// Created by: JSW

using Packed.API.Exceptions;
using Packed.Data.Core.DTOs;

namespace Packed.API.Services;

/// <summary>
/// Interface defining a service that interfaces with the Packed back end to
/// support API operations
/// </summary>
public interface IPackedDataService
{
    /// <summary>
    /// Retrieve all lists which currently exist
    /// </summary>
    /// <returns>
    /// All lists which currently exist
    /// </returns>
    Task<IEnumerable<ListDto>> GetAllListsAsync();

    /// <summary>
    /// Retrieve the list with the specified ID, if it exists
    /// </summary>
    /// <param name="listId">ID of the list to search for</param>
    /// <returns>
    /// The specified list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be located</exception>
    Task<ListDto> GetListByIdAsync(int listId);

    /// <summary>
    /// Create a new list
    /// </summary>
    /// <param name="newList">New list</param>
    /// <returns>
    /// A representation of the new list
    /// </returns>
    /// <exception cref="DuplicateListException">A list with the given name already exists</exception>
    Task<ListDto> CreateNewListAsync(ListDto newList);

    /// <summary>
    /// Update an existing list
    /// </summary>
    /// <param name="listId">ID of list to update</param>
    /// <param name="updatedList">Updated list</param>
    /// <returns>
    /// A representation of the updated list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be found</exception>
    /// <exception cref="DuplicateListException">List with given description already exists</exception>
    Task<ListDto> UpdateListAsync(int listId, ListDto updatedList);

    /// <summary>
    /// Delete list with given ID
    /// </summary>
    /// <param name="listId">ID of list to delete</param>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    Task DeleteListAsync(int listId);

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
    Task<ItemDto> GetItemById(int listId, int itemId);
}