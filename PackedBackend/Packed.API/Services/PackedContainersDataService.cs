// Date Created: 2022/12/12
// Created by: JSW

using Npgsql;
using Packed.API.Exceptions;
using Packed.Data.Core.DTOs;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API.Services;

/// <summary>
/// Data service for manipulating containers
/// </summary>
public class PackedContainersDataService : IPackedContainersDataService
{
    #region FIELDS

    /// <summary>
    /// Unit of work for interacting with Packed data store
    /// </summary>
    private readonly IPackedUnitOfWork _packedUnitOfWork;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new data service
    /// </summary>
    /// <param name="packedUnitOfWork">Unit of work for interacting with Packed data store</param>
    public PackedContainersDataService(IPackedUnitOfWork packedUnitOfWork)
    {
        _packedUnitOfWork = packedUnitOfWork;
    }

    #endregion CONSTRUCTOR

    #region METHODS

    /// <summary>
    /// Retrieve all containers in the specified list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <returns>
    /// All containers
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    public async Task<List<ContainerDto>> GetContainersAsync(int listId)
    {
        // Get list
        var foundList = await GetList(listId);

        return foundList.Containers
                   ?.Select(c => new ContainerDto(c))
                   .ToList()
               ?? new List<ContainerDto>();
    }

    /// <summary>
    /// Add a new container to the specified list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="newContainer">New container</param>
    /// <returns>
    /// A representation of the new container
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="DuplicateContainerException">Container with same name already exists in the list</exception>
    public async Task<ContainerDto> AddContainerAsync(int listId, ContainerDto newContainer)
    {
        // Get list. Although we won't use the return value, it ensures that the list actually exists
        await GetList(listId);

        // If the list was found, then we will attempt to add the new container
        // Start by creating the entity we are going to add
        var containerToAdd = new Container
        {
            ListId = listId,
            Name = newContainer.Name,
            Placements = new List<Placement>()
        };

        try
        {
            // Then we'll add it to the list of items tracked by the context
            _packedUnitOfWork.ContainerRepository.Create(containerToAdd);

            // Finally, we'll attempt to persist the container
            await _packedUnitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            // If the exception is that we have a unique violation, then we throw a DuplicateListException
            if (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new DuplicateContainerException("A container with the same name already exists", e);
            }

            // Otherwise something else happen and we rethrow the exception
            throw;
        }

        // On success, we'll return a DTO representation of the container
        return new ContainerDto(containerToAdd);
    }

    /// <summary>
    /// Retrieve a specific container by ID
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <returns>
    /// The specified container
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="ContainerNotFoundException">Container could not be found</exception>
    public async Task<ContainerDto> GetContainerByIdAsync(int listId, int containerId)
    {
        // Get list
        var foundList = await GetList(listId);

        // If we found the list, then we'll attempt to locate the exact container
        var foundContainer = GetContainer(foundList, containerId);

        // Finally, return a DTO representation of the container
        return new ContainerDto(foundContainer);
    }

    /// <summary>
    /// Update an existing container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <param name="updatedContainer">The updated container</param>
    /// <returns>
    /// A representation of the updated container
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="ContainerNotFoundException">Container could not be found</exception>
    /// <exception cref="DuplicateContainerException">Container with same name already exists in the list</exception>
    public async Task<ContainerDto> UpdateContainerAsync(int listId, int containerId, ContainerDto updatedContainer)
    {
        // Get list
        var foundList = await GetList(listId);

        // If we found the list, then we'll attempt to locate the exact container
        var foundContainer = GetContainer(foundList, containerId);

        // Bit of short-circuit logic here: if we're not actually updating the container name then we don't have to do anything
        if (string.Equals(updatedContainer.Name, foundContainer.Name))
        {
            return new ContainerDto(foundContainer);
        }

        // Now that we've found the container, we can make the actual update
        foundContainer.Name = updatedContainer.Name;

        try
        {
            // Update the entity
            _packedUnitOfWork.ContainerRepository.Update(foundContainer);

            // Save changes to data store
            await _packedUnitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            // If the exception is that we have a unique violation, then we throw a DuplicateListException
            if (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new DuplicateContainerException("A container with the same name already exists", e);
            }

            // Otherwise something else happen and we rethrow the exception
            throw;
        }

        return new ContainerDto(foundContainer);
    }

    /// <summary>
    /// Remove a container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="ContainerNotFoundException">Container could not be found</exception>
    public async Task DeleteContainerAsync(int listId, int containerId)
    {
        // Get list
        var foundList = await GetList(listId);

        // If we found the list, then we'll attempt to locate the exact container
        var foundContainer = GetContainer(foundList, containerId);

        // Finally, delete the container
        _packedUnitOfWork.ContainerRepository.Delete(foundContainer);
        await _packedUnitOfWork.SaveChangesAsync();
    }

    #endregion METHODS

    #region HELPER METHODS

    /// <summary>
    /// Retrieve the specified list and throw an exception if it does not exist
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <returns>
    /// The specified list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be found</exception>
    private async Task<List> GetList(int listId)
    {
        // Attempt to find the list
        var foundList = await _packedUnitOfWork.ListRepository.GetListByIdAsync(listId);

        // If we couldn't find the list, then throw a ListNotFoundException
        if (foundList is null)
        {
            throw new ListNotFoundException($"Could not find list with ID {listId}");
        }

        return foundList;
    }

    /// <summary>
    /// Retrieve a specific container from a list and ensure it exists
    /// </summary>
    /// <param name="foundList">The loaded list</param>
    /// <param name="containerId">The specific container ID</param>
    /// <returns>
    /// The specified container
    /// </returns>
    /// <exception cref="ContainerNotFoundException">The container could not be found</exception>
    private Container GetContainer(List foundList, int containerId)
    {
        // Try to find the container in the list's collection of containers
        var foundContainer = foundList.Containers
            .SingleOrDefault(c => c.Id == containerId);

        // If the container was not found, then throw an exception
        if (foundContainer is null)
        {
            throw new ContainerNotFoundException(
                $"Container with ID {containerId} could not be found in list with ID {foundList.Id}");
        }

        return foundContainer;
    }

    #endregion HELPER METHODS
}