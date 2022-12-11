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
    /// Unit of work for interacting with Packed backend
    /// </summary>
    private readonly IPackedUnitOfWork _unitOfWork;

    #endregion FIELDS

    #region CONSTRUCTORS

    /// <summary>
    /// Create a new data service
    /// </summary>
    /// <param name="unitOfWork">Packed unit of work</param>
    public PackedDataService(IPackedUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
        var foundLists = (await _unitOfWork.ListRepository.GetAllListsAsync()) ?? new List<List>();

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
        var foundList = await _unitOfWork.ListRepository.GetListByIdAsync(listId);

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
    public async Task<ListDto> CreateNewListAsync(ListDto newList)
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
            _unitOfWork.ListRepository.Create(listToCreate);

            // Save all changes to database context
            await _unitOfWork.SaveChangesAsync();
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
    public async Task<ListDto> UpdateListAsync(int listId, ListDto updatedList)
    {
        // Start by finding the list which needs to be updated
        var listToUpdate = await _unitOfWork.ListRepository.GetListByIdAsync(listId);

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
            _unitOfWork.ListRepository.Update(listToUpdate);
            await _unitOfWork.SaveChangesAsync();
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

    /// <summary>
    /// Delete list with given ID
    /// </summary>
    /// <param name="listId">ID of list to delete</param>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    public async Task DeleteListAsync(int listId)
    {
        // Attempt to find the list
        var foundList = await _unitOfWork.ListRepository.GetListByIdAsync(listId);

        // If we couldn't find the list, then throw a ListNotFoundException
        if (foundList is null)
        {
            throw new ListNotFoundException($"Could not find list with ID {listId}");
        }

        // If we found the list, then delete it
        _unitOfWork.ListRepository.Delete(foundList);
        await _unitOfWork.SaveChangesAsync();
    }

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
        // Start by trying to retrieve the specified list
        var foundList = await _unitOfWork.ListRepository.GetListByIdAsync(listId);

        // If specified list could not be found, then throw a ListNotFoundException
        if (foundList is null)
        {
            throw new ListNotFoundException($"List with ID {listId} not found");
        }

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
        // Start by trying to retrieve the specified list
        var foundList = await _unitOfWork.ListRepository.GetListByIdAsync(listId);

        // If list could not be found, throw a ListNotFoundException
        if (foundList is null)
        {
            throw new ListNotFoundException($"List with ID {listId} could not be found");
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
            _unitOfWork.ItemRepository.Create(itemToAdd);

            // Attempt to save changes to the database
            await _unitOfWork.SaveChangesAsync();
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
        // Start by trying to retrieve the specified list
        var foundList = await _unitOfWork.ListRepository.GetListByIdAsync(listId);

        // If list could not be found, throw a ListNotFoundException
        if (foundList is null)
        {
            throw new ListNotFoundException($"List with ID {listId} could not be found");
        }

        // Attempt to find the specified item
        var foundItem = foundList.Items
            .SingleOrDefault(i => i.Id == itemId);

        // If specified item was not found, then throw a ItemNotFoundException
        if (foundItem is null)
        {
            throw new ItemNotFoundException($"Item with ID {itemId} could not be found in list with ID {listId}");
        }

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
    public async Task<ItemDto> UpdateItemAsync(int listId, int itemId, ItemDto updatedItem)
    {
        // Start by trying to retrieve the specified list
        var foundList = await _unitOfWork.ListRepository.GetListByIdAsync(listId);

        // If list could not be found, throw a ListNotFoundException
        if (foundList is null)
        {
            throw new ListNotFoundException($"List with ID {listId} could not be found");
        }

        // Attempt to find the specified item
        var foundItem = foundList.Items
            .SingleOrDefault(i => i.Id == itemId);

        // If specified item was not found, then throw a ItemNotFoundException
        if (foundItem is null)
        {
            throw new ItemNotFoundException($"Item with ID {itemId} could not be found in list with ID {listId}");
        }

        // If we found the specified item, then update it
        foundItem.Name = updatedItem.Name;
        foundItem.Quantity = updatedItem.Quantity;

        try
        {
            // Update the item via context
            _unitOfWork.ItemRepository.Update(foundItem);

            // Attempt to save changes to DB
            await _unitOfWork.SaveChangesAsync();
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

    #endregion METHODS
}