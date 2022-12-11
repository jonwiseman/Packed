// Date Created: 2022/12/10
// Created by: JSW

using Npgsql;
using Packed.API.Exceptions;
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
    public async Task<ListDto> GetListByIdAsync(int listId)
    {
        var foundList = await _listRepository.GetListByIdAsync(listId);

        if (foundList is null)
        {
            throw new ListNotFoundException();
        }

        return new ListDto(foundList);
    }

    /// <summary>
    /// Create a new list
    /// </summary>
    /// <param name="newList">New list</param>
    /// <returns>
    /// A representation of the new list
    /// </returns>
    /// <exception cref="DuplicateListException">A list with the given name already exists</exception>
    public async Task<ListDto> CreateNewList(ListDto newList)
    {
        // Initialize entity to create
        var listToCreate = new List
        {
            Description = newList.Description,
            Items = new List<Item>(),
            Containers = new List<Container>()
        };

        try
        {
            // Create the entity in our context
            _listRepository.Create(listToCreate);

            // Save all changes to database context
            await _listRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            // If the exception is that we have a unique violation, then we throw a DuplicateListException
            if (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new DuplicateListException("A list with the same name already exists", e);
            }

            // Otherwise something else happen and we rethrow the exception
            throw;
        }

        // Return the new representation of the created list
        return new ListDto(listToCreate);
    }

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
    public async Task<ListDto> UpdateList(int listId, ListDto updatedList)
    {
        // Start by finding the list which needs to be updated
        var listToUpdate = await _listRepository.GetListByIdAsync(listId);

        // If the list we are trying to update could not be found, throw an exception and let caller deal with it
        if (listToUpdate is null)
        {
            throw new ListNotFoundException($"List with ID {listId} could not be found");
        }

        // If there is no actual update to perform, then do nothing
        if (string.Equals(updatedList.Description, listToUpdate.Description))
        {
            return new ListDto(listToUpdate);
        }

        // If we have found the list, then go ahead and set the description...
        listToUpdate.Description = updatedList.Description;

        try
        {
            // ...and attempt to update the list
            _listRepository.Update(listToUpdate);
            await _listRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            // If the exception is that we have a unique violation, then we throw a DuplicateListException
            if (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new DuplicateListException("A list with the same name already exists", e);
            }

            // Otherwise something else happen and we rethrow the exception
            throw;
        }

        return new ListDto(listToUpdate);
    }

    #endregion METHODS
}