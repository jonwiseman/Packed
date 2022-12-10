// Date Created: 2022/12/10
// Created by: JSW

using Packed.Data.Core.DTOs;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API.Services;

/// <summary>
/// Standard implementation of the <see cref="IPackedDataService"/> interface
/// </summary>
public class PackedDataService : IPackedDataService
{
    #region FIELDS

    /// <summary>
    /// Repository for interacting with
    /// </summary>
    private readonly IListRepository _listRepository;

    #endregion FIELDS

    #region CONSTRUCTORS

    public PackedDataService(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    #endregion CONSTRUCTORS

    #region METHODS

    /// <summary>
    /// Retrieve all lists which currently exist
    /// </summary>
    /// <returns>
    /// All lists which currently exist
    /// </returns>
    public async Task<IEnumerable<ListDto>> GetAllListsAsync()
    {
        // Find all lists which exist in the database
        var foundLists = (await _listRepository.GetAllListsAsync()) ?? new List<List>();

        // Convert these list entities into DTO representations
        return foundLists
            .Select(l => new ListDto(l))
            .ToList();
    }

    /// <summary>
    /// Retrieve the list with the specified ID, if it exists
    /// </summary>
    /// <param name="listId">ID of the list to search for</param>
    /// <returns>
    /// The specified list, or null if it does not exist
    /// </returns>
    public async Task<ListDto?> GetListByIdAsync(int listId)
    {
        var foundList = await _listRepository.GetListByIdAsync(listId);

        return foundList is null
            ? null
            : new ListDto(foundList);
    }

    #endregion METHODS
}