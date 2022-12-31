// Date Created: 2022/12/27
// Created by: JSW

using Newtonsoft.Json;
using Packed.API.Client.Exceptions;
using Packed.API.Client.Responses;
using Packed.API.Core.DTOs;
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
    /// <param name="updatedList">Updated list</param>
    /// <returns>
    /// A representation of the updated list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be found</exception>
    /// <exception cref="DuplicateListException">List with given description already exists</exception>    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    Task<PackedList> UpdateListAsync(int listId, ListDto updatedList);

    /// <summary>
    /// Delete list with given ID
    /// </summary>
    /// <param name="listId">ID of list to delete</param>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    Task DeleteListAsync(int listId);

    #endregion LISTS
}