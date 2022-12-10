// Date Created: 2022/12/10
// Created by: JSW

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
    /// The specified list, or null if it does not exist
    /// </returns>
    Task<ListDto?> GetListByIdAsync(int listId);
}