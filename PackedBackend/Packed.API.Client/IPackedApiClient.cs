// Date Created: 2023/01/03
// Created by: JSW

using Newtonsoft.Json;
using Packed.API.Client.Exceptions;
using Packed.API.Client.Responses;
using Packed.API.Core.Exceptions;

namespace Packed.API.Client;

/// <summary>
/// Interface defining a client to interact with Packed API
/// </summary>
public interface IPackedApiClient
{
    #region LISTS

    /// <summary>
    /// Retrieve all lists which currently exist
    /// </summary>
    /// <returns>
    /// All lists which currently exist
    /// </returns>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    Task<IEnumerable<PackedList>> GetAllListsAsync();

    /// <summary>
    /// Retrieve the list with the specified ID, if it exists
    /// </summary>
    /// <param name="listId">ID of the list to search for</param>
    /// <returns>
    /// The specified list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be located</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    Task<PackedList> GetListByIdAsync(int listId);

    /// <summary>
    /// Create a new list
    /// </summary>
    /// <param name="description">List description</param>
    /// <returns>
    /// A representation of the new list and a link to the created list
    /// </returns>
    /// <exception cref="DuplicateListException">A list with the given name already exists</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    Task<(PackedList, string)> CreateNewListAsync(string description);

    /// <summary>
    /// Update an existing list
    /// </summary>
    /// <param name="listId">ID of list to update</param>
    /// <param name="updatedDescription">Updated list description</param>
    /// <returns>
    /// A representation of the updated list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be found</exception>
    /// <exception cref="DuplicateListException">List with given description already exists</exception>    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    Task<PackedList> UpdateListAsync(int listId, string updatedDescription);

    /// <summary>
    /// Delete list with given ID
    /// </summary>
    /// <param name="listId">ID of list to delete</param>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    Task DeleteListAsync(int listId);

    #endregion LISTS

    #region ITEMS

    /// <summary>
    /// Get all items for the given list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <returns>
    /// All items belonging to the specified list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be found</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    Task<IEnumerable<PackedItem>> GetItemsForListAsync(int listId);

    /// <summary>
    /// Create a new item in the specified list
    /// </summary>
    /// <param name="listId">ID of list to add item to</param>
    /// <param name="name">Name of new item</param>
    /// <param name="quantity">New item quantity</param>
    /// <returns>
    /// A representation of the new item and a link to the location of the new item
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="DuplicateItemException">Item with same name already in list</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    Task<(PackedItem, string)> CreateItemForListAsync(int listId, string name, int quantity);

    /// <summary>
    /// Retrieve a specific item from the specified list
    /// </summary>
    /// <param name="listId">List to retrieve item from</param>
    /// <param name="itemId">ID of item to retrieve</param>
    /// <returns>
    /// The specified item
    /// </returns>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    Task<PackedItem> GetItemFromListAsync(int listId, int itemId);

    /// <summary>
    /// Update an item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="newName">New name of item</param>
    /// <param name="newQuantity">New quantity of item</param>
    /// <returns>
    /// A representation of the updated item
    /// </returns>
    /// <exception cref="DuplicateItemException">Item with same name already exists</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    Task<PackedItem> UpdateItemAsync(int listId, int itemId, string newName, int newQuantity);

    /// <summary>
    /// Delete an item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    Task DeleteItemAsync(int listId, int itemId);

    #endregion ITEMS

    #region CONTAINERS

    /// <summary>
    /// Get all containers belonging to a specific list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <returns>
    /// All existing containers
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    Task<IEnumerable<PackedContainer>> GetContainersForListAsync(int listId);

    /// <summary>
    /// Create a new container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="name">Container name</param>
    /// <returns>
    /// A representation of the created container and a link to the container's location
    /// </returns>
    /// <exception cref="ListNotFoundException">List not found</exception>
    /// <exception cref="DuplicateContainerException">Container with same name already exists</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    Task<(PackedContainer, string)> CreateContainerAsync(int listId, string name);

    /// <summary>
    /// Get a specific container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <returns>
    /// The specified container
    /// </returns>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    Task<PackedContainer> GetContainerAsync(int listId, int containerId);

    /// <summary>
    /// Update a container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <param name="updatedName">Updated container name</param>
    /// <returns>
    /// A representation of the updated container
    /// </returns>
    /// <exception cref="DuplicateContainerException">Container with name already exists</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    Task<PackedContainer> UpdateContainerAsync(int listId, int containerId, string updatedName);

    /// <summary>
    /// Delete a container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    Task DeleteContainerAsync(int listId, int containerId);

    #endregion CONTAINERS

    #region PLACEMENTS

    /// <summary>
    /// Get all placements for the given item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>
    /// All placements
    /// </returns>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    Task<IEnumerable<PackedPlacement>> GetPlacementsAsync(int listId, int itemId);

    /// <summary>
    /// Create a new placement
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="containerId">Container ID</param>
    /// <returns></returns>
    /// <exception cref="ItemQuantityException">Too many placements</exception>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    Task<(PackedPlacement, string)> CreatePlacementAsync(int listId, int itemId, int containerId);

    /// <summary>
    /// Delete a placement
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="placementId">Placement ID</param>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    Task DeletePlacementAsync(int listId, int itemId, int placementId);

    #endregion PLACEMENTS
}